using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart, int>
    {

        public ShoppingCartRepository(FarminhouseContext context) : base(context) { }



        public async Task<ICollection<ShoppingCart>> GetAllByUserIdAsync(int id)
        {
            ICollection<ShoppingCart> shoppingCart =  await GetAllAsync();

            return shoppingCart
                .Where(cart => cart.UserId == id)
                .ToList();
        }





    }
}
