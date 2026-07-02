using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Data.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public virtual Booking? Booking { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string Status { get; set; } = "Pending";

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string? TransactionId { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(200)]
        public string? Notes { get; set; }
    }
}
