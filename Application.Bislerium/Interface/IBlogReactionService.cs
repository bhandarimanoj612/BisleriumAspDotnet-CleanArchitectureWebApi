using Domain.Bislerium.Models;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface IBlogReactionService
    {
        //for adding blog reactions 
        Task<BlogReaction> AddReactionAsync(int blogPostId, string userName, ReactionCategory reactionCategory);

        //for getting total upvotes
        Task<int> GetTotalUpvotesAsync(int blogPostId);
        //for geting total downvtes total 
        Task<int> GetTotalDownvotesAsync(int blogPostId);
        //get user vote by username 

        Task<IEnumerable<BlogReaction>> GetUserVotesForPostAsync(int blogPostId, string userName);
    }
}
