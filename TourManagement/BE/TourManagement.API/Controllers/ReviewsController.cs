using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TourManagement.Business.DTOs;
using TourManagement.Business.Services;
using TourManagement.Data.Context;

namespace TourManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ODataController
    {
        private readonly IReviewService _reviewService;
        private readonly TourManagementDbContext _context;

        public ReviewsController(IReviewService reviewService, TourManagementDbContext context)
        {
            _reviewService = reviewService;
            _context = context;
        }

        [HttpGet("tour/{tourId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTour(int tourId)
        {
            var reviews = await _reviewService.GetReviewsByTourIdAsync(tourId);
            return Ok(reviews);
        }

        [HttpGet("tour/{tourId}/summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSummaryByTour(int tourId)
        {
            var summary = await _reviewService.GetReviewSummaryAsync(tourId);
            return Ok(summary);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _reviewService.AddReviewAsync(dto);
                return CreatedAtAction(nameof(GetByTour), new { tourId = dto.TourId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [EnableQuery]
        [HttpGet("/odata/Reviews")]
        [AllowAnonymous]
        public IActionResult GetOData()
        {
            return Ok(_context.Reviews.AsQueryable());
        }
    }
}
