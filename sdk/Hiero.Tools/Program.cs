using System;

namespace Hiero.Tools
{
    static class Program
    {
        static int Main(string[] args)
        {
            //return ProtoTransformer.Run(
            //[
            //    string.Format("--src {0}", System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory().Split("bin")[0], "hapi")),
            //    string.Format("--output {0}", System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory().Split("bin")[0], "hapi.generated")),
            //    string.Format("--root {0}", "proto"),
            //    string.Format("--skip {0}", "\"publish_stream_request_bytes.proto\""),
            //]);

            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage: Hiero.Tools <tool> [args...]");
                return 1;
            }

            return args[0] switch
            {
                "transform-protos" => ProtoTransformer.Run(args[1..]),

                _ => Err($"Unknown tool: {args[0]}")
            };
        }

        static int Err(string msg) { Console.Error.WriteLine(msg); return 1; }
    }

}