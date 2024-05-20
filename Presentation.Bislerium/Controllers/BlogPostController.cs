using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly UserManager<ApplicationUser> _userManager;


        public BlogPostController(IBlogService blogService, UserManager<ApplicationUser> userManager)
        {
            _blogService = blogService;
            _userManager = userManager;
        }

        //anonymous user can also see blog post 
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            var blogPosts = await _blogService.GetAllBlogPostsAsync();
            return Ok(blogPosts);
        }

//fetch blogpost  by  username 
        [HttpGet("blogByUserName")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username cannot be empty");
            }

            var blogPosts = await _blogService.GetAllBlogPostsByUsernameAsync(username);
            return Ok(blogPosts);
        }

        //making search work 
        [HttpGet("AllSearch")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBlogs(
            [FromQuery] string sortBy = "recency",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchQuery = "")
        {
            try
            {
                var blogs = await _blogService.GetFilteredAndSortedBlogPostsAsync(sortBy, page, pageSize, searchQuery);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //fetch blog by doing shorting 
        [HttpGet("All")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBlogs([FromQuery] string sortBy = "popularity", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var blogs = await _blogService.GetAllBlogPostsAsync(sortBy, page, pageSize);
            return Ok(blogs);
        }

        //fetch popular blog 

        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularBlogsAsync(int page = 1, int pageSize = 10)
        {
            var popularBlogs = await _blogService.GetPopularBlogsAsync(page, pageSize);
            return Ok(popularBlogs);
        }
        //fetch recent blog 


        [HttpGet("recent")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentBlogsAsync(int page = 1, int pageSize = 10)
        {
            var recentBlogs = await _blogService.GetRecentBlogsAsync(page, pageSize);
            return Ok(recentBlogs);
        }


        //below is for creating blog post  

        [HttpPost("createBlogPost")]
        public async Task<IActionResult> CreateBlogPost([FromForm] BlogPostCreatedDto blogPostDto, IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _blogService.CreateBlogPostAsync(blogPostDto, userId, file);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        //for updating blog post 

        [HttpPut("{id}")]
        public async Task<IActionResult> EditBlogPost(int id, [FromForm] BlogPostUpdateDto blogPostDto, IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _blogService.EditBlogPostAsync(id, blogPostDto, userId, file);
            if (result == null)
            {
                return Forbid();
            }
            return Ok(result);
        }

        //for delting blogpost 


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSuccess = await _blogService.DeleteBlogPostAsync(id, userId);
            if (!isSuccess)
            {
                return NotFound();
            }
            return Ok();
        }

        //getting blog post history
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetBlogPostHistory(int id)
        {
            var history = await _blogService.GetBlogPostHistoryAsync(id);
            return Ok(history);
        }
        //get blogpost by blog id 

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogPostById(int id)
        {
            var blogPost = await _blogService.GetBlogPostByIdAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            return Ok(blogPost);
        }

        //for getting popularblogs
        [HttpGet("totalpopularblogs")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTotalPopularBlogsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var blogs = await _blogService.GetTotalPopularBlogsAsync(page, pageSize);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //10 most populalr blogs post and popular blogger has same logic difference is i will show only blogger name when calling that below code

        [HttpGet("top10MostPopularBlogs")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTop10MostPopularBlogsAsync()
        {
            try
            {
                var blogs = await _blogService.GetTop10MostPopularBlogsAsync();
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
//i  need to show popular bloggername only from here

        [HttpGet("top10MostPopularBloggers")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTop10MostPopularBloggersAsync()
        {
            try
            {
                var blogs = await _blogService.GetTop10MostPopularBlogsAsync();
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //for getting top10 blog by moth and year

        [HttpGet("top10bymonthyear/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTop10MostPopularBlogsByMonthAndYearAsync(int year, int month)
        {
            try
            {
                var blogs = await _blogService.GetTop10MostPopularBlogsByMonthAndYearAsync(year, month);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //for getting top10 blogger by moth and year
        [HttpGet("top10Bloggersbymonthyear/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTop10MostPopularBloggersBYMoth(int year, int month)
        {
            try
            {
                var blogs = await _blogService.GetTop10MostPopularBlogsByMonthAndYearAsync(year, month);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //for adding pagination


        [HttpGet("paginate")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetPaginatedAndSortableBlogsAsync(int page = 1, int pageSize = 10, string sortBy = "recency")
        {
            try
            {
                var blogs = await _blogService.GetPaginatedAndSortableBlogsAsync(page, pageSize, sortBy);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
