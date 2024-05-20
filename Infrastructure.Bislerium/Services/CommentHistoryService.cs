using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class CommentHistoryService: ICommentHistoryService
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentHistoryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CommentHistory>> GetHistoriesByUserNameAsync(string userName)
        {
            return await _dbContext.CommentHistories
                 .Include(history => history.User)
                 .Where(history => history.UserName == userName)
                 .ToListAsync();
        }
    }
}
