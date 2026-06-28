using System;

namespace Hiero.Tools
{
    static class Program
    {
        static int Main(string[] args)
        {
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