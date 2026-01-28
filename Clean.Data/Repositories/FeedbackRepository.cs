using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.Feedback.ToListAsync();
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _context.Feedback.FindAsync(id);
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