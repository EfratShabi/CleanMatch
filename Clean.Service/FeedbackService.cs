using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Service
{
    public class FeedbackService: IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        public List<Feedback> GetAllFeedbacks()
        {
            return _feedbackRepository.GetAllFeedbacks();
        }
        public Feedback GetFeedbackById(int id)
        {
            return _feedbackRepository.GetFeedbackById(id);
        }
        public void AddFeedback(Feedback newFeedback)
        {
            _feedbackRepository.AddFeedback(newFeedback);
            _feedbackRepository.Save();
        }
        public void UpdateFeedback(Feedback updatedFeedback)
        {
            _feedbackRepository.UpdateFeedback(updatedFeedback);
            _feedbackRepository.Save();
        }
        public void DeleteFeedback(int id)
        {
            _feedbackRepository.DeleteFeedback(id);
            _feedbackRepository.Save();

        }
       
    }
}
