using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clean.Core.Repositories
{
    public interface IFeedbackRepository
    {
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<Feedback?> GetFeedbackByIdAsync(int id);
        Task AddFeedbackAsync(Feedback newFeedback);
        Task UpdateFeedbackAsync(Feedback updatedFeedback);
        Task DeleteFeedbackAsync(int id);
        Task SaveAsync();
    }
}