using Microsoft.AspNetCore.Mvc;
using TourManagement.Business.Services;
using TourManagement.Data.Repositories;
using System.Threading.Tasks;

namespace TourManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingService _bookingService;

        public PaymentsController(IVnPayService vnPayService, IBookingRepository bookingRepository, IBookingService bookingService)
        {
            _vnPayService = vnPayService;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
        }

        [HttpPost("create-url")]
        public async Task<IActionResult> CreatePaymentUrl([FromQuery] string bookingCode, [FromQuery] string returnUrl)
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var booking = bookings.FirstOrDefault(b => b.BookingCode == bookingCode);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            var url = _vnPayService.CreatePaymentUrl(booking, HttpContext, returnUrl);
            return Ok(new { url });
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> PaymentCallback()
        {
            var collections = Request.Query;
            var isValid = _vnPayService.PaymentExecute(collections);

            var bookingCodeStr = collections["vnp_TxnRef"].ToString();
            var bookingCode = bookingCodeStr.Split('_')[0]; // Extract the actual booking code

            if (isValid)
            {
                // Payment success, update DB
                var bookings = await _bookingRepository.GetAllAsync();
                var booking = bookings.FirstOrDefault(b => b.BookingCode == bookingCode);
                
                if (booking != null && booking.Status != "paid")
                {
                    booking.Status = "paid";
                    booking.PaymentMethod = "vnpay";
                    await _bookingRepository.UpdateAsync(booking);
                }
                return Redirect($"{collections["vnp_ReturnUrl"]}?status=success&bookingCode={bookingCode}");
            }
            
            return Redirect($"{collections["vnp_ReturnUrl"]}?status=failed&bookingCode={bookingCode}");
        }
    }
}
