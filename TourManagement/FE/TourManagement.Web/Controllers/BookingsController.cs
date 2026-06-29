using System;
using System.Linq;
using System.Text.Json;
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
        private const string WizardSessionKey = "BookingWizardSession";

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // --- Helper Methods for Session ---
        private BookingWizardSessionModel GetWizardSession()
        {
            var json = HttpContext.Session.GetString(WizardSessionKey);
            return string.IsNullOrEmpty(json) ? new BookingWizardSessionModel() : JsonSerializer.Deserialize<BookingWizardSessionModel>(json) ?? new BookingWizardSessionModel();
        }

        private void SaveWizardSession(BookingWizardSessionModel model)
        {
            var json = JsonSerializer.Serialize(model);
            HttpContext.Session.SetString(WizardSessionKey, json);
        }

        private void ClearWizardSession()
        {
            HttpContext.Session.Remove(WizardSessionKey);
        }
        // ----------------------------------

        // STEP 1: Select Tour
        [HttpGet]
        public async Task<IActionResult> Create(string? keyword = null, decimal? minPrice = null, decimal? maxPrice = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var tours = await _bookingService.GetActiveToursAsync(keyword, minPrice, maxPrice, fromDate, toDate);
            var session = GetWizardSession();
            if (session.SelectedTourId > 0)
            {
                ViewBag.SelectedTourId = session.SelectedTourId;
            }
            ViewBag.Keyword = keyword;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(tours);
        }

        [HttpPost]
        public IActionResult SelectTour(int tourId)
        {
            if (tourId <= 0) return RedirectToAction("Create");

            var session = GetWizardSession();
            session.SelectedTourId = tourId;
            SaveWizardSession(session);

            return RedirectToAction("BookingDetails");
        }

        // STEP 2: Booking Details
        [HttpGet]
        public async Task<IActionResult> BookingDetails()
        {
            var session = GetWizardSession();
            
            // Guard: Cannot access Step 2 without Step 1
            if (session.SelectedTourId <= 0) 
                return RedirectToAction("Create");

            var tours = await _bookingService.GetActiveToursAsync();
            var tour = tours.FirstOrDefault(t => t.TourId == session.SelectedTourId);
            if (tour == null) return RedirectToAction("Create");

            var model = new BookingDetailsViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                DepartureDate = tour.StartDate,
                PricePerAdult = tour.PricePerAdult,
                PricePerChild = tour.PricePerChild,
                
                CustomerName = session.CustomerName ?? "",
                Email = session.Email ?? "",
                PhoneNumber = session.Phone ?? "",
                Address = session.Address,
                AdultCount = session.AdultCount > 0 ? session.AdultCount : 1,
                ChildCount = session.ChildCount,
                InfantCount = 0,
                PromoCode = session.PromoCode,
                SpecialRequest = session.SpecialRequest
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BookingDetails(BookingDetailsViewModel model)
        {
            var session = GetWizardSession();
            
            // Guard
            if (session.SelectedTourId <= 0) 
                return RedirectToAction("Create");

            ModelState.Remove("PromoCode");
            ModelState.Remove("SpecialRequest");
            ModelState.Remove("TourName");

            var tours = await _bookingService.GetActiveToursAsync();
            var tour = tours.FirstOrDefault(t => t.TourId == session.SelectedTourId);
            if (tour != null && (model.AdultCount + model.ChildCount + model.InfantCount) > tour.AvailableSeats)
            {
                ModelState.AddModelError("", $"Số lượng hành khách ({model.AdultCount + model.ChildCount + model.InfantCount}) vượt quá số chỗ còn trống ({tour.AvailableSeats}).");
            }

            if (ModelState.IsValid)
            {
                session.CustomerName = model.CustomerName;
                session.Email = model.Email;
                session.Phone = model.PhoneNumber;
                session.Address = model.Address;
                session.AdultCount = model.AdultCount;
                session.ChildCount = model.ChildCount;
                session.PromoCode = model.PromoCode;
                session.SpecialRequest = model.SpecialRequest;
                session.PaymentMethod = model.PaymentMethod;
                session.BookingDate = DateTime.Now;

                SaveWizardSession(session);
                return RedirectToAction("Review");
            }
            return View(model);
        }

        // STEP 3: Review
        [HttpGet]
        public async Task<IActionResult> Review()
        {
            var session = GetWizardSession();
            
            // Guard: Cannot access Step 3 without Step 1 & 2
            if (session.SelectedTourId <= 0) return RedirectToAction("Create");
            if (string.IsNullOrEmpty(session.CustomerName) || string.IsNullOrEmpty(session.Phone)) return RedirectToAction("BookingDetails");

            var tours = await _bookingService.GetActiveToursAsync();
            var tour = tours.FirstOrDefault(t => t.TourId == session.SelectedTourId);
            
            var priceCalc = await _bookingService.CalculatePriceAsync(session.SelectedTourId, session.AdultCount, session.ChildCount, session.PromoCode);

            var model = new BookingReviewViewModel
            {
                TourName = tour?.TourName,
                DepartureDate = tour?.StartDate ?? DateTime.Now,
                CustomerName = session.CustomerName,
                Email = session.Email,
                PhoneNumber = session.Phone,
                Address = session.Address,
                AdultCount = session.AdultCount,
                ChildCount = session.ChildCount,
                PromoCode = session.PromoCode,
                OriginalPrice = priceCalc.OriginalPrice,
                DiscountAmount = priceCalc.DiscountAmount,
                FinalPrice = priceCalc.FinalPrice
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(string paymentMethod)
        {
            var session = GetWizardSession();
            if (session.SelectedTourId <= 0) return RedirectToAction("Create");

            // Allow paymentMethod override from form or fallback to session
            var finalPaymentMethod = !string.IsNullOrEmpty(paymentMethod) ? paymentMethod : (session.PaymentMethod ?? "cash");

            var dto = new CreateBookingDTO
            {
                ScheduleId = session.SelectedTourId,
                CustomerName = session.CustomerName ?? "",
                Email = session.Email ?? "",
                PhoneNumber = session.Phone ?? "",
                Address = session.Address,
                AdultCount = session.AdultCount,
                ChildCount = session.ChildCount,
                PromoCode = session.PromoCode,
                SpecialRequest = session.SpecialRequest,
                PaymentMethod = finalPaymentMethod,
                BookingDate = session.BookingDate != default ? session.BookingDate : DateTime.Now
            };

            var response = await _bookingService.CreateBookingAsync(dto);

            if (response.Success)
            {
                ClearWizardSession();
                return RedirectToAction("Success", new { bookingCode = response.Data?.BookingCode });
            }

            ModelState.AddModelError("", response.Message);
            return RedirectToAction("Review");
        }

        // STEP 4: Success
        [HttpGet]
        public async Task<IActionResult> Success(string bookingCode)
        {
            if (string.IsNullOrEmpty(bookingCode)) return RedirectToAction("Create");
            
            var booking = await _bookingService.GetBookingByCodeAsync(bookingCode);
            if (booking == null) return NotFound();
            
            return View(booking);
        }
    }
}
