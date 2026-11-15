using Clean.Core.Entities;

namespace Clean.Core.Repositories
{
    public interface IFeedbackRepository
    {
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<List<Feedback>> GetFeedbacksByCleanerAsync(int cleanerId);
        Task<Feedback?> GetFeedbackByIdAsync(int id);
        Task<bool> ExistsForBookingAsync(int bookingCode);
        Task AddFeedbackAsync(Feedback newFeedback);
        Task UpdateFeedbackAsync(Feedback updatedFeedback);
        Task DeleteFeedbackAsync(int id);
        Task SaveAsync();
    }
}
