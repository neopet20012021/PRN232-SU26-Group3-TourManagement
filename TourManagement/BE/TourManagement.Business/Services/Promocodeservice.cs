using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Data.Context;

namespace TourManagement.Business.Services
{
    public interface IPromoCodeService
    {
        Task<bool> IsValidAsync(string promoCode, decimal bookingValue = 0);
        Task<decimal> GetDiscountPercentAsync(string promoCode);
        Task<bool> UsePromoCodeAsync(string promoCode);
    }

    public class PromoCodeService : IPromoCodeService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PromoCodeService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<bool> IsValidAsync(string promoCode, decimal bookingValue = 0)
        {
            if (string.IsNullOrWhiteSpace(promoCode))
                return false;

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TourManagementDbContext>();
                var code = await context.PromoCodes.FirstOrDefaultAsync(p => p.Code.ToUpper() == promoCode.ToUpper());
                if (code == null)
                    return false;

                var now = DateTime.Now;
                if (!code.IsActive)
                    return false;
                if (now < code.StartDate || now > code.EndDate)
                    return false;
                if (code.UsageCount >= code.MaxUsage)
                    return false;
                if (bookingValue > 0 && bookingValue < code.MinBookingValue)
                    return false;

                return true;
            }
        }

        public async Task<decimal> GetDiscountPercentAsync(string promoCode)
        {
            if (string.IsNullOrWhiteSpace(promoCode))
                return 0m;

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TourManagementDbContext>();
                var code = await context.PromoCodes.FirstOrDefaultAsync(p => p.Code.ToUpper() == promoCode.ToUpper());
                return code?.DiscountPercent ?? 0m;
            }
        }

        public async Task<bool> UsePromoCodeAsync(string promoCode)
        {
            if (string.IsNullOrWhiteSpace(promoCode))
                return false;

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TourManagementDbContext>();
                var code = await context.PromoCodes.FirstOrDefaultAsync(p => p.Code.ToUpper() == promoCode.ToUpper());
                if (code == null)
                    return false;

                code.UsageCount++;
                context.PromoCodes.Update(code);
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
