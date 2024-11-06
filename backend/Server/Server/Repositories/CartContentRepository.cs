using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class CartContentRepository : Repository<CartContent, int>
    {

        public CartContentRepository(FarminhouseContext context) : base(context) { }

        public async Task AddProductsToShoppingCart(ShoppingCart shoppingCart)
        {
            
        }


    }
}
