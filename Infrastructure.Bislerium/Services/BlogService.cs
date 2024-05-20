using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using Infrastructure.Bislerium.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.SignalR;
using Infrastructure.Bislerium.Hubs;

namespace Infrastructure.Bislerium.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        private readonly IHubContext<NotificationHubs ,INotificationClient> _hubContext;

        public BlogService(ApplicationDbContext context, IFileService fileService,IHubContext <NotificationHubs,INotificationClient> hubContext)
        {
            _context = context;
            _fileService = fileService;
            _hubContext = hubContext;
        }

        //for doing search

        public async Task<IEnumerable<BlogPost>> GetFilteredAndSortedBlogPostsAsync(string sortBy, int page, int pageSize, string searchQuery)
        {
            try
            {
                IQueryable<BlogPost> query = _context.BlogPosts;

                // Apply search query filter if provided
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(blog => blog.Title.Contains(searchQuery) || blog.Body.Contains(searchQuery));
                }

                // Apply sorting
                switch (sortBy.ToLower())
                {
                    case "random":
                        query = query.OrderBy(x => Guid.NewGuid());
                        break;
                    case "popularity":
                        // Prioritize popular posts by sorting by total popularity, higher scores first
                        query = query.OrderByDescending(x => x.TotalPopularity);
                        break;
                    case "recency":
                        query = query.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        // Default sorting by the last update date
                        query = query.OrderByDescending(x => x.LastUpdateDate);
                        break;
                }

                // Apply pagination
                query = query.Skip((page - 1) * pageSize).Take(pageSize);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                throw new ApplicationException($"Error retrieving blogs: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            return await _context.BlogPosts
                .Where(bp => bp.IsDeleted != true)
                .Include(bp => bp.Comments) // Include comments
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsByUsernameAsync(string username)
        {
            return await _context.BlogPosts
                .Where(bp => bp.IsDeleted != true && bp.UserName == username)
                .Include(bp => bp.Comments) // Include comments
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(string sortBy, int page, int pageSize)
        {
            IQueryable<BlogPost> query = _context.BlogPosts;

            // Apply filtering
            query = query.Where(x => !x.IsDeleted);

            // Apply sorting
            switch (sortBy.ToLower())
            {
                case "random":
                    query = query.OrderBy(x => Guid.NewGuid());
                    break;
                case "popularity":
                    // Prioritize popular posts by sorting by total popularity, higher scores first
                    query = query.OrderByDescending(x => x.TotalPopularity);
                    break;
                case "recency":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    // Default sorting by the last update date
                    query = query.OrderByDescending(x => x.LastUpdateDate);
                    break;
            }

            // Apply pagination
            var blogPosts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return blogPosts;
        }

        public async Task<IEnumerable<BlogPost>> GetPopularBlogsAsync(int page, int pageSize)
        {
            // Fetch data from the database
            var blogPosts = await _context.BlogPosts.ToListAsync();

            // Sort by popularity
            blogPosts = blogPosts.OrderByDescending(x => x.TotalPopularity).ToList();

            // Apply pagination
            return blogPosts.Skip((page - 1) * pageSize).Take(pageSize);
        }
        public async Task<IEnumerable<BlogPost>> GetRecentBlogsAsync(int page, int pageSize)
        {
            // Get recent non-deleted blogs sorted by recency
            return await _context.BlogPosts
                .Where(x=>x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<BlogPost> CreateBlogPostAsync(BlogPostCreatedDto blogPostDto, string userId, IFormFile file)
        {
            var blogPost = new BlogPost
            {
                Title = blogPostDto.Title,
                Body = blogPostDto.Body,
                ImageUrl = await _fileService.WriteFile(file),
                CreatedAt = DateTime.Now,
                UserId = userId,
                UserName=blogPostDto.UserName,
                UserProfile=blogPostDto.UserProfile
            };

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            // Create history entry for blog post creation
            var blogPostHistory = new BlogPostHistory
            {
                BlogPostId = blogPost.Id,
                Title = blogPost.Title,
                Body = blogPost.Body,
                UpdatedAt = DateTime.Now,
                UpdatedByUserId=userId,
                UserId = userId,
                ImageUrl= await _fileService.WriteFile(file),
                Action="Created",
                UpdatedByUserName=blogPost.UserName
            };

            _context.BlogPostHistories.Add(blogPostHistory);
            await _context.SaveChangesAsync();

            // Send notification to author
            await _hubContext.Clients.All.ReceiveNotification($"{blogPostDto.UserName} has created blog post '{blogPostDto.Title}' successfully!");

            return blogPost;
        }

        public async Task<BlogPost> EditBlogPostAsync(int postId, BlogPostUpdateDto blogPostDto, string userId, IFormFile file)
        {
            var blogPost = await _context.BlogPosts.FindAsync(postId);

            if (blogPost == null)
            {
                return null;
            }

            if (blogPost.UserId != userId)
            {
                return null;
            }

            // Create history entry for blog post before update
            var blogPostHistory = new BlogPostHistory
            {
                BlogPostId = blogPost.Id,
                Title = blogPostDto.Title,
                Body = blogPostDto.Body,
                UpdatedAt = DateTime.Now,
                UpdatedByUserId = userId,
                UserId = userId,
                ImageUrl = await _fileService.WriteFile(file),
                Action="Edited",
                UpdatedByUserName=blogPost.UserName,

            };

            _context.BlogPostHistories.Add(blogPostHistory);

            blogPost.Title = blogPostDto.Title;
            blogPost.Body = blogPostDto.Body;
            blogPost.ImageUrl = await _fileService.WriteFile(file);
            blogPost.LastUpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return blogPost;
        }


        public async Task<bool> DeleteBlogPostAsync(int postId, string userId)
        {
            var blogPost = await _context.BlogPosts.FindAsync(postId);

            if (blogPost == null)
            {
                return false;
            }

            if (blogPost.UserId != userId)
            {
                return false;
            }

            // Create history entry for blog post before deletion
            var blogPostHistory = new BlogPostHistory
            {
                BlogPostId = blogPost.Id,
                Title = blogPost.Title,
                Body = blogPost.Body,
                UpdatedAt = DateTime.Now,
                UserId = userId,
                ImageUrl= blogPost.ImageUrl,
                Action="Deleted",
                UpdatedByUserName = blogPost.UserName,

            };

            _context.BlogPostHistories.Add(blogPostHistory);

            // Perform soft delete by setting IsDeleted to true
            blogPost.IsDeleted = true;
            blogPost.DeletedAt = DateTime.Now;
            blogPost.DeletedByUserId = userId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BlogPostHistory>> GetBlogPostHistoryAsync(int postId)
        {
            return await _context.BlogPostHistories
                .Where(h => h.BlogPostId == postId)
                .OrderByDescending(h => h.UpdatedAt)
                .ToListAsync();
        }

        public async Task<BlogPost> GetBlogPostByIdAsync(int postId)
        {
            return await _context.BlogPosts
                .Where(bp => bp.Id == postId && bp.IsDeleted != true)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetTotalPopularBlogsAsync(int page, int pageSize)
        {
            return await _context.BlogPosts
                .OrderByDescending(bp => bp.TotalPopularity)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetTop10MostPopularBlogsAsync()
        {
            return await _context.BlogPosts
                .OrderByDescending(bp => bp.TotalPopularity)
                .Take(10)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<BlogPost>> GetTop10MostPopularBloggersAsync()
        {
            return await _context.BlogPosts
                .OrderByDescending(bp => bp.TotalPopularity)
                .Take(10)
                .ToListAsync();
        }




        public async Task<IEnumerable<BlogPost>> GetTop10MostPopularBlogsByMonthAndYearAsync(int year, int month)
        {
            return await _context.BlogPosts
                .Where(bp => bp.CreatedAt.Year == year && bp.CreatedAt.Month == month)
                .OrderByDescending(bp => bp.TotalPopularity)
                .Take(10)
                .ToListAsync();
        }


        public async Task<IEnumerable<BlogPost>> GetPaginatedAndSortableBlogsAsync(int page, int pageSize, string sortBy)
        {
            IQueryable<BlogPost> query = _context.BlogPosts;

            switch (sortBy.ToLower())
            {
                case "random":
                    query = query.OrderBy(_ => Guid.NewGuid());
                    break;
                case "popularity":
                    query = query.OrderByDescending(bp => bp.TotalPopularity);
                    break;
                case "recency":
                    query = query.OrderByDescending(bp => bp.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(bp => bp.CreatedAt);
                    break;
            }

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

    }
}