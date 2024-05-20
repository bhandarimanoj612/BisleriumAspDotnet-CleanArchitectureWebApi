using Application.Bislerium.Interface;
using Domain.Bislerium.Models.Dto;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Bislerium.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata;

namespace Infrastructure.Bislerium.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHubs, INotificationClient> _hubContext;

        public CommentService(ApplicationDbContext context, IHubContext<NotificationHubs, INotificationClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Where(c => c.IsDeleted != true)
                .ToListAsync();
        }


        //for geting comment of users
        public async Task<IEnumerable<Comment>> GetCommentsForPostAsync(int postId)
        {
            return await _context.Comments
        .Where(c => c.BlogPostId == postId && c.IsDeleted != true)
        .ToListAsync();
        }

        //for adding comments in post
        public async Task<GeneralServiceResponseDto> AddCommentAsync(int postId, string userId, CommentDto commentDto)
        {
            var blogPost = await _context.BlogPosts.FindAsync(postId);
            if (blogPost == null)
            {
                return new GeneralServiceResponseDto { IsSucceed = false, StatusCode = 404, Message = "Blog post not found" };
            }

            var comment = new Comment
            {
                Content = commentDto.Content,
                CreatedAt = DateTime.Now,
                UserId = userId,
                BlogPostId = postId,
                UserName = commentDto.UserName,
                UserProfile=commentDto.UserProfile
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Create history entry for comment creation
            var commentHistory = new CommentHistory
            {
                CommentId = comment.Id,
                Content = comment.Content,
                UpdatedAt = DateTime.Now,
                UpdatedByUserId = userId,
                UserId = userId,
                Action="Created",
                UserName= comment.UserName,
              
            };

            _context.CommentHistories.Add(commentHistory);

            /// Calculate total popularity of the blog post
    int totalUpvotes = await _context.BlogReactions.Where(br => br.BlogPostId == postId && br.Reaction == "upvote").CountAsync();
            int totalDownvotes = await _context.BlogReactions.Where(br => br.BlogPostId == postId && br.Reaction == "downvote").CountAsync();
            int totalComments = await _context.Comments.Where(c => c.BlogPostId == postId && c.IsDeleted != true).CountAsync();
            int totalPopularity = (2 * totalUpvotes) + (-1 * totalDownvotes) + (1 * totalComments);

            // Update total popularity of the blog post
            blogPost.TotalPopularity = totalPopularity;

            await _context.SaveChangesAsync();

            // Send notification to the author of the blog post
            var notificationMessage = $"User {commentDto.UserName} commented on  blog post '{blogPost.Title}'.";
            await _hubContext.Clients.All.ReceiveNotification(notificationMessage);

            return new GeneralServiceResponseDto { IsSucceed = true, StatusCode = 200, Message = $" ('{commentDto.Content}'). Comment added successfully" };
        }

        public async Task<GeneralServiceResponseDto> EditCommentAsync(int commentId, int blogId, string userId, CommentDto commentDto)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return new GeneralServiceResponseDto { IsSucceed = false, StatusCode = 404, Message = "Comment not found" };
            }

            if (comment.UserId != userId)
            {
                return new GeneralServiceResponseDto { IsSucceed = false, StatusCode = 403, Message = "User not authorized to edit this comment" };
            }

            // Ensure that the comment belongs to the specified blog ID
            if (comment.BlogPostId != blogId)
            {
                return new GeneralServiceResponseDto { IsSucceed = false, StatusCode = 404, Message = "Comment does not belong to the specified blog" };
            }

            // Create history entry for comment before update
            var commentHistory = new CommentHistory
            {
                CommentId = comment.Id,
                Content = commentDto.Content,
                UpdatedAt = DateTime.Now,
                UserId = userId,
                UpdatedByUserId = userId,
                Action = "Edited",
                UserName= commentDto.UserName
               
            };

            _context.CommentHistories.Add(commentHistory);

            comment.Content = commentDto.Content;
            comment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new GeneralServiceResponseDto { IsSucceed = true, StatusCode = 200, Message = $"Comment edited successfully" };
        }



        public async Task<bool> DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return false; // Comment not found
            }

            if (comment.UserId != userId)
            {
                return false; // User is not authorized to delete this comment
            }

            var postId = comment.BlogPostId;

            // Calculate total popularity of the blog post before the comment deletion
            int totalUpvotes = await _context.BlogReactions.Where(br => br.BlogPostId == postId && br.Reaction == "upvote").CountAsync();
            int totalDownvotes = await _context.BlogReactions.Where(br => br.BlogPostId == postId && br.Reaction == "downvote").CountAsync();
            int totalComments = await _context.Comments.Where(c => c.BlogPostId == postId && c.IsDeleted != true).CountAsync();
            int totalPopularity = (2 * totalUpvotes) + (-1 * totalDownvotes) + (1 * totalComments);

            // Update total popularity of the blog post after subtracting the comment's popularity
            totalPopularity -= 1; // Assuming each comment adds a popularity of 1

            // Update total popularity of the blog post
            var blogPost = await _context.BlogPosts.FindAsync(postId);
            if (blogPost != null)
            {
                blogPost.TotalPopularity = totalPopularity;
            }

            // Create history entry for comment deletion
            var commentHistory = new CommentHistory
            {
                CommentId = comment.Id,
                Content = comment.Content,
                UpdatedAt = DateTime.Now,
                UpdatedByUserId = userId,
                UserId = userId,
                Action = "Deleted",
                UserName = comment.UserName
            };

            _context.CommentHistories.Add(commentHistory);

            // Soft delete the comment
            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            comment.DeletedByUserId = userId;

            await _context.SaveChangesAsync();

            return true;
        }


        //for comment history
        public async Task<IEnumerable<CommentHistory>> GetCommentHistoryAsync(int commentId)
        {
            return await _context.CommentHistories
                .Where(h => h.CommentId == commentId)
                .OrderByDescending(h => h.UpdatedAt)
                .ToListAsync();
        }


    }
}
