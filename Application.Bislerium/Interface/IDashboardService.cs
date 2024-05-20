// IDashboardService.cs
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface IDashboardService
    {
        //all list to show total in dashboard
        Task<int> GetTotalBlogPostsCount();

        //to ge total upvote count 
        Task<int> GetTotalUpvotesCount();

        //to ge total downvote count 
        Task<int> GetTotalDownvotesCount();

        //to ge total  comment 
        Task<int> GetTotalCommentsCount();

        //to ge total blog posts count by month 
        Task<int> GetTotalBlogPostsCountByMonth(int year, int month);

        //to ge total upvotes counts by month  
        Task<int> GetTotalUpvotesCountByMonth(int year, int month);

        //for get total downvotescount by month 
        Task<int> GetTotalDownvotesCountByMonth(int year, int month);

        //for geting total comments count by month
        Task<int> GetTotalCommentsCountByMonth(int year, int month);
    }
}
