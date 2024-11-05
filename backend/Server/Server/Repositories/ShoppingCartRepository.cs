using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart, int>
    {

        public ShoppingCartRepository(FarminhouseContext context) : base(context) { }




    }
}
