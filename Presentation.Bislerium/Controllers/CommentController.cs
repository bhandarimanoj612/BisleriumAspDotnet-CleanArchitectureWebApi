// CommentController.cs
using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllComments()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsForPost(int postId)
        {
            var comments = await _commentService.GetCommentsForPostAsync(postId);
            return Ok(comments);
        }

        [HttpPost("addComments/{postId}")]
        //[Authorize]
        public async Task<IActionResult> AddComment(int postId, [FromBody] CommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (commentDto == null)
            {
                return BadRequest("Invalid comment data");
            }

            var result = await _commentService.AddCommentAsync(postId, userId, commentDto);

            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            return Ok(result);
        }


        [HttpPut("{blogId}/comments/{commentId}")]
        //[Authorize] // Requires authentication
        public async Task<IActionResult> EditComment(int blogId, int commentId, [FromBody] CommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _commentService.EditCommentAsync(commentId, blogId, userId, commentDto);
            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, result.Message);
            }
            return Ok(result);
        }


        [HttpDelete("{commentId}")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSuccess = await _commentService.DeleteCommentAsync(commentId, userId);
            if (!isSuccess)
            {
                return NotFound();
            }
            return Ok(new { message = "Comment deleted successfully" });
        }
        //for getting comment history


        [HttpGet("comments/{id}/history")]
        public async Task<IActionResult> GetCommentHistory(int id)
        {
            var history = await _commentService.GetCommentHistoryAsync(id);
            return Ok(history);
        }   

    }
}
