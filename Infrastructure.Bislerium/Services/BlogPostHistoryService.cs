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
    public class BlogPostHistoryService:IBlogPostHistoryService
    {
        private readonly ApplicationDbContext _context;
        public BlogPostHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPostHistory>> GetHistoriesByUserNameAsync(string userName)
        {
            return await _context.BlogPostHistories
                .Include(history => history.User)
                .Where(history => history.UpdatedByUserName == userName)
                .ToListAsync();
        }
    }
}
