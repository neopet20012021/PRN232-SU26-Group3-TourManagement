using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Data.Context;

namespace TourManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly TourManagementDbContext _context;

        public AnalyticsController(TourManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet("DashboardStats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalTours = await _context.Tours.CountAsync(t => t.IsActive);
            
            // Only count active schedules that haven't ended yet as active schedules
            var totalSchedules = await _context.TourSchedules.CountAsync(s => s.Status == "Active");

            var totalUsers = await _context.Users.CountAsync();

            var allBookings = await _context.Bookings.ToListAsync();
            
            var totalBookings = allBookings.Count;
            
            // Revenue is calculated from Paid and Confirmed bookings
            var totalRevenue = allBookings
                .Where(b => b.Status.ToLower() == "paid" || b.Status.ToLower() == "confirmed")
                .Sum(b => b.TotalPrice);

            var pendingBookings = allBookings.Count(b => b.Status.ToLower() == "pending");

            return Ok(new
            {
                TotalTours = totalTours,
                TotalSchedules = totalSchedules,
                TotalUsers = totalUsers,
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings,
                TotalRevenue = totalRevenue
            });
        }
    }
}
