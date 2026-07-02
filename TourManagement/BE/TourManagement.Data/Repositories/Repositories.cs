using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data.Context;

namespace TourManagement.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task SaveChangesAsync();
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TourManagementDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(TourManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null) => predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);
        public void Update(T entity) => _dbSet.Update(entity);
        public void UpdateRange(IEnumerable<T> entities) => _dbSet.UpdateRange(entities);
        public void Remove(T entity) => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }

    public interface IBookingRepository : IRepository<Models.Booking>
    {
    }

    public class BookingRepository : Repository<Models.Booking>, IBookingRepository
    {
        public BookingRepository(TourManagementDbContext context) : base(context)
        {
        }
    }

    public interface ITourRepository : IRepository<Models.Tour>
    {
        Task<Models.Tour> GetByTourCodeAsync(string tourCode);
        Task<IEnumerable<Models.Tour>> GetActiveTours();
        Task<IEnumerable<Models.Tour>> GetToursByCategory(string category);
        Task<IEnumerable<Models.Tour>> SearchTours(string searchTerm);
    }

    public class TourRepository : Repository<Models.Tour>, ITourRepository
    {
        public TourRepository(TourManagementDbContext context) : base(context)
        {
        }

        public async Task<Models.Tour> GetByTourCodeAsync(string tourCode)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TourCode == tourCode);
        }

        public async Task<IEnumerable<Models.Tour>> GetActiveTours()
        {
            return await _dbSet
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Tour>> GetToursByCategory(string category)
        {
            return await _dbSet
                .Where(t => t.IsActive && t.Category == category)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Tour>> SearchTours(string searchTerm)
        {
            return await _dbSet
                .Where(t => t.IsActive && (
                    t.TourName.Contains(searchTerm) ||
                    t.TourCode.Contains(searchTerm) ||
                    t.Destination.Contains(searchTerm)
                ))
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }

    public interface IUserRepository : IRepository<Models.User>
    {
        Task<Models.User> GetByUsernameAsync(string username);
    }

    public class UserRepository : Repository<Models.User>, IUserRepository
    {
        public UserRepository(TourManagementDbContext context) : base(context)
        {
        }

        public async Task<Models.User> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }
    }

    public interface ITourScheduleRepository : IRepository<Models.TourSchedule>
    {
    }

    public class TourScheduleRepository : Repository<Models.TourSchedule>, ITourScheduleRepository
    {
        public TourScheduleRepository(TourManagementDbContext context) : base(context)
        {
        }
    }

    public interface IPromoCodeRepository : IRepository<Models.PromoCode>
    {
        Task<Models.PromoCode> GetByCodeAsync(string code);
    }

    public class PromoCodeRepository : Repository<Models.PromoCode>, IPromoCodeRepository
    {
        public PromoCodeRepository(TourManagementDbContext context) : base(context)
        {
        }

        public async Task<Models.PromoCode> GetByCodeAsync(string code)
        {
            return await _context.PromoCodes.FirstOrDefaultAsync(p => p.Code == code && p.IsActive);
        }
    }

    public interface IPaymentRepository : IRepository<Models.Payment>
    {
    }

    public class PaymentRepository : Repository<Models.Payment>, IPaymentRepository
    {
        public PaymentRepository(TourManagementDbContext context) : base(context)
        {
        }
    }
}