using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TourManagement.Business.DTOs;
using TourManagement.Data.Context;
using TourManagement.Data.Models;
using TourManagement.Data.Repositories;

namespace TourManagement.Business.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDTO> CreateBookingAsync(CreateBookingDTO bookingDto);
        Task<BookingDTO> GetBookingByIdAsync(int bookingId);
        Task<BookingDTO> GetBookingByCodeAsync(string bookingCode);
        Task<IEnumerable<BookingDTO>> GetCustomerBookingsAsync(string email);
        Task<IEnumerable<BookingDTO>> GetScheduleBookingsAsync(int scheduleId);
        Task<PriceCalculationDTO> CalculatePriceAsync(int scheduleId, int adultCount, int childCount, string promoCode = null, string userEmail = null, int? userId = null);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string newStatus);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<IEnumerable<TourSelectDTO>> GetActiveToursAsync(string? keyword = null, decimal? minPrice = null, decimal? maxPrice = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ValidatePromoCodeAsync(string promoCode);
    }

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITourScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;
        private readonly IPromoCodeService _promoCodeService;
        private readonly TourManagementDbContext _context;

        public BookingService(
            IBookingRepository bookingRepository, 
            ITourRepository tourRepository, 
            ITourScheduleRepository scheduleRepository, 
            IMapper mapper, 
            IPromoCodeService promoCodeService,
            TourManagementDbContext context)
        {
            _bookingRepository = bookingRepository;
            _tourRepository = tourRepository;
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
            _promoCodeService = promoCodeService;
            _context = context;
        }

        public async Task<BookingResponseDTO> CreateBookingAsync(CreateBookingDTO bookingDto)
        {
            var priceCalc = await CalculatePriceAsync(bookingDto.ScheduleId, bookingDto.AdultCount, bookingDto.ChildCount, bookingDto.PromoCode, bookingDto.Email, bookingDto.UserId);
            
            int? promoCodeId = null;
            if (!string.IsNullOrWhiteSpace(bookingDto.PromoCode))
            {
                var promoCodeEntity = await _context.PromoCodes.FirstOrDefaultAsync(p => p.Code == bookingDto.PromoCode);
                promoCodeId = promoCodeEntity?.PromoCodeId;
            }

            var booking = new Booking
            {
                ScheduleId = bookingDto.ScheduleId,
                CustomerName = bookingDto.CustomerName,
                PhoneNumber = bookingDto.PhoneNumber,
                Email = bookingDto.Email,
                CCCD = bookingDto.CCCD,
                AdultCount = bookingDto.AdultCount,
                ChildCount = bookingDto.ChildCount,
                InfantCount = bookingDto.InfantCount,
                RoomType = bookingDto.RoomType,
                SpecialRequest = bookingDto.SpecialRequest,
                PaymentMethod = bookingDto.PaymentMethod,
                BookingDate = bookingDto.BookingDate,
                TotalPrice = priceCalc.OriginalPrice,
                DiscountAmount = priceCalc.DiscountAmount,
                FinalPrice = priceCalc.FinalPrice,
                Status = "pending",
                BookingCode = GenerateBookingCode(),
                UserId = bookingDto.UserId,
                PromoCodeId = promoCodeId,
                PromoCode = bookingDto.PromoCode
            };

            var payment = new Payment
            {
                Amount = priceCalc.FinalPrice,
                PaymentMethod = bookingDto.PaymentMethod,
                Status = "Pending",
                PaymentDate = DateTime.Now
            };

            booking.Payments.Add(payment);

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            if (priceCalc.DiscountAmount > 0 && !string.IsNullOrWhiteSpace(bookingDto.PromoCode))
            {
                await _promoCodeService.UsePromoCodeAsync(bookingDto.PromoCode);
            }

            return new BookingResponseDTO { Success = true, Data = _mapper.Map<BookingDTO>(booking) };
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO> GetBookingByCodeAsync(string bookingCode)
        {
            var booking = (await _bookingRepository.GetAllAsync()).FirstOrDefault(b => b.BookingCode == bookingCode);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<IEnumerable<BookingDTO>> GetCustomerBookingsAsync(string email)
        {
            var bookings = (await _bookingRepository.GetAllAsync()).Where(b => b.Email == email);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<IEnumerable<BookingDTO>> GetScheduleBookingsAsync(int scheduleId)
        {
            var bookings = (await _bookingRepository.GetAllAsync()).Where(b => b.ScheduleId == scheduleId);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<PriceCalculationDTO> CalculatePriceAsync(int scheduleId, int adultCount, int childCount, string promoCode = null, string userEmail = null, int? userId = null)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }

            decimal originalPrice = (schedule.ActualAdultPrice * adultCount) + (schedule.ActualChildPrice * childCount);
            decimal discount = 0;
            
            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                if (promoCode.Trim().ToUpper() == "WELCOME")
                {
                    if (userId.HasValue)
                    {
                        var hasPreviousBooking = await _context.Bookings.AnyAsync(b => b.UserId == userId.Value);
                        if (hasPreviousBooking)
                        {
                            throw new Exception("Mã WELCOME chỉ áp dụng cho lần đặt tour đầu tiên của tài khoản này.");
                        }
                    }
                    else
                    {
                        throw new Exception("Mã WELCOME chỉ áp dụng cho tài khoản đăng nhập đặt tour lần đầu.");
                    }
                }

                if (await _promoCodeService.IsValidAsync(promoCode, originalPrice))
                {
                    decimal discountPercent = await _promoCodeService.GetDiscountPercentAsync(promoCode);
                    discount = originalPrice * discountPercent;
                }
            }

            return new PriceCalculationDTO
            {
                OriginalPrice = originalPrice,
                DiscountAmount = discount,
                FinalPrice = originalPrice - discount,
                AppliedPromoCode = discount > 0 ? promoCode : null
            };
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string newStatus)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;
            booking.Status = newStatus;
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            return await UpdateBookingStatusAsync(bookingId, "cancelled");
        }

        public async Task<IEnumerable<TourSelectDTO>> GetActiveToursAsync(string? keyword = null, decimal? minPrice = null, decimal? maxPrice = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.TourSchedules
                .Include(s => s.Tour)
                .Where(s => s.Tour.IsActive && s.Status == "Active")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(s => s.Tour.TourName.ToLower().Contains(lowerKeyword) || 
                                         s.Tour.TourCode.ToLower().Contains(lowerKeyword) || 
                                         s.Tour.Destination.ToLower().Contains(lowerKeyword));
            }

            if (minPrice.HasValue) query = query.Where(s => s.ActualAdultPrice >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(s => s.ActualAdultPrice <= maxPrice.Value);
            if (fromDate.HasValue) query = query.Where(s => s.StartDate >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(s => s.StartDate <= toDate.Value);

            var schedules = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TourSelectDTO>>(schedules);
        }

        public async Task<bool> ValidatePromoCodeAsync(string promoCode)
        {
            return !string.IsNullOrWhiteSpace(promoCode) && await _promoCodeService.IsValidAsync(promoCode);
        }

        private string GenerateBookingCode()
        {
            return $"TM-{DateTime.Now.Year}-{DateTime.Now.Ticks % 100000:D5}";
        }
    }
}