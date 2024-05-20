using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostHistoryController : ControllerBase
    {
        private readonly IBlogPostHistoryService _blogPostHistoryService;

        public BlogPostHistoryController(IBlogPostHistoryService blogPostHistoryService)
        {
            _blogPostHistoryService = blogPostHistoryService;
        }

        //below code is for showing blog post history
        [HttpGet("{userName}")]
        public async Task<ActionResult<List<BlogPostHistory>>> Get(string userName)
        {
            var histories = await _blogPostHistoryService.GetHistoriesByUserNameAsync(userName);
            if (histories == null || histories.Count == 0)
            {
                return NotFound();
            }
            return Ok(histories);
        }

    }
}
