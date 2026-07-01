using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Data.Models
{
    [Table("TourSchedules")]
    public class TourSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int TourId { get; set; }

        [ForeignKey(nameof(TourId))]
        public virtual Tour? Tour { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số người tham gia tối đa phải lớn hơn 0")]
        public int MaxParticipants { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số chỗ trống không được nhỏ hơn 0")]
        public int AvailableSeats { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "Giá không được nhỏ hơn 0")]
        public decimal ActualAdultPrice { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "Giá không được nhỏ hơn 0")]
        public decimal ActualChildPrice { get; set; }

        [StringLength(20)]
        [Column(TypeName = "VARCHAR(20)")]
        public string Status { get; set; } = "Active"; // Active, Full, Closed, Cancelled

        [StringLength(100)]
        public string? GuideName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
