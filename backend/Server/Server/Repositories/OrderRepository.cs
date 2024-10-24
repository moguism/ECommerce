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
            .Include(order => order.Payments)
            .Include(order => order.User)
            .Include(order => order.Products)
            .ToArrayAsync();
    }

    public async Task<Order> GetById(int id)
    {
        // "FirstOrDefaultAsync" DEVUELVE NULO SI NO EXISTE
        Order order = await GetQueryable()
            .Include(order => order.Payments)
            .Include(order => order.User)
            .Include(order => order.Products)
            .FirstOrDefaultAsync(order => order.Id == id);
        return order;
    }
}
