using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class TemporalOrderRepository : Repository<TemporalOrder, int>
{
    public TemporalOrderRepository(FarminhouseContext context) : base(context) { }

    public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
    {
        return await GetQueryable()
            .Include(temporalOrder => temporalOrder.User)
            .Include(temporalOrder => temporalOrder.Wishlist)
            .FirstOrDefaultAsync(temporalOrder => temporalOrder.Id == id);
    }


}
