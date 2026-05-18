using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hiero.SDK.Utils
{
    internal class JsonHelper
    {
        public static readonly JsonSerializerOptions Options = new ()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        public static string ToJsonString(object obj, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Serialize(obj, options ?? Options);
        }
	}
}
