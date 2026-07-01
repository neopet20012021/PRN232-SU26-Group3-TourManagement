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
    [Route("odata/Tours")]
    public class ToursController : ODataController
    {
        private readonly TourManagementDbContext _context;

        public ToursController(TourManagementDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(_context.Tours.AsQueryable());
        }

        [EnableQuery]
        [HttpGet("{key}")]
        [AllowAnonymous]
        public IActionResult Get(int key)
        {
            var tour = _context.Tours.Where(t => t.TourId == key);
            if (!tour.Any())
            {
                return NotFound();
            }
            return Ok(SingleResult.Create(tour));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] Tour tour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (tour.Nights > tour.Days || tour.Nights < tour.Days - 1)
            {
                return BadRequest("Số đêm không hợp lệ so với số ngày.");
            }

            if (await _context.Tours.AnyAsync(t => t.TourCode == tour.TourCode))
            {
                return BadRequest("Mã tour đã tồn tại.");
            }

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            return Created(tour);
        }

        [HttpPut("{key}")]
        [AllowAnonymous]
        public async Task<IActionResult> Put(int key, [FromBody] Tour update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.TourId)
            {
                return BadRequest();
            }

            if (update.Nights > update.Days || update.Nights < update.Days - 1)
            {
                return BadRequest("Số đêm không hợp lệ so với số ngày.");
            }

            if (await _context.Tours.AnyAsync(t => t.TourCode == update.TourCode && t.TourId != key))
            {
                return BadRequest("Mã tour đã tồn tại.");
            }

            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Updated(update);
        }

        [HttpDelete("{key}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int key)
        {
            var tour = await _context.Tours.FindAsync(key);
            if (tour == null)
            {
                return NotFound();
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
