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
        public List<Feedback> GetAllFeedbacks();
        public Feedback GetFeedbackById(int id);
        public void AddFeedback(Feedback newFeedback);
        public void UpdateFeedback(Feedback updatedFeedback);
        public void DeleteFeedback(int id);

    }
}
