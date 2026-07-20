using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Business.DTOs
{
    public class ReviewDTO
    {
        public int ReviewId { get; set; }
        public int TourId { get; set; }
        public int? UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int CleanlinessRating { get; set; }
        public int ComfortRating { get; set; }
        public int AmenitiesRating { get; set; }
        public int ValueRating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateReviewDTO
    {
        [Required(ErrorMessage = "Mã tour là bắt buộc")]
        public int TourId { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Họ tên người đánh giá là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không dài quá 100 ký tự")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số sao đánh giá chung là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Đánh giá từ 1 đến 5 sao")]
        public int Rating { get; set; } = 5;

        [Range(1, 5)]
        public int CleanlinessRating { get; set; } = 5;

        [Range(1, 5)]
        public int ComfortRating { get; set; } = 5;

        [Range(1, 5)]
        public int AmenitiesRating { get; set; } = 5;

        [Range(1, 5)]
        public int ValueRating { get; set; } = 5;

        [Required(ErrorMessage = "Bình luận là bắt buộc")]
        [StringLength(1000, ErrorMessage = "Nội dung nhận xét không quá 1000 ký tự")]
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewSummaryDTO
    {
        public int TourId { get; set; }
        public double AverageRating { get; set; }
        public double AvgCleanliness { get; set; }
        public double AvgComfort { get; set; }
        public double AvgAmenities { get; set; }
        public double AvgValue { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> StarCounts { get; set; } = new Dictionary<int, int>();
        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}
