// DashboardController.cs
using Application.Bislerium.Interface;
using Domain.Bislerium.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        //for showing  count of all time for user 
        [HttpGet("counts/alltime")]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public async Task<IActionResult> GetCountsAllTime()
        {
            var totalCounts = new
            {
                TotalBlogPosts = await _dashboardService.GetTotalBlogPostsCount(),
                TotalUpvotes = await _dashboardService.GetTotalUpvotesCount(),
                TotalDownvotes = await _dashboardService.GetTotalDownvotesCount(),
                TotalComments = await _dashboardService.GetTotalCommentsCount()
            };
            return Ok(totalCounts);
        }

        //for showing count of dashboard 
        [HttpGet("counts/{year}/{month}")]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public async Task<IActionResult> GetCountsByMonth(int year, int month)
        {
            var monthlyCounts = new
            {
                TotalBlogPosts = await _dashboardService.GetTotalBlogPostsCountByMonth(year, month),
                TotalUpvotes = await _dashboardService.GetTotalUpvotesCountByMonth(year, month),
                TotalDownvotes = await _dashboardService.GetTotalDownvotesCountByMonth(year, month),
                TotalComments = await _dashboardService.GetTotalCommentsCountByMonth(year, month)
            };
            return Ok(monthlyCounts);
        }
    }
}
