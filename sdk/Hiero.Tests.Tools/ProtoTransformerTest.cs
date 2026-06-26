using Hiero.Tools;

namespace Hiero.Tests.Tools
{
    public class ProtoTransformerTest
    {
        private static readonly string HapiDir = Path.GetFullPath(@"..\..\..\..\..\hapi");
        private static readonly string OutputDir = Path.GetFullPath(@"..\..\..\..\..\hapi.generated");

        [Fact]
        public void TransformProtos_RunsWithoutError()
        {
            int result = ProtoTransformer.Run([HapiDir, OutputDir]);
            Assert.Equal(0, result);
        }
    }
}
