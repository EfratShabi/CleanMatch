using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;

namespace Clean.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly TimeProvider _timeProvider;
        private readonly HashSet<string> _badwords;

        public FeedbackService(IFeedbackRepository feedbackRepository, TimeProvider timeProvider)
        {
            _feedbackRepository = feedbackRepository;
            _timeProvider = timeProvider;
            _badwords = LoadBadWords();
        }

        // AppContext.BaseDirectory is the folder of the built app, where BadWords.txt is copied to.
        // (The current directory depends on where the app was started from, and may not contain the file.)
        private static HashSet<string> LoadBadWords()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "BadWords.txt");
            if (!File.Exists(path))
            {
                return new HashSet<string>();
            }

            var words = File.ReadAllLines(path)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0);
            return new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _feedbackRepository.GetAllFeedbacksAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksByCleanerAsync(int cleanerId)
        {
            return await _feedbackRepository.GetFeedbacksByCleanerAsync(cleanerId);
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _feedbackRepository.GetFeedbackByIdAsync(id);
        }

        public async Task AddFeedbackAsync(Feedback newFeedback, Bookings booking)
        {
            ReviewRules.Validate(newFeedback.Rating, newFeedback.TextOfFeedback);
            if (ReviewRules.ContainsBadWord(newFeedback.TextOfFeedback, _badwords))
            {
                throw new ArgumentException("הטקסט מכיל תוכן לא הולם.");
            }

            if (booking.Status != BookingStatus.Confirmed || !booking.HasEnded(IsraelTime.Now(_timeProvider)))
            {
                throw new InvalidOperationException("אפשר לדרג רק הזמנה שהסתיימה.");
            }
            if (await _feedbackRepository.ExistsForBookingAsync(booking.BookingCode))
            {
                throw new InvalidOperationException("כבר דירגת את ההזמנה הזו.");
            }

            newFeedback.CleanerId = booking.CleanersId;
            newFeedback.CreatedAt = _timeProvider.GetUtcNow().UtcDateTime;
            await _feedbackRepository.AddFeedbackAsync(newFeedback);
            await _feedbackRepository.SaveAsync();
        }

        public async Task UpdateFeedbackAsync(Feedback updatedFeedback)
        {
            await _feedbackRepository.UpdateFeedbackAsync(updatedFeedback);
            await _feedbackRepository.SaveAsync();
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            await _feedbackRepository.DeleteFeedbackAsync(id);
            await _feedbackRepository.SaveAsync();
        }
    }
}
