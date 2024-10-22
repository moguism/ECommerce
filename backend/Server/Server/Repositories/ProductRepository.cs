using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ProductRepository : Repository<Order, int>
    {
        public ProductRepository(FarminhouseContext context) : base(context) { }

    }
}
