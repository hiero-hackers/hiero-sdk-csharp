using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Hiero.Tools
{
    public static partial class ProtoTransformer
    {
        private static readonly string Regex_Prefix = string.Empty;
        private static readonly string Regex_Suffix = string.Empty;
        private static readonly string Regex_RpcRequest = string.Empty;
        private static readonly string Regex_RequestType = string.Empty;
        private static readonly string Regex_ResponseType = string.Empty;

        [GeneratedRegex(@"^\s*import\s+""(?<Regex_Import>[^""]+)""\s*;", RegexOptions.Multiline)] 
        private static partial Regex Regex_Import();
        [GeneratedRegex(@"^\s*(?:message|enum)\s+(?<Regex_Class>[A-Za-z_]\w*)\s*\{", RegexOptions.Multiline)]
        private static partial Regex Regex_Class();
        [GeneratedRegex(@"^(\s*package\s+)(?<Regex_Package>[^;]*)(\s*;.*)$", RegexOptions.Multiline)] 
        private static partial Regex Regex_Package();
        [GeneratedRegex(@"^(\s*option\s+java_package\s*=\s*"")[^""]*("".*)$", RegexOptions.Multiline)] 
        private static partial Regex Regex_JavaPackage();
        [GeneratedRegex(@"^(?<Regex_Prefix>\s*//\s*<<<pbj\.java_package\s*=\s*"")[^""]*(?<Regex_Suffix>"">>>.*)", RegexOptions.Multiline)]
        private static partial Regex Regex_PbjJavaPackage();
        [GeneratedRegex(@"^(?<Regex_Prefix>\s+(?:repeated|optional|required)\s+|(?<!\w)\s*)(?<Regex_FieldType>\.?[A-Za-z][A-Za-z0-9_.]*)(?<Regex_Suffix>\s+\w+\s*=\s*\d+\s*;.*)$", RegexOptions.Multiline)]
        private static partial Regex Regex_FieldType();
        [GeneratedRegex(@"^\s*rpc\s+\w+\s*\((?<Regex_RpcRequest>stream\s+)?(?<Regex_RequestType>\.?[A-Za-z][A-Za-z0-9_.]*)\s*\)\s*returns\s*\(\s*(?<Regex_RpcResponse>stream\s+)?(?<Regex_ResponseType>\.?[A-Za-z][A-Za-z0-9_.]*)\s*\)\s*;", RegexOptions.Multiline)]
        private static partial Regex Regex_RpcMethod();

        public static int Run(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: transform-protos --src <sourceDir> --output <outputDir> --root <protoRoot> [--skip <file1> <file2> ...]");
                return 1;
            }

            HashSet<string> skipFiles = [];
            string? sourceDir = null, outputDir = null, protoRoot = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--src":
                        sourceDir = Path.GetFullPath(args[i + 1]);
                        break;
                    case "--output":
                        outputDir = Path.GetFullPath(args[i + 1]);
                        break;
                    case "--root":
                        protoRoot = string.Format("{0}\\", args[i + 1].Trim('\\'));
                        break;
                    case "--skip":
                        foreach (string _skip in args[i + 1].Split("\"", StringSplitOptions.RemoveEmptyEntries))
                            skipFiles.Add(_skip.Trim());
                        break;
                    default:
                        Console.Error.WriteLine($"Unknown argument: {args[i + 1]}");
                        return 1;
                }

                i++;
            }

            outputDir ??= string.Format("{0}.generated", sourceDir);

            if (sourceDir is null)
            {
                Console.Error.WriteLine("Missing required arguments. Usage: transform-protos --src <sourceDir>");
                return 1;
            }

            if (!Directory.Exists(sourceDir))
            {
                Console.Error.WriteLine($"Source directory not found: {sourceDir}");
                return 1;
            }

            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            IList<string> files = [..Directory
                .EnumerateFiles(sourceDir, "*.proto", SearchOption.AllDirectories)
                .Where(_ => skipFiles.Any(__ => _.EndsWith(__)) is false)
                .OrderBy(f => f)];

            if (files.Count == 0)
            {
                Console.Error.WriteLine($"No .proto files found under {sourceDir}");
                return 1;
            }

            // Pass One: collect all message names and their source file paths
            Console.WriteLine("Pass One");

            IList<ProtoFile> protoFiles = [.. files.Select(file =>
            {
                string filepath = Path.GetRelativePath(sourceDir, file);
                string directory = Path.GetDirectoryName(filepath)!;
                string path = Path.Combine(Path.GetFileName(sourceDir), directory);
                string package = protoRoot is not null && path.Contains(protoRoot) is false
                    ? path
                    : string.Format("{0}{1}", protoRoot, string.Join("", path.Split(protoRoot)[1..]));

                Console.WriteLine("File: {0}", filepath);

                if (filepath.Contains("consensus_service") || filepath.Contains("smart_contract_service"))
                { }

                string text = File.ReadAllText(file, Encoding.UTF8);

                return new ProtoFile
                {
                    FilePath = filepath,
                    Package = package.Replace(Path.DirectorySeparatorChar, '.'),
                    Classes = [.. Regex_Class().Matches(text).Select(match =>
                    {
                        ProtoClass protoClass = new()
                        {
                            Name = match.Groups[nameof(Regex_Class)].Value,
                        };

                        Console.WriteLine("Proto Class: {0}", protoClass.Name);

                        return protoClass;
                    })]
                };
            })];

            Dictionary<string, ProtoFile> fileByPath = protoFiles.ToDictionary(f => f.FilePath);
            Dictionary<string, string> packageByMessageName = protoFiles
                .SelectMany(f => f.Classes.Select(m => (m.Name, f.Package)))
                .GroupBy(x => x.Name)
                .ToDictionary(g => g.Key, g => g.Count() == 1 ? g.First().Package : null!);

            // Pass Two: rewrite each file
            Console.WriteLine("Pass Two");

            foreach (string file in files)
            {
                string filePath = Path.GetRelativePath(sourceDir, file);
                string[] lines = File.ReadAllLines(file, Encoding.UTF8);

                ProtoFile protoFile = fileByPath[filePath];
                 
                string javaPackage = string.Format("com.hiero.{0}", protoFile.Package);
                string csharpNamespace = string.Format("Hiero.{0}", string.Join(".", protoFile.Package.Split('.').Select(_ =>
                {
                    return string.Format("{0}{1}", char.ToUpper(_[0]), _[1..]);
                })));

                HashSet<string> localMessageNames = [.. protoFile.Classes.Select(m => m.Name)];

                List<string> output = new(lines.Length + 2);
                bool csharpInserted = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    // 0. Rewrite import paths to use proto-root-relative forward-slash paths
                    Match importMatch = Regex_Import().Match(line);

                    if (importMatch.Success)
                    {
                        string rawImportPath = importMatch.Groups[nameof(Regex_Import)].Value;
                        string importFileName = Path.GetFileName(rawImportPath);

                        // Find the ProtoFile whose FilePath filename matches this import
                        if (protoFiles.FirstOrDefault(f => f.FilePath.Replace(Path.DirectorySeparatorChar, '/').EndsWith(rawImportPath)) is ProtoFile matchedFile)
                        {
                            // Rebuild the import path using forward slashes (proto standard)
                            string newImportPath = string.Format("{0}/{1}", matchedFile.Package["proto.".Length..].Replace('.', '/'), importFileName);
                            output.Add($"import \"{newImportPath}\";");
                        }
                        else
                        {
                            // Unknown import — pass through unchanged
                            output.Add(line);
                        }

                        continue;
                    }

                    // 1. Rewrite package declaration to use the proto-relative directory path
                    Match packageMatch = Regex_Package().Match(line);

                    if (packageMatch.Success)
                    {
                        string newLine = string.Format("package {0};", protoFile.Package);
                        output.Add(newLine);
                        continue;
                    }

                    // 2. Rewrite java_package option and inject csharp_namespace directly after
                    Match javaPackageMatch = Regex_JavaPackage().Match(line);

                    if (javaPackageMatch.Success)
                    {
                        output.Add(string.Format("{0}{1}{2}", javaPackageMatch.Groups[1].Value, javaPackage, javaPackageMatch.Groups[2].Value));

                        if (csharpInserted is false)
                        {
                            output.Add(string.Format("option csharp_namespace = \"{0}\";", csharpNamespace));

                            csharpInserted = true;
                        }

                        continue;
                    }

                    // 3. Qualify cross-file message type references so Google.Protobuf's C# codegen resolves them unambiguously.
                    Match fieldMatch = Regex_FieldType().Match(line);

                    if (fieldMatch.Success)
                    {
                        string typeName = fieldMatch.Groups[nameof(Regex_FieldType)].Value;
                        string simpleName = typeName.Split('.')[^1];

                        // Only rewrite if the simple name resolves to a known ProtoClass
                        if (packageByMessageName.TryGetValue(simpleName, out string? externalPackage) 
                            && externalPackage is not null
                            && (!localMessageNames.Contains(simpleName) || typeName.Contains('.')))
                        {
                            string qualified = string.Format("{0}.{1}", externalPackage, simpleName);

                            line = string.Format("{0}{1}{2}", fieldMatch.Groups[nameof(Regex_Prefix)].Value, qualified, fieldMatch.Groups[nameof(Regex_Suffix)].Value);

                            Console.WriteLine("  Qualified: {0} -> {1}", typeName, qualified);
                        }
                    }

                    // 4. Rewrite pbj.java_package comment to match the rewritten package declaration
                    Match pbjMatch = Regex_PbjJavaPackage().Match(line);

                    if (pbjMatch.Success)
                        line = string.Format("{0}{1}{2}", pbjMatch.Groups[nameof(Regex_Prefix)].Value, javaPackage, pbjMatch.Groups[nameof(Regex_Suffix)].Value);

                    // 5. Qualify cross-file message type references in rpc declarations
                    Match rpcMatch = Regex_RpcMethod().Match(line);

                    if (rpcMatch.Success)
                    {
                        foreach (string groupName in new[] { nameof(Regex_RequestType), nameof(Regex_ResponseType) })
                        {
                            string typeName = rpcMatch.Groups[groupName].Value;
                            string strippedTypeName = typeName.TrimStart('.');
                            string simpleName = strippedTypeName.Split('.')[^1];

                            if (packageByMessageName.TryGetValue(simpleName, out string? externalPackage)
                                && externalPackage is not null
                                && (!localMessageNames.Contains(simpleName) || strippedTypeName.Contains('.')))
                            {
                                string qualified = string.Format("{0}.{1}", externalPackage, simpleName);
                                line = Regex.Replace(line, $@"(?<![A-Za-z0-9_.]){Regex.Escape(typeName)}(?![A-Za-z0-9_.])", qualified);

                                Console.WriteLine("  Qualified RPC type: {0} -> {1}", typeName, qualified);
                            }
                        }

                        output.Add(line);
                        continue;
                    }

                    output.Add(line); 
                }

                // Write rewritten file to output directory, preserving subdirectory structure
                string outputPath = Path.Combine(outputDir, filePath);
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                File.WriteAllText(outputPath, string.Join('\n', output), Encoding.UTF8);

                Console.WriteLine("Written: {0}", filePath);
            }

            return 0;
        }
    }

    public class ProtoFile
    {
        public required string FilePath { get; init; }
        public required List<ProtoClass> Classes { get; init; }
        public required string Package { get; init; }
    }

    public class ProtoClass
    {
        public required string Name { get; init; }
    }
}
