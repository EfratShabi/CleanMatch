using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Services
{
    public interface IFeedbackService
    {
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<Feedback?> GetFeedbackByIdAsync(int id);
        Task AddFeedbackAsync(Feedback newFeedback);
        Task UpdateFeedbackAsync(Feedback updatedFeedback);
        Task DeleteFeedbackAsync(int id);
    }
}