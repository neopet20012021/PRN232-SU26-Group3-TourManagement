using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TourManagement.Business.DTOs;
using TourManagement.Data.Models;
using TourManagement.Data.Repositories;

namespace TourManagement.Business.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDTO> CreateBookingAsync(CreateBookingDTO bookingDto);
        Task<BookingDTO> GetBookingByIdAsync(int bookingId);
        Task<BookingDTO> GetBookingByCodeAsync(string bookingCode);
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<IEnumerable<BookingDTO>> GetCustomerBookingsAsync(string email);
        Task<IEnumerable<BookingDTO>> GetTourBookingsAsync(int tourId);
        Task<PriceCalculationDTO> CalculatePriceAsync(int tourId, int adultCount, int childCount, string promoCode = null);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string newStatus);
        Task<bool> UpdateBookingAsync(int bookingId, UpdateBookingDTO dto);
        Task<bool> DeleteBookingAsync(int bookingId);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<IEnumerable<TourSelectDTO>> GetActiveToursAsync(string? keyword = null, decimal? minPrice = null, decimal? maxPrice = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ValidatePromoCodeAsync(string promoCode);
    }

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public async Task<BookingResponseDTO> CreateBookingAsync(CreateBookingDTO bookingDto)
        {
            var tour = await _tourRepository.GetByIdAsync(bookingDto.TourId);
            if (tour == null) return new BookingResponseDTO { Success = false, Message = "Tour không tồn tại." };

            var priceCalc = await CalculatePriceAsync(bookingDto.TourId, bookingDto.AdultCount, bookingDto.ChildCount, bookingDto.PromoCode);
            var booking = new Booking
            {
                TourId = bookingDto.TourId,
                CustomerName = bookingDto.CustomerName,
                PhoneNumber = bookingDto.PhoneNumber,
                Email = bookingDto.Email,
                CCCD = bookingDto.CCCD,
                AdultCount = bookingDto.AdultCount,
                ChildCount = bookingDto.ChildCount,
                InfantCount = bookingDto.InfantCount,
                RoomType = bookingDto.RoomType,
                SpecialRequest = bookingDto.SpecialRequest,
                PromoCode = bookingDto.PromoCode,
                PaymentMethod = bookingDto.PaymentMethod,
                BookingDate = bookingDto.BookingDate,
                TotalPrice = priceCalc.FinalPrice,
                DiscountAmount = priceCalc.DiscountAmount,
                FinalPrice = priceCalc.FinalPrice,
                Status = "pending",
                BookingCode = GenerateBookingCode(),
                CreatedDate = DateTime.Now
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            // Reload with Tour for mapper
            var saved = await _bookingRepository.GetByIdAsync(booking.BookingId);
            var dto = _mapper.Map<BookingDTO>(saved);
            if (dto != null && tour != null)
                dto.TourName = tour.TourName;

            return new BookingResponseDTO { Success = true, Message = "Đặt tour thành công!", Data = dto };
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;
            var dto = _mapper.Map<BookingDTO>(booking);
            if (booking.Tour != null) dto.TourName = booking.Tour.TourName;
            return dto;
        }

        public async Task<BookingDTO> GetBookingByCodeAsync(string bookingCode)
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var booking = bookings.FirstOrDefault(b => b.BookingCode == bookingCode);
            if (booking == null) return null;
            var dto = _mapper.Map<BookingDTO>(booking);
            if (booking.Tour != null) dto.TourName = booking.Tour.TourName;
            return dto;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var tours = await _tourRepository.GetAllAsync();
            var tourDict = tours.ToDictionary(t => t.TourId, t => t.TourName);

            return bookings.Select(b =>
            {
                var dto = _mapper.Map<BookingDTO>(b);
                if (tourDict.TryGetValue(b.TourId, out var name))
                    dto.TourName = name;
                return dto;
            }).OrderByDescending(b => b.CreatedDate).ToList();
        }

        public async Task<IEnumerable<BookingDTO>> GetCustomerBookingsAsync(string email)
        {
            var bookings = (await _bookingRepository.GetAllAsync()).Where(b => b.Email == email);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<IEnumerable<BookingDTO>> GetTourBookingsAsync(int tourId)
        {
            var bookings = (await _bookingRepository.GetAllAsync()).Where(b => b.TourId == tourId);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<PriceCalculationDTO> CalculatePriceAsync(int tourId, int adultCount, int childCount, string promoCode = null)
        {
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null) throw new Exception("Tour not found");

            decimal originalPrice = (tour.PricePerAdult * adultCount) + (tour.ChildPrice * childCount);
            decimal discount = 0;
            if (!string.IsNullOrWhiteSpace(promoCode) && await ValidatePromoCodeAsync(promoCode))
            {
                discount = originalPrice * 0.1m; // 10% discount
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

        public async Task<bool> UpdateBookingAsync(int bookingId, UpdateBookingDTO dto)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            booking.CustomerName = dto.CustomerName;
            booking.PhoneNumber = dto.PhoneNumber;
            booking.Email = dto.Email;
            booking.AdultCount = dto.AdultCount;
            booking.ChildCount = dto.ChildCount;
            booking.InfantCount = dto.InfantCount;
            booking.SpecialRequest = dto.SpecialRequest;
            booking.Notes = dto.Notes;
            booking.Status = dto.Status;
            booking.PaymentMethod = dto.PaymentMethod;

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;
            _bookingRepository.Remove(booking);
            await _bookingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            return await UpdateBookingStatusAsync(bookingId, "cancelled");
        }

        public async Task<IEnumerable<TourSelectDTO>> GetActiveToursAsync(string? keyword = null, decimal? minPrice = null, decimal? maxPrice = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var tours = await _tourRepository.GetAllAsync();
            var query = tours.Where(t => t.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(t => t.TourName.ToLower().Contains(lowerKeyword) ||
                                         t.TourCode.ToLower().Contains(lowerKeyword) ||
                                         t.Destination.ToLower().Contains(lowerKeyword));
            }

            if (minPrice.HasValue) query = query.Where(t => t.PricePerAdult >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(t => t.PricePerAdult <= maxPrice.Value);
            if (fromDate.HasValue) query = query.Where(t => t.DepartureDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(t => t.DepartureDate.Date <= toDate.Value.Date);

            return _mapper.Map<IEnumerable<TourSelectDTO>>(query.ToList());
        }

        public async Task<bool> ValidatePromoCodeAsync(string promoCode)
        {
            return !string.IsNullOrWhiteSpace(promoCode);
        }

        private string GenerateBookingCode()
        {
            return $"TM-{DateTime.Now.Year}-{DateTime.Now.Ticks % 100000:D5}";
        }
    }
}