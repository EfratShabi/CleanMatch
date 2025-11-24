using Clean.Core.Entities;
using Clean.Core.Repositories;
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
        public List<Feedback> GetAllFeedbacks()
        {
            return _context.Feedback.ToList();

        }

        public Feedback GetFeedbackById(int id)
        {
            var feedback = _context.Feedback.ToList().Find(feedback => feedback.Id == id);
            return feedback;
        }
        public void AddFeedback(Feedback newFeedback)
        {
            _context.Feedback.Add(newFeedback);
        }
        public void UpdateFeedback(Feedback updatedFeedback)
        {
            _context.Feedback.Update(updatedFeedback);
        }
        public  void DeleteFeedback(int id)
        {
            _context.Feedback.Remove(GetFeedbackById(id));
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
