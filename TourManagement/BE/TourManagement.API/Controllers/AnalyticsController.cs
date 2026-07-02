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
            
            // Only count active schedules
            var totalSchedules = await _context.TourSchedules.CountAsync(s => s.Status == "Active");

            var totalUsers = await _context.Users.CountAsync();

            var allBookings = await _context.Bookings.ToListAsync();
            
            var totalBookings = allBookings.Count;
            
            // Revenue is calculated from Paid and Confirmed bookings
            var totalRevenue = allBookings
                .Where(b => b.Status.ToLower() == "paid" || b.Status.ToLower() == "confirmed")
                .Sum(b => b.TotalPrice);

            var pendingBookings = allBookings.Count(b => b.Status.ToLower() == "pending");

            // Real monthly revenue for last 6 months
            var today = System.DateTime.Today;
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => today.AddMonths(-i))
                .Select(d => new { Year = d.Year, Month = d.Month })
                .Reverse()
                .ToList();

            var monthlyRevenueList = last6Months.Select(m => {
                var revenue = allBookings
                    .Where(b => (b.Status.ToLower() == "paid" || b.Status.ToLower() == "confirmed") 
                                && b.BookingDate.Year == m.Year 
                                && b.BookingDate.Month == m.Month)
                    .Sum(b => b.TotalPrice);
                return new {
                    MonthName = $"{m.Month}/{m.Year}",
                    Revenue = revenue
                };
            }).ToList();

            // Real booking status breakdown
            var statusCounts = allBookings
                .GroupBy(b => b.Status)
                .Select(g => new {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return Ok(new
            {
                TotalTours = totalTours,
                TotalSchedules = totalSchedules,
                TotalUsers = totalUsers,
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenueList,
                StatusCounts = statusCounts
            });
        }
    }
}
