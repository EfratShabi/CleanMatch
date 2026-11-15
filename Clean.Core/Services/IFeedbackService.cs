using Clean.Core.Entities;

namespace Clean.Core.Services
{
    public interface IFeedbackService
    {
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<List<Feedback>> GetFeedbacksByCleanerAsync(int cleanerId);
        Task<Feedback?> GetFeedbackByIdAsync(int id);
        Task AddFeedbackAsync(Feedback newFeedback, Bookings booking);
        Task UpdateFeedbackAsync(Feedback updatedFeedback);
        Task DeleteFeedbackAsync(int id);
    }
}
