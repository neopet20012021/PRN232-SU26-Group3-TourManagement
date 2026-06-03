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
                .ForMember(dest => dest.TourName, opt => opt.MapFrom(src => src.Tour != null ? src.Tour.TourName : ""))
                .ReverseMap();

            CreateMap<CreateBookingDTO, Booking>()
                .ReverseMap();

            // Tour mappings
            CreateMap<Tour, TourSelectDTO>()
                .ReverseMap();

            CreateMap<Tour, TourDTO>()
                .ReverseMap();
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
        public System.DateTime DepartureDate { get; set; }
        public int AvailableSeats { get; set; }
        public int MaxCapacity { get; set; }
        public bool IsActive { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}