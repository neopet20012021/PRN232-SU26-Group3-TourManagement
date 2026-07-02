using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Business.DTOs;
using TourManagement.Business.Services;
using TourManagement.Web.Models;

namespace TourManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBookingService _bookingService;

        public AdminController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ─── Dashboard / Booking List ─────────────────────────────────────────
        public async Task<IActionResult> Index(string? keyword = null, string? status = null)
        {
            var allBookings = (await _bookingService.GetAllBookingsAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lk = keyword.ToLower();
                allBookings = allBookings.Where(b =>
                    b.BookingCode.ToLower().Contains(lk) ||
                    b.CustomerName.ToLower().Contains(lk) ||
                    b.Email.ToLower().Contains(lk) ||
                    b.PhoneNumber.Contains(lk) ||
                    b.TourName.ToLower().Contains(lk)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
                allBookings = allBookings.Where(b => b.Status == status).ToList();

            var vm = new AdminBookingListViewModel
            {
                Bookings = allBookings,
                SearchKeyword = keyword,
                StatusFilter = status,
                TotalCount = allBookings.Count
            };

            ViewBag.Tours = (await _bookingService.GetActiveToursAsync()).ToList();
            return View(vm);
        }

        // ─── GET: Admin/GetBooking/{id} (for Edit modal) ───────────────────
        [HttpGet]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Json(booking);
        }

        // ─── POST: Admin/CreateBooking ────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(CreateBookingDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join("; ", errors) });
            }

            var result = await _bookingService.CreateBookingAsync(dto);
            return Json(new { success = result.Success, message = result.Message, data = result.Data });
        }

        // ─── POST: Admin/UpdateBooking ────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBooking(int bookingId, UpdateBookingDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join("; ", errors) });
            }

            var result = await _bookingService.UpdateBookingAsync(bookingId, dto);
            return Json(new { success = result, message = result ? "Cập nhật thành công!" : "Cập nhật thất bại." });
        }

        // ─── POST: Admin/DeleteBooking ────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);
            return Json(new { success = result, message = result ? "Xóa thành công!" : "Không thể xóa booking này." });
        }

        // ─── POST: Admin/UpdateStatus ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var result = await _bookingService.UpdateBookingStatusAsync(id, status);
            return Json(new { success = result });
        }
    }
}
