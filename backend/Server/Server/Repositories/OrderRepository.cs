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

    public async Task<Order> GetByHashOrSession(string hash)
    {
        Order order = await GetQueryable().FirstOrDefaultAsync(order => order.HashOrSession.Equals(hash));
        return order;
    }
}
