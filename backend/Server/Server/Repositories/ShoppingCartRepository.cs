using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart, int>
    {

        UserRepository _userRepository;

        public ShoppingCartRepository(FarminhouseContext context) : base(context) 
        { 
        }


        public async Task<ShoppingCart> GetAllByUserIdAsync(int id, bool temporal)
        {
            return await GetQueryable().Include(cart => cart.CartContent).FirstOrDefaultAsync(cart => cart.UserId == id && cart.Temporal == temporal);
        }


        //Método que añade un nuevo carrito a un usuario si no tenía
        //Devuelve True si tenía carrito, False si no
        public async Task AddNewShoppingCart(User user)
        {
            // Verificar si existe un carrito del usuario
            var existingShoppingCart = await _context.ShoppingCart
                .Where(cart => cart.UserId == user.Id)
                .FirstOrDefaultAsync();

            //Si no existe, lo crea
            if(existingShoppingCart == null)
            {
                _context.ShoppingCart.Add(new ShoppingCart {
                    UserId = user.Id,
                    User = user,
                });
            }


        }


        public void AddCartContent(ShoppingCart shoppingCart, CartContent cartContent)
        {
            CartContent c =  shoppingCart.CartContent.Where(c => c.Id == cartContent.Id).FirstOrDefault();

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



    }
}
