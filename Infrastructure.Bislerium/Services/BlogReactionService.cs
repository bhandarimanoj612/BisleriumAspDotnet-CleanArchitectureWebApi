using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Data;
using Infrastructure.Bislerium.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class BlogReactionService : IBlogReactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHubs, INotificationClient> _hubContext;

        public BlogReactionService(ApplicationDbContext context, IHubContext<NotificationHubs, INotificationClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<BlogReaction> AddReactionAsync(int blogPostId, string userName, ReactionCategory reactionCategory)
        {
            // Find the existing reaction of the user for the specified blog post
            var existingReaction = await _context.BlogReactions.FirstOrDefaultAsync(br => br.BlogPostId == blogPostId && br.UserName == userName);

            // If an existing reaction is found
            if (existingReaction != null)
            {
                // If the existing reaction is the same as the new reaction, remove it
                if (existingReaction.Reaction == reactionCategory.Content)
                {
                    _context.BlogReactions.Remove(existingReaction);
                    await _context.SaveChangesAsync();

                    // Update total reactions of the blog post
                    await UpdateTotalReactions(blogPostId);

                    // Update total Popularity of the blog post
                    await UpdateTotalPopularity(blogPostId);

                    return null; // Return null to indicate that the reaction is removed
                }

                // Toggle reaction (from upvote to downvote or vice versa)
                if (existingReaction.Reaction == "upvote")
                {
                    existingReaction.Reaction = "downvote";
                    existingReaction.TotalUpVotes = 0;
                    existingReaction.TotalDownVotes = 1;
                }
                else if (existingReaction.Reaction == "downvote")
                {
                    existingReaction.Reaction = "upvote";
                    existingReaction.TotalUpVotes = 1;
                    existingReaction.TotalDownVotes = 0;
                }

                await _context.SaveChangesAsync();

                // Update total reactions of the blog post
                await UpdateTotalReactions(blogPostId);

                // Update total Popularity of the blog post
                await UpdateTotalPopularity(blogPostId);

                return existingReaction; // Return the existing reaction with the toggled values
            }

            // Create a new reaction if no existing reaction found
            var newReaction = new BlogReaction
            {
                BlogPostId = blogPostId,
                UserName = userName,
                Reaction = reactionCategory.Content,
                CreatedAt = DateTime.Now
            };

            // Update total upvotes or downvotes based on the reaction category
            if (reactionCategory.Content == "upvote")
            {
                newReaction.TotalUpVotes = 1;
            }
            else if (reactionCategory.Content == "downvote")
            {
                newReaction.TotalDownVotes = 1;
            }

            _context.BlogReactions.Add(newReaction);
            await _context.SaveChangesAsync();

            // Update total reactions of the blog post
            await UpdateTotalReactions(blogPostId);

            //Update Popularity
            await UpdateTotalPopularity(blogPostId);

            //// Send notification to the author
            //var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            //if (blogPost != null)
            //{
            //    if (reactionCategory.Content == "upvote")
            //    {
            //        await _hubContext.Clients.User(blogPost.UserId).ReceiveNotification($"User {userName} upvoted your blog post '{blogPost.Title}'.");
            //    }
            //    else if (reactionCategory.Content == "downvote")
            //    {
            //        await _hubContext.Clients.User(blogPost.UserId).ReceiveNotification($"User {userName} downvoted your blog post '{blogPost.Title}'.");
            //    }
            //}


            // Send notification to all users
            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            if (blogPost != null)
            {
                if (reactionCategory.Content == "upvote")
                {
                    await _hubContext.Clients.All.ReceiveNotification($"User {userName} upvoted the blog post '{blogPost.Title}'.");
                }
                else if (reactionCategory.Content == "downvote")
                {
                    await _hubContext.Clients.All.ReceiveNotification($"User {userName} downvoted the blog post '{blogPost.Title}'.");
                }
            }

            return newReaction;
        }


        // Method to update the total reactions of the blog post
        private async Task UpdateTotalReactions(int blogPostId)
        {
            var totalUpvotes = await _context.BlogReactions
                .Where(br => br.BlogPostId == blogPostId && br.Reaction == "upvote")
                .CountAsync();

            var totalDownvotes = await _context.BlogReactions
                .Where(br => br.BlogPostId == blogPostId && br.Reaction == "downvote")
                .CountAsync();

            var totalReactions = totalUpvotes - totalDownvotes;

            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            if (blogPost != null)
            {
                blogPost.TotalReaction = totalReactions;
                await _context.SaveChangesAsync();
            }
        }

        // Method to update the total popularity of the blog post
        private async Task UpdateTotalPopularity(int blogPostId)
        {
            var blogPost = await _context.BlogPosts
                .Include(bp => bp.Comments)
                .FirstOrDefaultAsync(bp => bp.Id == blogPostId);

            if (blogPost != null)
            {
                int totalUpvotes = await _context.BlogReactions
                    .Where(br => br.BlogPostId == blogPostId && br.Reaction == "upvote")
                    .CountAsync();

                int totalDownvotes = await _context.BlogReactions
                    .Where(br => br.BlogPostId == blogPostId && br.Reaction == "downvote")
                    .CountAsync();

                int totalComments = blogPost.Comments?.Count ?? 0;

                // Calculate popularity based on the provided formula
                int popularity = (2 * totalUpvotes) + (-1 * totalDownvotes) + (1 * totalComments);

                // Update the TotalPopularity property of the blog post
                blogPost.TotalPopularity = popularity;

                await _context.SaveChangesAsync();
            }
        }


        public async Task<int> GetTotalUpvotesAsync(int blogPostId)
        {
            return await _context.BlogReactions
                .Where(br => br.BlogPostId == blogPostId && br.Reaction == "upvote")
                .CountAsync();
        }

        public async Task<int> GetTotalDownvotesAsync(int blogPostId)
        {
            return await _context.BlogReactions
                .Where(br => br.BlogPostId == blogPostId && br.Reaction == "downvote")
                .CountAsync();
        }
        public async Task<IEnumerable<BlogReaction>> GetUserVotesForPostAsync(int blogPostId, string userName)
        {
            // Retrieve user votes for the specified blog post and user from the database
            var userVotes = await _context.BlogReactions
                .Where(br => br.BlogPostId == blogPostId && br.UserName == userName)
                .ToListAsync();

            return userVotes;
        }

    }

}
