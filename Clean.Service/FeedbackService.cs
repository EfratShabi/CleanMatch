using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly HashSet<string> _badwords;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
            _badwords = LoadBadWords();
        }

        private HashSet<string> LoadBadWords()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "BadWords.txt");

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                return new HashSet<string>(lines, StringComparer.OrdinalIgnoreCase);
            }

            return new HashSet<string>();
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _feedbackRepository.GetAllFeedbacksAsync();
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _feedbackRepository.GetFeedbackByIdAsync(id);
        }

        public async Task AddFeedbackAsync(Feedback newFeedback)
        {
            // כאן אפשר להוסיף את הבדיקה של המילים הרעות מול _badwords אם צריך
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