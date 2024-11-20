using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class CartContentRepository : Repository<CartContent, int>
    {

        public CartContentRepository(FarminhouseContext context) : base(context) { }
        

        public async Task AddProductosToCartAsync(ShoppingCart shoppingCart, CartContentDto cartContentDto)
        {
            CartContent cartContent = await GetQueryable()
                .Include(c => c.Product)
                .Include(c => c.ShoppingCart)
                .FirstOrDefaultAsync(c => c.ShoppingCartId == shoppingCart.Id
                && c.ProductId == cartContentDto.ProductId);
                

            //Si el producto no estaba añadido al carrito, añade uno nuevo
            if (cartContent == null)
            {
                Add(new CartContent
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
                //Se pasa el la cantidad del producto
                cartContent.Quantity = cartContentDto.Quantity;

                Update(cartContent);
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
        public async Task DeleteByIdShoppingCartAsync(ShoppingCart cart)
        {
            IEnumerable<CartContent> cartContents = await GetQueryable().Where(c => c.ShoppingCartId == cart.Id).ToArrayAsync();
            foreach (CartContent cartContent in cartContents)
            {
                _context.CartContent.Remove(cartContent);
            }
        }
    }
}
