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
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsByTourIdAsync(int tourId);
        Task<ReviewSummaryDTO> GetReviewSummaryAsync(int tourId);
        Task<ReviewDTO> AddReviewAsync(CreateReviewDTO dto);
    }

    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByTourIdAsync(int tourId)
        {
            var reviews = await _reviewRepository.GetReviewsByTourIdAsync(tourId);
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<ReviewSummaryDTO> GetReviewSummaryAsync(int tourId)
        {
            var reviewsList = (await _reviewRepository.GetReviewsByTourIdAsync(tourId)).ToList();

            var summary = new ReviewSummaryDTO
            {
                TourId = tourId,
                TotalReviews = reviewsList.Count,
                AverageRating = reviewsList.Any() ? Math.Round(reviewsList.Average(r => r.Rating), 1) : 0,
                AvgCleanliness = reviewsList.Any() ? Math.Round(reviewsList.Average(r => r.CleanlinessRating), 1) : 0,
                AvgComfort = reviewsList.Any() ? Math.Round(reviewsList.Average(r => r.ComfortRating), 1) : 0,
                AvgAmenities = reviewsList.Any() ? Math.Round(reviewsList.Average(r => r.AmenitiesRating), 1) : 0,
                AvgValue = reviewsList.Any() ? Math.Round(reviewsList.Average(r => r.ValueRating), 1) : 0,
                Reviews = _mapper.Map<List<ReviewDTO>>(reviewsList)
            };

            for (int i = 1; i <= 5; i++)
            {
                summary.StarCounts[i] = reviewsList.Count(r => r.Rating == i);
            }

            return summary;
        }

        public async Task<ReviewDTO> AddReviewAsync(CreateReviewDTO dto)
        {
            var tour = await _tourRepository.GetByIdAsync(dto.TourId);
            if (tour == null)
            {
                throw new ArgumentException("Tour không tồn tại.");
            }

            var review = _mapper.Map<Review>(dto);
            review.CreatedDate = DateTime.Now;

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return _mapper.Map<ReviewDTO>(review);
        }
    }
}
