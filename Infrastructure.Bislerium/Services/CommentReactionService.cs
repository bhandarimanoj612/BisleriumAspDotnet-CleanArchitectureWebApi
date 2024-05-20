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
    public class CommentReactionService : ICommentReactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHubs, INotificationClient> _hubContext;

        public CommentReactionService(ApplicationDbContext context, IHubContext<NotificationHubs, INotificationClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

      
        public async Task<CommentReaction> AddReactionAsync(int commentId, string userName, ReactionCategory reactionCategory)
        {
            // Find the existing reaction of the user for the specified comment
            var existingReaction = await _context.CommentReactions.FirstOrDefaultAsync(cr => cr.CommentId == commentId && cr.UserName == userName);

            // If an existing reaction is found
            if (existingReaction != null)
            {
                // If the existing reaction is the same as the new reaction, remove it
                if (existingReaction.Reaction == reactionCategory.Content)
                {
                    _context.CommentReactions.Remove(existingReaction);
                    await _context.SaveChangesAsync();

                   await UpdateTotalReactions(commentId);
                    return null;
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

                // Update total reactions of the comment
                await UpdateTotalReactions(commentId);

                return existingReaction; // Return the existing reaction with the toggled values
            }

            // Create a new reaction if no existing reaction found
            var newReaction = new CommentReaction
            {
                CommentId = commentId,
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

            _context.CommentReactions.Add(newReaction);
            await _context.SaveChangesAsync();
            // Update total reactions of the comment
            await UpdateTotalReactions(commentId);

            //// Send notification to the author of the comment
            

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                if (reactionCategory.Content == "upvote")
                {
                    await _hubContext.Clients.All.ReceiveNotification($"User {userName} upvoted on comment.");
                }
                else if (reactionCategory.Content == "downvote")
                {
                    await _hubContext.Clients.All.ReceiveNotification($"User {userName} downvoted on comment.");
                }
            }

          await UpdateTotalReactions(commentId); // Update total reactions

            return newReaction;
        }

        // Method to update the total reactions of the comment
        private async Task UpdateTotalReactions(int commentId)
        {
            var totalUpvotes = await _context.CommentReactions
                .Where(cr => cr.CommentId == commentId && cr.Reaction == "upvote")
                .SumAsync(cr => cr.TotalUpVotes);

            var totalDownvotes = await _context.CommentReactions
                .Where(cr => cr.CommentId == commentId && cr.Reaction == "downvote")
                .SumAsync(cr => cr.TotalDownVotes);

            var totalReactions = totalUpvotes - totalDownvotes;

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                comment.TotalReaction = totalReactions;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalUpvotesAsync(int commentId)
        {
            return await _context.CommentReactions
                .Where(cr => cr.CommentId == commentId && cr.Reaction == "upvote")
                .CountAsync();
                //.SumAsync(cr => cr.TotalUpVotes);
        }

        public async Task<int> GetTotalDownvotesAsync(int commentId)
        {
            return await _context.CommentReactions
                .Where(cr => cr.CommentId == commentId && cr.Reaction == "downvote")
                .CountAsync();
                //.SumAsync(cr => cr.TotalDownVotes);
        }
    }
}
