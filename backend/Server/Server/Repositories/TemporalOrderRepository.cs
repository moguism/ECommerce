using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class TemporalOrderRepository : Repository<TemporalOrder, int>
{
    public TemporalOrderRepository(FarminhouseContext context) : base(context) { }

    public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
    {
        return await GetQueryable().Include(temporalOrder => temporalOrder.ShoppingCart).FirstOrDefaultAsync(temporalOrder => temporalOrder.Id == id);
    }

    /*public async Task<TemporalOrder> AddDirectOrder(IEnumerable<CartContent> cartContents, ShoppingCart cart)
    {
        // Agrego todos los productos
        foreach (CartContent cartContent in cartContents)
        {
            _context.CartContent.Add(new CartContent
            {
                ProductId = cartContent.ProductId,
                Quantity = cartContent.Quantity,
                ShoppingCartId = cart.Id,
                Product = _context.Products.FirstOrDefault(p => p.Id == cartContent.ProductId),
                ShoppingCart = _context.ShoppingCart.FirstOrDefault(c => c.Id == cart.Id)
            });
        }
        return null;
    }*/

}
