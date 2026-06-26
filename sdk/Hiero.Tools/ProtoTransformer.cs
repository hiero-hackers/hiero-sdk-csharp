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
        [GeneratedRegex(@"^\s*message\s+(?<Regex_Message>[A-Za-z_]\w*)\s*\{", RegexOptions.Multiline)] private static partial Regex Regex_Message();
        [GeneratedRegex(@"^(\s*package\s+)(?<Regex_Package>[^;]*)(\s*;.*)$", RegexOptions.Singleline)] private static partial Regex Regex_Package();
        [GeneratedRegex(@"^(\s*option\s+java_package\s*=\s*"")[^""]*("".*)$", RegexOptions.Singleline)] private static partial Regex Regex_JavaPackage();

        public static int Run(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: transform-protos <sourceDir> <outputDir>");
                return 1;
            }

            string sourceDir = Path.GetFullPath(args[0]);
            string outputDir = Path.GetFullPath(args[1]);

            if (!Directory.Exists(sourceDir))
            {
                Console.Error.WriteLine($"Source directory not found: {sourceDir}");
                return 1;
            }

            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            IList<string> files = [..Directory
                .EnumerateFiles(sourceDir, "*.proto", SearchOption.AllDirectories)
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
                string text = File.ReadAllText(file, Encoding.UTF8);

                Console.WriteLine("File: {0}", filepath);

                return new ProtoFile
                {
                    FilePath = filepath,
                    Package = directory[directory.IndexOf(@"proto\") ..].Replace(@"\", "."),
                    Messages = [.. Regex_Message().Matches(text).Select(match =>
                    {
                        ProtoMessage protoMessage = new()
                        {
                            Name = match.Groups[nameof(Regex_Message)].Value,
                        };

                        Console.WriteLine("Message: {0}", protoMessage.Name);

                        return protoMessage;
                    })]
                };
            })];

            Dictionary<string, ProtoFile> fileByPath = protoFiles.ToDictionary(f => f.FilePath);

            // Pass Two: rewrite each file
            Console.WriteLine("Pass Two");

            foreach (string file in files)
            {
                string filePath = Path.GetRelativePath(sourceDir, file);
                string[] lines = File.ReadAllLines(file, Encoding.UTF8);

                ProtoFile protoFile = fileByPath[filePath];
                 
                string javaPackage = string.Format("com.hiero.{0}", protoFile.Package);
                string csharpNamespace = string.Format("Hiero.{0}", string.Join(".", protoFile.Package.Split().Select(_ =>
                {
                    return string.Format("{0}{1}", char.ToUpper(_[0]), _[1..]);
                })));

                List<string> output = new(lines.Length + 2);
                bool csharpInserted = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

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

    public struct ProtoFile
    {
        public string FilePath { get; set; }
        public List<ProtoMessage> Messages { get; set; }
        public string Package { get; set; }
    }

    public struct ProtoMessage
    {
        public string Name { get; set; }
    }
}
