using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

// PREGUNTAR A JOSE POR QUÉ TENEMOS LOS MÉTODOS DE GETALL SI ACABAMOS USANDO GETQUERYABLE

public class OrderRepository : Repository<Order, int>
{
    public OrderRepository(FarminhouseContext context): base(context) { }

    public async Task<ICollection<Order>> GetAllWithFullDataAsync()
    {
        return await GetQueryable()
            .Include(order => order.Payments)
            .Include(order => order.User)
            .Include(order => order.Products)
            .ToArrayAsync();
    }

    /*public async Task<Order> GetById(int id)
    {
        Order order = await GetQueryable()
            .Include(order => order.Payments)
            .Include(order => order.User)
            .Include(order => order.Products)
            .First(order => order.Id == id);
        return order;
    }*/
}
