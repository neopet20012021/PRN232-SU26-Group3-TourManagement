using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Business.DTOs;
using TourManagement.Business.Services;

namespace TourManagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsApiController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsApiController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("tours")]
        public async Task<IActionResult> GetActiveTours()
        {
            try
            {
                var tours = await _bookingService.GetActiveToursAsync();
                return Ok(new { success = true, data = tours });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("calculate-price")]
        public async Task<IActionResult> CalculatePrice([FromBody] PriceCalculationRequest request)
        {
            try
            {
                var result = await _bookingService.CalculatePriceAsync(
                    request.TourId,
                    request.AdultCount,
                    request.ChildCount,
                    request.PromoCode
                );
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("validate-promo")]
        public async Task<IActionResult> ValidatePromoCode([FromBody] string promoCode)
        {
            try
            {
                var isValid = await _bookingService.ValidatePromoCodeAsync(promoCode);
                return Ok(new { success = true, isValid = isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class PriceCalculationRequest
    {
        public int TourId { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string? PromoCode { get; set; }
    }
}