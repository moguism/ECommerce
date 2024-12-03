using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart, int>
    {

        public ShoppingCartRepository(FarminhouseContext context) : base(context)
        {
        }


        public async Task<ShoppingCart> GetAllByUserIdAsync(int id)
        {
            return await GetQueryable()
                .Include(cart => cart.CartContent)
                    .ThenInclude(content => content.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == id);
        }

        public async Task<ShoppingCart> GetAllShoppingCartByShoppingCartIdAsync(int id)
        {
            return await GetQueryable()
                .Include(cart => cart.CartContent)
                .ThenInclude(content => content.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == id);
        }


        //Método que añade un nuevo carrito a un usuario si no tenía
        public async Task<ShoppingCart> CreateShoppingCartAsync(User user)
        {
            ShoppingCart newCart = new ShoppingCart
            {
                UserId = user.Id,
            };

            ShoppingCart returnedCart = await InsertAsync(newCart);
            return returnedCart;
        }


        public void AddCartContent(ShoppingCart shoppingCart, CartContent cartContent)
        {
            CartContent c = shoppingCart.CartContent.Where(c => c.Id == cartContent.Id).FirstOrDefault();

            //Si el contenido del carrito del usuario no existe, lo añade
            if (c == null)
            {
                shoppingCart.CartContent.ToList().Add(cartContent);
            }
            //Sino, lo actualiza
            else
            {
                c.Quantity += cartContent.Quantity;
                //shoppingCart.CartContent.ToList().Update(c);

            }
        }
        public async Task<ShoppingCart> GetIdShoppingCartByUserId(int userId)
        {
            ShoppingCart shoppingCart = await GetQueryable().Include(shoppingCart => shoppingCart.CartContent).FirstOrDefaultAsync(c => c.UserId == userId);
            return shoppingCart;
        }



    }
}