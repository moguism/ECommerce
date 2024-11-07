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

        public async Task AddProductosToCartAsync(ShoppingCart shoppingCart, CartContentDto cartContentDto)
        {
            CartContent cartContent = await _context.CartContent
                .FirstOrDefaultAsync(c => c.ShoppingCartId == shoppingCart.Id 
                && c.ProductId == cartContentDto.ProductId);

            //Si el producto no estaba añadido al carrito, añade uno nuevo
            if (cartContent == null)
            {
                _context.CartContent.Add(new CartContent
                {
                    ProductId = cartContentDto.ProductId,
                    Quantity = cartContentDto.Quantity,
                    ShoppingCartId = shoppingCart.Id,
                    Product = _context.Products.FirstOrDefault(p => p.Id == cartContentDto.ProductId),
                    ShoppingCart = _context.ShoppingCart.FirstOrDefault(c => c.Id == shoppingCart.Id)
                });
            }
            else
            {
                //Sino, actualiza la cantidad
                cartContent.Quantity += cartContentDto.Quantity;
                _context.CartContent.Update(cartContent);
            }


        }

        public async Task RemoveProductFromCartAsync(ShoppingCart cart, int productId)
        {
            CartContent cartContent = await _context.CartContent.FirstOrDefaultAsync(c => c.ProductId == productId);
            if(cartContent == null)
            {
                return;
            }
            _context.CartContent.Remove(cartContent);


        }
    }
}
