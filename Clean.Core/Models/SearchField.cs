using System.Text.Json.Serialization;

namespace Clean.Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SearchField { Date, Start, Hours }
}
