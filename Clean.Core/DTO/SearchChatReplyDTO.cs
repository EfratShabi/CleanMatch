using Clean.Core.Models;

namespace Clean.Core.DTO
{
    public record SearchChatReplyDTO(
        SearchConversationStatus Status,
        SearchIntent Intent,
        SearchField? NextField,
        List<CleanerSearchResultDTO>? Results);
}
