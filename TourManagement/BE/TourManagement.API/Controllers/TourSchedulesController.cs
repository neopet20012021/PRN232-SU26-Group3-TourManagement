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
    [Route("odata/TourSchedules")]
    public class TourSchedulesController : ODataController
    {
        private readonly TourManagementDbContext _context;

        public TourSchedulesController(TourManagementDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(_context.TourSchedules.Include(s => s.Tour).AsQueryable());
        }

        [EnableQuery]
        [HttpGet("{key}")]
        [AllowAnonymous]
        public IActionResult Get(int key)
        {
            var schedule = _context.TourSchedules.Include(s => s.Tour).Where(t => t.ScheduleId == key);
            if (!schedule.Any())
            {
                return NotFound();
            }
            return Ok(SingleResult.Create(schedule));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Post([FromBody] TourSchedule schedule)
        {
            // Ignore Navigation properties from validation
            ModelState.Remove("Tour");
            ModelState.Remove("Bookings");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tour = await _context.Tours.FindAsync(schedule.TourId);
            if (tour == null)
            {
                return BadRequest("Tour không tồn tại.");
            }

            // Tự động tính EndDate dựa trên số ngày của Tour
            // Nếu Tour có 3 ngày 2 đêm, và StartDate là ngày 1, thì EndDate là ngày 1 + (3 - 1) = ngày 3
            if (tour.Days > 0)
            {
                schedule.EndDate = schedule.StartDate.AddDays(tour.Days - 1);
            }
            else
            {
                schedule.EndDate = schedule.StartDate;
            }
            
            // Set default values if not provided
            if (schedule.AvailableSeats == 0)
            {
                schedule.AvailableSeats = schedule.MaxParticipants;
            }

            schedule.CreatedDate = DateTime.Now;

            _context.TourSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return Created(schedule);
        }

        [HttpPut("{key}")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Put(int key, [FromBody] TourSchedule update)
        {
            ModelState.Remove("Tour");
            ModelState.Remove("Bookings");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.ScheduleId)
            {
                return BadRequest();
            }

            // Optional: recalculate EndDate if StartDate changes, etc.

            _context.Entry(update).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TourSchedules.Any(e => e.ScheduleId == key))
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
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> Delete(int key)
        {
            var schedule = await _context.TourSchedules.FindAsync(key);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.TourSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
