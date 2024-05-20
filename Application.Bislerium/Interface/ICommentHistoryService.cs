using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface ICommentHistoryService
    {
        //for showing comment history
        Task<List<CommentHistory>> GetHistoriesByUserNameAsync(string userName);
    }
}
