using AutoMapper;
using TourManagement.Business.DTOs;
using TourManagement.Data.Models;

namespace TourManagement.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Booking mappings
            CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.TourName, opt => opt.MapFrom(src => src.Schedule != null && src.Schedule.Tour != null ? src.Schedule.Tour.TourName : ""))
                .ForMember(dest => dest.PromoCode, opt => opt.MapFrom(src => src.AppliedPromoCode != null ? src.AppliedPromoCode.Code : src.PromoCode))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Payments.FirstOrDefault() != null ? src.Payments.FirstOrDefault().PaymentMethod : "cash"))
                .ReverseMap();

            CreateMap<CreateBookingDTO, Booking>()
                .ReverseMap();

            // Tour mappings
            CreateMap<Tour, TourSelectDTO>()
                .ReverseMap();

            CreateMap<TourSchedule, TourSelectDTO>()
                .ForMember(dest => dest.TourId, opt => opt.MapFrom(src => src.TourId))
                .ForMember(dest => dest.TourName, opt => opt.MapFrom(src => src.Tour != null ? src.Tour.TourName : ""))
                .ForMember(dest => dest.TourCode, opt => opt.MapFrom(src => src.Tour != null ? src.Tour.TourCode : ""))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Tour != null ? src.Tour.Category : ""))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Tour != null ? src.Tour.Destination : ""))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Tour != null ? src.Tour.Image : ""))
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.AvailableSeats))
                .ForMember(dest => dest.PricePerAdult, opt => opt.MapFrom(src => src.ActualAdultPrice))
                .ForMember(dest => dest.PricePerChild, opt => opt.MapFrom(src => src.ActualChildPrice))
                .ReverseMap();

            CreateMap<Tour, TourDTO>()
                .ReverseMap();

            // User mappings
            CreateMap<User, UserDTO>()
                .ReverseMap();

            // Review mappings
            CreateMap<Review, ReviewDTO>()
                .ReverseMap();
            CreateMap<CreateReviewDTO, Review>();
        }
    }

    public class TourDTO
    {
        public int TourId { get; set; }
        public string TourCode { get; set; } = string.Empty;
        public string TourName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Days { get; set; }
        public int Nights { get; set; }
        public decimal PricePerAdult { get; set; }
        public decimal ChildPrice { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public bool IsActive { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}