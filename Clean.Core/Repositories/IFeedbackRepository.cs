using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Repositories
{
    public interface IFeedbackRepository
    {
        public List<Feedback> GetAllFeedbacks();
        public Feedback GetFeedbackById(int id);
    }
}
