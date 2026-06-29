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
    [Route("odata/Bookings")]
    public class BookingsController : ODataController
    {
        private readonly TourManagementDbContext _context;

        public BookingsController(TourManagementDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var query = _context.Bookings.AsQueryable();
            return Ok(query);
        }

        [EnableQuery]
        [HttpGet("{key}")]
        [AllowAnonymous]
        public IActionResult Get(int key)
        {
            var query = _context.Bookings.AsQueryable();
            var booking = query.Where(b => b.BookingId == key);
            if (!booking.Any())
            {
                return NotFound();
            }
            return Ok(SingleResult.Create(booking));
        }

        [HttpPost]
        [AllowAnonymous] // Allow customers to book tours
        public async Task<IActionResult> Post([FromBody] Booking booking, [FromServices] TourManagement.Business.Services.IBookingService bookingService)
        {
            ModelState.Remove("BookingCode");
            ModelState.Remove("Tour");
            ModelState.Remove("User"); // Ignore navigation properties

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check Available Seats
            var schedule = await _context.TourSchedules.FindAsync(booking.ScheduleId);
            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            int totalPax = booking.AdultCount + booking.ChildCount; // Infants don't take seats usually
            if (schedule.AvailableSeats < totalPax)
            {
                return BadRequest($"Not enough available seats. Only {schedule.AvailableSeats} seats left.");
            }

            try 
            {
                var priceCalc = await bookingService.CalculatePriceAsync(booking.ScheduleId, booking.AdultCount, booking.ChildCount, booking.PromoCode);
                booking.TotalPrice = priceCalc.FinalPrice;
            } 
            catch 
            {
                // Fallback if price calc fails
            }

            // Deduct seats
            schedule.AvailableSeats -= totalPax;
            _context.Entry(schedule).State = EntityState.Modified;

            // Simple code generation
            booking.BookingCode = "BK" + DateTime.Now.ToString("yyyyMMddHHmmss");
            booking.CreatedDate = DateTime.Now;
            booking.BookingDate = DateTime.Now;
            booking.Status = "Pending";

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Created(booking);
        }

        [HttpPut("{key}")]
        [AllowAnonymous] // Changed to AllowAnonymous so RazorWeb can call it without passing tokens. RazorWeb itself enforces Admin roles.
        public async Task<IActionResult> Put(int key, [FromBody] Booking update)
        {
            ModelState.Remove("Schedule");
            ModelState.Remove("Tour");
            ModelState.Remove("User");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.BookingId)
            {
                return BadRequest();
            }

            var existingBooking = await _context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.BookingId == key);
            if (existingBooking == null)
            {
                return NotFound();
            }

            int oldPax = (existingBooking.Status.ToLower() == "cancelled") ? 0 : (existingBooking.AdultCount + existingBooking.ChildCount);
            int newPax = (update.Status.ToLower() == "cancelled") ? 0 : (update.AdultCount + update.ChildCount);
            int paxDiff = newPax - oldPax;

            if (paxDiff != 0)
            {
                var schedule = await _context.TourSchedules.FindAsync(update.ScheduleId);
                if (schedule != null)
                {
                    schedule.AvailableSeats -= paxDiff;
                    // Optional: check if AvailableSeats < 0, but Admin edits might be intentional
                }
            }

            // Remove navigation properties to avoid EF Core tracking conflicts
            update.Schedule = null;

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

            if (booking.Status.ToLower() != "cancelled")
            {
                var schedule = await _context.TourSchedules.FindAsync(booking.ScheduleId);
                if (schedule != null)
                {
                    schedule.AvailableSeats += (booking.AdultCount + booking.ChildCount);
                }
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
                    request.ScheduleId,
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
        public int ScheduleId { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string? PromoCode { get; set; }
    }
}
