using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Business.DTOs;
using TourManagement.Business.Services;
using TourManagement.Web.Models;

namespace TourManagement.Web.Controllers
{
    [AllowAnonymous]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? tourId)
        {
            var tours = await _bookingService.GetActiveToursAsync();
            if (tourId.HasValue)
            {
                var selectedTour = tours.FirstOrDefault(t => t.Id == tourId.Value);
                if (selectedTour != null)
                {
                    ViewBag.SelectedTourId = tourId.Value;
                }
            }
            return View(tours);
        }

        [HttpPost]
        public IActionResult NextStep(int tourId)
        {
            HttpContext.Session.SetInt32("Booking_TourId", tourId);
            return RedirectToAction("BookingDetails");
        }

        [HttpGet]
        public async Task<IActionResult> BookingDetails()
        {
            var tourId = HttpContext.Session.GetInt32("Booking_TourId");
            if (!tourId.HasValue) return RedirectToAction("Create");

            var tours = await _bookingService.GetActiveToursAsync();
            var tour = tours.FirstOrDefault(t => t.Id == tourId.Value);
            if (tour == null) return RedirectToAction("Create");

            var model = new BookingDetailsViewModel
            {
                TourId = tour.Id,
                TourName = tour.TourName,
                DepartureDate = tour.DepartureDate,
                PricePerAdult = tour.PricePerAdult,
                PricePerChild = tour.PricePerChild
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult BookingDetails(BookingDetailsViewModel model)
        {
            // Do not require PromoCode and SpecialRequest
            ModelState.Remove("PromoCode");
            ModelState.Remove("SpecialRequest");

            if (ModelState.IsValid)
            {
                // Store in session (in a real app, serialize to JSON)
                HttpContext.Session.SetString("Booking_CustomerName", model.CustomerName);
                HttpContext.Session.SetString("Booking_Email", model.Email);
                HttpContext.Session.SetString("Booking_PhoneNumber", model.PhoneNumber);
                HttpContext.Session.SetInt32("Booking_AdultCount", model.AdultCount);
                HttpContext.Session.SetInt32("Booking_ChildCount", model.ChildCount);
                HttpContext.Session.SetString("Booking_PromoCode", model.PromoCode ?? "");
                HttpContext.Session.SetString("Booking_SpecialRequest", model.SpecialRequest ?? "");
                
                return RedirectToAction("Review");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Review()
        {
            var tourId = HttpContext.Session.GetInt32("Booking_TourId");
            if (!tourId.HasValue) return RedirectToAction("Create");

            var tours = await _bookingService.GetActiveToursAsync();
            var tour = tours.FirstOrDefault(t => t.Id == tourId.Value);
            
            var adultCount = HttpContext.Session.GetInt32("Booking_AdultCount") ?? 1;
            var childCount = HttpContext.Session.GetInt32("Booking_ChildCount") ?? 0;
            var promoCode = HttpContext.Session.GetString("Booking_PromoCode");
            var customerName = HttpContext.Session.GetString("Booking_CustomerName");
            var email = HttpContext.Session.GetString("Booking_Email");
            var phone = HttpContext.Session.GetString("Booking_PhoneNumber");

            var priceCalc = await _bookingService.CalculatePriceAsync(tourId.Value, adultCount, childCount, promoCode);

            var model = new BookingReviewViewModel
            {
                TourName = tour?.TourName,
                DepartureDate = tour?.DepartureDate ?? DateTime.Now,
                CustomerName = customerName,
                Email = email,
                PhoneNumber = phone,
                AdultCount = adultCount,
                ChildCount = childCount,
                PromoCode = promoCode,
                OriginalPrice = priceCalc.OriginalPrice,
                DiscountAmount = priceCalc.DiscountAmount,
                FinalPrice = priceCalc.FinalPrice
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(string paymentMethod)
        {
            var tourId = HttpContext.Session.GetInt32("Booking_TourId");
            if (!tourId.HasValue) return RedirectToAction("Create");

            var dto = new CreateBookingDTO
            {
                TourId = tourId.Value,
                CustomerName = HttpContext.Session.GetString("Booking_CustomerName") ?? "",
                Email = HttpContext.Session.GetString("Booking_Email") ?? "",
                PhoneNumber = HttpContext.Session.GetString("Booking_PhoneNumber") ?? "",
                AdultCount = HttpContext.Session.GetInt32("Booking_AdultCount") ?? 1,
                ChildCount = HttpContext.Session.GetInt32("Booking_ChildCount") ?? 0,
                PromoCode = HttpContext.Session.GetString("Booking_PromoCode"),
                SpecialRequest = HttpContext.Session.GetString("Booking_SpecialRequest"),
                PaymentMethod = paymentMethod,
                BookingDate = DateTime.Now
            };

            var response = await _bookingService.CreateBookingAsync(dto);

            if (response.Success)
            {
                // Clear session
                HttpContext.Session.Clear();
                return RedirectToAction("Success", new { bookingCode = response.Data?.BookingCode });
            }

            ModelState.AddModelError("", response.Message);
            return RedirectToAction("Review");
        }

        [HttpGet]
        public async Task<IActionResult> Success(string bookingCode)
        {
            var booking = await _bookingService.GetBookingByCodeAsync(bookingCode);
            if (booking == null) return NotFound();
            return View(booking);
        }
    }
}