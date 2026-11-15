using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clean.Data.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DataContext _context;
        public FeedbackRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _context.Feedback.OrderByDescending(f => f.CreatedAt).ToListAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksByCleanerAsync(int cleanerId)
        {
            return await _context.Feedback
                .Where(f => f.CleanerId == cleanerId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _context.Feedback.FindAsync(id);
        }

        public async Task<bool> ExistsForBookingAsync(int bookingCode)
        {
            return await _context.Feedback.AnyAsync(f => f.BookingCode == bookingCode);
        }

        public async Task AddFeedbackAsync(Feedback newFeedback)
        {
            await _context.Feedback.AddAsync(newFeedback);
        }

        public async Task UpdateFeedbackAsync(Feedback updatedFeedback)
        {
            _context.Feedback.Update(updatedFeedback);
            await Task.CompletedTask;
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            var feedback = await GetFeedbackByIdAsync(id);
            if (feedback != null)
            {
                _context.Feedback.Remove(feedback);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
