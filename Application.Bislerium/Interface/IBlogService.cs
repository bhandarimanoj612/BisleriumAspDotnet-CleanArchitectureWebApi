using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Application.Bislerium.Interface
{
    public interface IBlogService
    {
        //for search
        Task<IEnumerable<BlogPost>> GetFilteredAndSortedBlogPostsAsync(string sortBy, int page, int pageSize, string searchQuery);

        //for geting all post async
        Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync();

        //for getting all blogpost by username 

        Task<IEnumerable<BlogPost>> GetAllBlogPostsByUsernameAsync(string username);

        //to create new blog posts
        Task<BlogPost> CreateBlogPostAsync(BlogPostCreatedDto blogPostDto, string userId, IFormFile file);

        //for editing blog post 
        Task<BlogPost> EditBlogPostAsync(int postId, BlogPostUpdateDto blogPostDto, string userId, IFormFile file);

        //for deleting blog posts
        Task<bool> DeleteBlogPostAsync(int postId, string userId);
        //random
        Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(string sortBy, int page, int pageSize);

        //popular
        Task<IEnumerable<BlogPost>> GetPopularBlogsAsync(int page, int pageSize);
        //recents
        Task<IEnumerable<BlogPost>> GetRecentBlogsAsync(int page, int pageSize);

        Task<IEnumerable<BlogPostHistory>> GetBlogPostHistoryAsync(int postId);

        Task<BlogPost> GetBlogPostByIdAsync(int postId);


     //total totalpopulation
        Task<IEnumerable<BlogPost>> GetTotalPopularBlogsAsync(int page, int pageSize);
        Task<IEnumerable<BlogPost>> GetTop10MostPopularBlogsAsync();
        Task<IEnumerable<BlogPost>> GetTop10MostPopularBloggersAsync();
        Task<IEnumerable<BlogPost>> GetTop10MostPopularBlogsByMonthAndYearAsync(int year, int month);
        Task<IEnumerable<BlogPost>> GetPaginatedAndSortableBlogsAsync(int page, int pageSize, string sortBy);
     //total population
    }
}
