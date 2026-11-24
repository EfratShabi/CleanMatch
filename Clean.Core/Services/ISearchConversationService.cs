using Clean.Core.Entities;
using Clean.Core.Models;

namespace Clean.Core.Services
{
    public interface ISearchConversationService
    {
        Task<SearchConversationResult> HandleAsync(
            Customers customer, string text, SearchIntent? known, SearchField? asked, CancellationToken ct = default);
    }
}
