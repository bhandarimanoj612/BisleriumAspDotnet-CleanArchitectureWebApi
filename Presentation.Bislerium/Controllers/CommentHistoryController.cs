using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentHistoryController : ControllerBase
    {

        private readonly ICommentHistoryService _commentHistoryService;

        public CommentHistoryController(ICommentHistoryService commentHistoryService)
        {
            _commentHistoryService = commentHistoryService;
        }
        //below code is for showing comment history

        [HttpGet("{userName}")]
        public async Task<ActionResult<List<CommentHistory>>> Get(string userName)
        {
            var histories = await _commentHistoryService.GetHistoriesByUserNameAsync(userName);
            if (histories == null || histories.Count == 0)
            {
                return NotFound();
            }
            return Ok(histories);
        }
    }
}
