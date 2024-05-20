using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface IBlogPostHistoryService
    {
        //getting history of blog post 
        Task<List<BlogPostHistory>> GetHistoriesByUserNameAsync(string userName);
    }
}
