using System.Text.Json.Serialization;

namespace Clean.Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SearchConversationStatus { NotUnderstood, NeedsMore, Results }

    public record SearchConversationResult(
        SearchConversationStatus Status,
        SearchIntent Intent,
        SearchField? NextField,
        List<CleanerMatch>? Matches);
}
