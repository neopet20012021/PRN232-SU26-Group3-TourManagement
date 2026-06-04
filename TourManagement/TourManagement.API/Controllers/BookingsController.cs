using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data.Context;
using TourManagement.Data.Models;

namespace TourManagement.API.Controllers
{
    [Route("odata/[controller]")]
    public class BookingsController : ODataController
    {
        private readonly TourManagementDbContext _context;

        public BookingsController(TourManagementDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Get()
        {
            return Ok(_context.Bookings.AsQueryable());
        }

        [EnableQuery]
        [HttpGet("{key}")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Get(int key)
        {
            var booking = _context.Bookings.Where(b => b.BookingId == key);
            if (!booking.Any())
            {
                return NotFound();
            }
            return Ok(SingleResult.Create(booking));
        }

        [HttpPost]
        [AllowAnonymous] // Allow customers to book tours
        public async Task<IActionResult> Post([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Simple code generation
            booking.BookingCode = "BK" + DateTime.Now.ToString("yyyyMMddHHmmss");
            booking.CreatedDate = DateTime.Now;
            booking.Status = "pending";

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Created(booking);
        }

        [HttpPut("{key}")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Put(int key, [FromBody] Booking update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(update).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(e => e.BookingId == key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(update);
        }

        [HttpDelete("{key}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int key)
        {
            var booking = await _context.Bookings.FindAsync(key);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("calculate-price")]
        [AllowAnonymous]
        public async Task<IActionResult> CalculatePrice([FromBody] PriceCalculationRequest request, [FromServices] TourManagement.Business.Services.IBookingService bookingService)
        {
            try
            {
                var result = await bookingService.CalculatePriceAsync(
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
        [AllowAnonymous]
        public async Task<IActionResult> ValidatePromoCode([FromBody] string promoCode, [FromServices] TourManagement.Business.Services.IBookingService bookingService)
        {
            try
            {
                var isValid = await bookingService.ValidatePromoCodeAsync(promoCode);
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
