using Microsoft.EntityFrameworkCore;
using Nethereum.Util;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class TemporalOrderRepository : Repository<TemporalOrder, int>
{
    public TemporalOrderRepository(FarminhouseContext context) : base(context) { }

    public async Task<TemporalOrder> GetFullTemporalOrderByUserId(int userId)
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.Wishlist)
            .Include(temporalOrder => temporalOrder.Wishlist.Products)
            .ThenInclude(product => product.Product)
            .OrderBy(temporalOrder => temporalOrder.Id)
            .LastOrDefaultAsync(temporalOrder => temporalOrder.UserId == userId);
    }

    public async Task<IEnumerable<TemporalOrder>> GetFullTemporalOrders()
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.Wishlist)
            .ThenInclude(w => w.Products)
            .ToListAsync();
    }

    public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
    {
        return await GetQueryable(false)
            .Include(temporalOrder => temporalOrder.Wishlist)
            .Include(temporalOrder => temporalOrder.Wishlist.Products)
            .ThenInclude(product => product.Product)
            .FirstOrDefaultAsync(temporalOrder => temporalOrder.Id == id);
    }
    public async Task<TemporalOrder> GetFullTemporalOderByIdWithoutUser(int id)
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.Wishlist)
            .Include(temporalOrder => temporalOrder.Wishlist.Products)
            .ThenInclude(product => product.Product)
            .FirstOrDefaultAsync(temporalOrder => temporalOrder.Id == id);
    }

    public async Task<TemporalOrder> GetFullTemporalOderByHashOrSession(string id)
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.Wishlist)
            .Include(temporalOrder => temporalOrder.Wishlist.Products)
            .ThenInclude(product => product.Product)
            .FirstOrDefaultAsync(temporalOrder => temporalOrder.HashOrSession.Equals(id));
    }

    public async Task<IEnumerable<TemporalOrder>> GetExpiredOrders(DateTime currentTime)
    {
        return await GetQueryable()
            .Where(temporalOrder => temporalOrder.ExpirationDate <= currentTime)
            .Include(temporalOrder => temporalOrder.Wishlist)
            .ThenInclude(w => w.Products)
            .ThenInclude(p => p.Product)
            .AsNoTracking()
            .ToListAsync();
    }
}
