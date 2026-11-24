using Clean.Core.Models;

namespace Clean.Core.Services
{
    public interface ISearchIntentService
    {
        Task<SearchIntent?> InterpretAsync(string customerText, SearchField? asked = null, CancellationToken ct = default);
    }
}
