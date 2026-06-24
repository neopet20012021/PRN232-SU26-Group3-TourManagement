using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Data.Models
{
    [Table("Tours")]
    public class Tour
    {
        [Key]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Tên tour là bắt buộc")]
        [StringLength(150)]
        public string TourName { get; set; } = string.Empty;

        [StringLength(20)]
        [Column(TypeName = "VARCHAR(20)")]
        public string TourCode { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Days { get; set; }

        [Required]
        public int Nights { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal PricePerAdult { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal ChildPrice { get; set; } // 70% of adult price

        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [StringLength(100)]
        public string Destination { get; set; } = string.Empty;

        [Required]
        public DateTime DepartureDate { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        [Required]
        public int MaxCapacity { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Itinerary { get; set; }

        [StringLength(500)]
        public string? IncludedServices { get; set; }

        [StringLength(500)]
        public string? ExcludedServices { get; set; }

        [StringLength(200)]
        public string? Image { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}