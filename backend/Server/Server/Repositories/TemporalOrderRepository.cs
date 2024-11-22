using Microsoft.EntityFrameworkCore;
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
            .Include(temporalOrder => temporalOrder.User)
            .Include(temporalOrder => temporalOrder.Wishlist)
            .OrderBy(temporalOrder => temporalOrder.Id)
            .LastOrDefaultAsync(temporalOrder => temporalOrder.UserId == userId);
    }

    public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.User)
            .Include(temporalOrder => temporalOrder.Wishlist)
            .FirstOrDefaultAsync(temporalOrder => temporalOrder.Id == id);
    }
    public async Task<TemporalOrder> GetFullTemporalOderByIdWithoutUser(int id)
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.Wishlist)
            .FirstOrDefaultAsync(temporalOrder => temporalOrder.Id == id);
    }

    public async Task<TemporalOrder> GetFullTemporalOderByHashOrSession(string id)
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.Wishlist)
            .FirstOrDefaultAsync(temporalOrder => temporalOrder.HashOrSession.Equals(id));
    }

    public async Task<IEnumerable<TemporalOrder>> GetExpiredOrders(DateTime currentTime)
    {
        return await GetQueryable().Where(temporalOrder => temporalOrder.ExpirationDate >= currentTime).ToListAsync();
    }
}
