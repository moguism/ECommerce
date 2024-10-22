using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class OrderRepository : Repository<Order, int>
{
    public OrderRepository(FarminhouseContext context): base(context) { }
}
