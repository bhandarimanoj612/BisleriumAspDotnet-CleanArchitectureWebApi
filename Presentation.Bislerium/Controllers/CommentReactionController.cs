using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentReactionController : ControllerBase
    {
        private readonly ICommentReactionService _commentReactionService;

        public CommentReactionController(ICommentReactionService commentReactionService)
        {
            _commentReactionService = commentReactionService;
        }

        //comment -->reaction upvote and downvote
        [HttpPost("reaction")]
        public async Task<IActionResult> AddReaction([FromQuery] int commentId, [FromQuery] string userName, [FromQuery] ReactionCategory reactionCategory)
        {
            try
            {
                var reaction = await _commentReactionService.AddReactionAsync(commentId, userName, reactionCategory);
                return Ok(reaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //getting reactions count of upvote and downvote

        [HttpGet("{commentId}/reactions")]
        public async Task<IActionResult> GetCommentReactions(int commentId)
        {
            var totalUpvotes = await _commentReactionService.GetTotalUpvotesAsync(commentId);
            var totalDownvotes = await _commentReactionService.GetTotalDownvotesAsync(commentId);

            var response = new
            {
                TotalUpvotes = totalUpvotes,
                TotalDownvotes = totalDownvotes
            };

            return Ok(response);
        }
    }
}
