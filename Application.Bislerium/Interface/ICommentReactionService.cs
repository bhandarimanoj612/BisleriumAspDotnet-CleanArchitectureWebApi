using Domain.Bislerium.Models;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface ICommentReactionService
    {
        //for comment reactions
        Task<CommentReaction> AddReactionAsync(int commentId, string userName, ReactionCategory reactionCategory);

        Task<int> GetTotalUpvotesAsync(int commentId);

        Task<int> GetTotalDownvotesAsync(int commentId);
    }
}
