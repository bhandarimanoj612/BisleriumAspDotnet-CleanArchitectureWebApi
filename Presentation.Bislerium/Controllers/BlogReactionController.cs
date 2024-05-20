using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogReactionController : ControllerBase
    {
        private readonly IBlogReactionService _blogReactionService;

        public BlogReactionController(IBlogReactionService blogReactionService)
        {
            _blogReactionService = blogReactionService;
        }

        //for adding reation in blog post 
        [HttpPost("reaction")]
        public async Task<IActionResult> AddReaction([FromQuery] int blogPostId, [FromQuery] string userName, [FromQuery] ReactionCategory reactionCategory)
        {
            try
            {
                var reaction = await _blogReactionService.AddReactionAsync(blogPostId, userName, reactionCategory);
                return Ok(reaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //for counting total votes

        [HttpGet("total-votes")]
        public async Task<IActionResult> GetTotalVotes(int blogPostId)
        {
            try
            {
                var totalUpvotes = await _blogReactionService.GetTotalUpvotesAsync(blogPostId);
                var totalDownvotes = await _blogReactionService.GetTotalDownvotesAsync(blogPostId);

                var totalVotes = new
                {
                    TotalUpvotes = totalUpvotes,
                    TotalDownvotes = totalDownvotes
                };

                return Ok(totalVotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //for getting user-votes
        [HttpGet("{blogPostId}/user-votes")]
        public async Task<IActionResult> GetUserVotes(int blogPostId, [FromQuery] string userName)
        {
            try
            {
                // Call the service method to fetch user votes for the specified blog post
                var userVotes = await _blogReactionService.GetUserVotesForPostAsync(blogPostId, userName);
                return Ok(userVotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
