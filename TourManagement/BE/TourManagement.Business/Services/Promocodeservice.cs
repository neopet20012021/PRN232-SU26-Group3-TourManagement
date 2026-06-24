using System.Collections.Generic;
using System.Threading.Tasks;

namespace TourManagement.Business.Services
{
    public interface IPromoCodeService
    {
        Task<bool> IsValidAsync(string promoCode);
        Task<decimal> GetDiscountPercentAsync(string promoCode);
    }

    public class PromoCodeService : IPromoCodeService
    {
        // Promo codes tạm thời (có thể thay bằng DB query sau)
        private readonly Dictionary<string, decimal> _promoCodes = new()
        {
            { "TOUR2025", 0.10m },
            { "VIP50", 0.05m },
            { "SUMMER100", 0.15m }
        };

        public async Task<bool> IsValidAsync(string promoCode)
        {
            if (string.IsNullOrWhiteSpace(promoCode))
                return false;

            return await Task.FromResult(_promoCodes.ContainsKey(promoCode.ToUpper()));
        }

        public async Task<decimal> GetDiscountPercentAsync(string promoCode)
        {
            if (string.IsNullOrWhiteSpace(promoCode))
                return 0m;

            _promoCodes.TryGetValue(promoCode.ToUpper(), out var discountPercent);
            return await Task.FromResult(discountPercent);
        }
    }
}
