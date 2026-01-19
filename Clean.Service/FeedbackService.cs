using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Clean.Service
{
    public class FeedbackService: IFeedbackService
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
