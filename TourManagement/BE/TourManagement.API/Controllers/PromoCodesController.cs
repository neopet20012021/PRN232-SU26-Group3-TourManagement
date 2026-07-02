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
    [Route("odata/PromoCodes")]
    public class PromoCodesController : ODataController
    {
        private readonly TourManagementDbContext _context;

        public PromoCodesController(TourManagementDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(_context.PromoCodes.AsQueryable());
        }

        [EnableQuery]
        [HttpGet("{key}")]
        [AllowAnonymous]
        public IActionResult Get(int key)
        {
            var code = _context.PromoCodes.Where(p => p.PromoCodeId == key);
            if (!code.Any())
            {
                return NotFound();
            }
            return Ok(SingleResult.Create(code));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] PromoCode promoCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exists = await _context.PromoCodes.AnyAsync(p => p.Code.ToUpper() == promoCode.Code.ToUpper());
            if (exists)
            {
                return BadRequest("Mã khuyến mãi này đã tồn tại.");
            }

            _context.PromoCodes.Add(promoCode);
            await _context.SaveChangesAsync();

            return Created(promoCode);
        }

        [HttpPut("{key}")]
        [AllowAnonymous]
        public async Task<IActionResult> Put(int key, [FromBody] PromoCode update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.PromoCodeId)
            {
                return BadRequest();
            }

            var exists = await _context.PromoCodes.AnyAsync(p => p.Code.ToUpper() == update.Code.ToUpper() && p.PromoCodeId != key);
            if (exists)
            {
                return BadRequest("Mã khuyến mãi này đã tồn tại.");
            }

            _context.Entry(update).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PromoCodes.Any(e => e.PromoCodeId == key))
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
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int key)
        {
            var promoCode = await _context.PromoCodes.FindAsync(key);
            if (promoCode == null)
            {
                return NotFound();
            }

            _context.PromoCodes.Remove(promoCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
