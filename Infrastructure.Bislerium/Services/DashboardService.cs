// DashboardService.cs
using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        //for showing totalblogpostscount 
        public async Task<int> GetTotalBlogPostsCount()
        {
            return await _context.BlogPosts.CountAsync();
        }
        //for showing total upvote count

        public async Task<int> GetTotalUpvotesCount()
        {
            return await _context.BlogReactions.SumAsync(br => br.TotalUpVotes);
        }

        //for getting total donwnvotescount
        public async Task<int> GetTotalDownvotesCount()
        {
            return await _context.BlogReactions.SumAsync(br => br.TotalDownVotes);
        }

        //for getting totalcount of commentscount
        public async Task<int> GetTotalCommentsCount()
        {
            return await _context.Comments.CountAsync();
        }
        //for getting total blog post count by month

        public async Task<int> GetTotalBlogPostsCountByMonth(int year, int month)
        {
            return await _context.BlogPosts
                .CountAsync(bp => bp.CreatedAt.Year == year && bp.CreatedAt.Month == month);
        }

        //for getting totalupvotescountsbymonth
        public async Task<int> GetTotalUpvotesCountByMonth(int year, int month)
        {
            return await _context.BlogReactions
                .Where(br => br.BlogPost.CreatedAt.Year == year && br.BlogPost.CreatedAt.Month == month)
                .SumAsync(br => br.TotalUpVotes);
        }
        //for gettingo total downvotescount by month
        public async Task<int> GetTotalDownvotesCountByMonth(int year, int month)
        {
            return await _context.BlogReactions
                .Where(br => br.BlogPost.CreatedAt.Year == year && br.BlogPost.CreatedAt.Month == month)
                .SumAsync(br => br.TotalDownVotes);
        }
        //for getting totallcommentscount by month
        public async Task<int> GetTotalCommentsCountByMonth(int year, int month)
        {
            return await _context.Comments
                .CountAsync(c => c.CreatedAt.Year == year && c.CreatedAt.Month == month);
        }
    }
}
