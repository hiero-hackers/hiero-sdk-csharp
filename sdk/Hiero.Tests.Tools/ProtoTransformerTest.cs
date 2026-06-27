using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hiero.Tests.Tools
{
    public class ProtoTransformerTest
    {
        /*
         * <Target Name="TransformProtos" BeforeTargets="BeforeBuild">
         *   <Exec Command="dotnet run --project &quot;$(MSBuildThisFileDirectory)..\Hiero.Tools\Hiero.Tools.csproj&quot; -- transform-protos &quot;$(MSBuildThisFileDirectory)hapi&quot; &quot;$(MSBuildThisFileDirectory)hapi.generated&quot;" ConsoleToMSBuild="true" />
         * </Target>
         */

        private static readonly string ProjectDir = Directory.GetCurrentDirectory().Split("bin")[0];
        private static readonly string SourceDir = Path.Combine(ProjectDir, "proto");
        private static readonly string GeneratedDir = Path.Combine(ProjectDir, "proto.generated");

        [Fact]
        public void FileSetsMatch()
        {
            IEnumerable<string> sourceFiles = Directory
                .EnumerateFiles(SourceDir, "*.proto", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(SourceDir, f))
                .OrderBy(f => f);

            IEnumerable<string> generatedFiles = Directory
                .EnumerateFiles(GeneratedDir, "*.proto", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(GeneratedDir, f))
                .OrderBy(f => f);

            Assert.Equal(sourceFiles, generatedFiles);
        }
        [Fact]
        public void PackageRewritten()
        {
            foreach (string generatedFile in Directory.EnumerateFiles(GeneratedDir, "*.proto", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(GeneratedDir, generatedFile);
                string[] lines = File.ReadAllLines(generatedFile);

                string? packageLine = lines.FirstOrDefault(l => Regex.IsMatch(l, @"^\s*package\s+"));
                Assert.NotNull(packageLine);

                // Package must start with "proto." and use the directory structure
                Assert.Matches(@"^\s*package\s+proto(\.[a-z][a-z0-9_]*)+\s*;", packageLine);

                // The declared package must match the file's own subdirectory path
                string expectedPackageSuffix = Path
                    .GetDirectoryName(relativePath)!
                    .Replace(Path.DirectorySeparatorChar, '.');

                Assert.Contains(expectedPackageSuffix, packageLine);
            }
        }
        [Fact]
        public void JavaPackageRewritten()
        {
            foreach (string generatedFile in Directory.EnumerateFiles(GeneratedDir, "*.proto", SearchOption.AllDirectories))
            {
                string[] lines = File.ReadAllLines(generatedFile);

                string? javaLine = lines.FirstOrDefault(l => Regex.IsMatch(l, @"option\s+java_package"));
                if (javaLine is null) continue; // not all protos have this option

                Assert.Matches(@"option\s+java_package\s*=\s*""com\.hiero\.", javaLine);
            }
        }
        [Fact]
        public void CsharpNamespaceInjected()
        {
            foreach (string generatedFile in Directory.EnumerateFiles(GeneratedDir, "*.proto", SearchOption.AllDirectories))
            {
                string[] lines = File.ReadAllLines(generatedFile);

                string? csharpLine = lines.FirstOrDefault(l => Regex.IsMatch(l, @"option\s+csharp_namespace"));
                Assert.NotNull(csharpLine);

                // Must start with Hiero. and use PascalCase segments
                Assert.Matches(@"option\s+csharp_namespace\s*=\s*""Hiero(\.[A-Z][A-Za-z0-9]*)+""", csharpLine);

                // csharp_namespace must appear directly after java_package (within 1 line)
                string? javaLine = lines.FirstOrDefault(l => Regex.IsMatch(l, @"option\s+java_package"));
                if (javaLine is not null)
                {
                    int javaIdx = Array.IndexOf(lines, javaLine);
                    int csharpIdx = Array.IndexOf(lines, csharpLine);
                    Assert.Equal(javaIdx + 1, csharpIdx);
                }
            }
        }
        [Fact]
        public void CrossFileReferencesQualified()
        {
            // Collect all message names defined across all *generated* files
            // and which package they belong to
            Dictionary<string, string> packageByMessage = [];

            foreach (string generatedFile in Directory.EnumerateFiles(GeneratedDir, "*.proto", SearchOption.AllDirectories))
            {
                string[] lines = File.ReadAllLines(generatedFile);

                string? packageLine = lines.FirstOrDefault(l => Regex.IsMatch(l, @"^\s*package\s+"));
                if (packageLine is null) continue;

                Match pkgMatch = Regex.Match(packageLine, @"package\s+(?<pkg>[^;]+)\s*;");
                if (!pkgMatch.Success) continue;

                string pkg = pkgMatch.Groups["pkg"].Value.Trim();

                foreach (string line in lines)
                {
                    Match msgMatch = Regex.Match(line, @"^\s*message\s+(?<name>[A-Za-z_]\w*)\s*\{");
                    if (msgMatch.Success)
                        packageByMessage.TryAdd(msgMatch.Groups["name"].Value, pkg);
                }
            }

            // Now verify: any field whose type is a known message from a different package
            // must use the leading-dot fully-qualified form
            foreach (string generatedFile in Directory.EnumerateFiles(GeneratedDir, "*.proto", SearchOption.AllDirectories))
            {
                string[] lines = File.ReadAllLines(generatedFile);

                string? packageLine = lines.FirstOrDefault(l => Regex.IsMatch(l, @"^\s*package\s+"));
                if (packageLine is null) continue;

                Match pkgMatch = Regex.Match(packageLine, @"package\s+(?<pkg>[^;]+)\s*;");
                if (!pkgMatch.Success) continue;

                string localPackage = pkgMatch.Groups["pkg"].Value.Trim();

                foreach (string line in lines)
                {
                    // Match unqualified field type references (no leading dot)
                    Match fieldMatch = Regex.Match(line,
                        @"^\s+(?:repeated|optional|required\s+)?(?<type>[A-Z][A-Za-z0-9_]*)\s+\w+\s*=\s*\d+\s*;");

                    if (!fieldMatch.Success) continue;

                    string typeName = fieldMatch.Groups["type"].Value;

                    if (packageByMessage.TryGetValue(typeName, out string? ownerPackage)
                        && ownerPackage != localPackage)
                    {
                        // A bare (unqualified) reference to a type from another package was found
                        Assert.Fail(
                            $"File '{Path.GetRelativePath(GeneratedDir, generatedFile)}' line '{line.Trim()}': " +
                            $"type '{typeName}' is defined in package '{ownerPackage}' but referenced without " +
                            $"full qualification. Expected '.{ownerPackage}.{typeName}'.");
                    }
                }
            }
        }
    }
}