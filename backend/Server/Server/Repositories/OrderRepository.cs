using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class OrderRepository : Repository<Order, int>
{
    public OrderRepository(FarminhouseContext context): base(context) { }

    // SI UNA TABLA NO TIENE FK, USAR GetAllAsync DIRECTAMENTE
    public async Task<ICollection<Order>> GetAllWithFullDataAsync()
    {
        return await GetQueryable()
            //.Include(order => order.UserId)
            .ToArrayAsync();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersByUserId(int userId)
    {
        return await GetQueryable()
        .Include(o => o.Wishlist)
        .Where(o => o.UserId == userId)
        .ToListAsync();
    }

    public async Task<Order> GetById(int orderId)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(order => order.Id == orderId);
    }

    public Order GetLastByUserId(int userId)
    {
        return GetQueryable()
            .LastOrDefault(o => o.UserId == userId);
    }


    public async Task<Order> GetBySessionId(string sessionid)
    {
        Order order = await GetQueryable().FirstOrDefaultAsync(order => order.SessionId.Equals(sessionid));
        return order;
    }

    public async Task<Order> GetByHash(string hash)
    {
        Order order = await GetQueryable().FirstOrDefaultAsync(order => order.Hash.Equals(hash));
        return order;
    }
}
