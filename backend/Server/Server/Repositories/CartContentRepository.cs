using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class CartContentRepository : Repository<CartContent, int>
    {

        public CartContentRepository(FarminhouseContext context) : base(context) { }
        

        public async Task AddProductosToCartAsync(ShoppingCart shoppingCart, CartContent cartContentDto)
        {
            await InsertAsync(new CartContent
            {
                ProductId = cartContentDto.ProductId,
                Quantity = cartContentDto.Quantity,
                ShoppingCartId = shoppingCart.Id,
                //Product = _context.Products.FirstOrDefault(p => p.Id == cartContentDto.ProductId),w
                //ShoppingCart = _context.ShoppingCart.FirstOrDefault(c => c.Id == shoppingCart.Id)
            });
        }


        public void RemoveProductFromCartAsync(User user, int productId)
        {
            CartContent cartContent = user.ShoppingCart.CartContent.FirstOrDefault(c => c.ProductId == productId);
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
