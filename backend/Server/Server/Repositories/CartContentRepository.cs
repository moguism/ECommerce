using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class CartContentRepository : Repository<CartContent, int>
    {

        public CartContentRepository(FarminhouseContext context) : base(context) { }


        public async Task<IEnumerable<CartContent>> GetCartContentByShoppingCartIdAsync(int shopCartId)
        {
            return _context.CartContent.Where(c => c.ShoppingCartId == shopCartId).ToList();
        }


    }
}
