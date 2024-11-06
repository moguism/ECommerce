using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class CartContentRepository : Repository<CartContent, int>
    {

        public CartContentRepository(FarminhouseContext context) : base(context) { }

        public async Task<ICollection<CartContent>> GetByShoppingCartIdAsync(int id)
        {
            return await _context.Set<CartContent>().ToArrayAsync();
        }


        //Añade productos al carrito, si ya existe el producto en el carrito de un usuario, incrementa la cantidad, sino, añade uno nuevo
        public async Task AddProductToCartAsync(CartContent cartContent)
        {


            // Verificar si el producto ya está en el carrito
            var existingProduct = await _context.CartContent
                .FirstOrDefaultAsync(cart => cart.ProductId == cartContent.ProductId && cart.ShoppingCartId == cartContent.ShoppingCartId);

            if (existingProduct != null)
            {
                // Si el producto ya existe, solo se incrementa la cantidad
                existingProduct.Quantity += cartContent.Quantity;
                _context.CartContent.Update(existingProduct);
            }
            else
            {
                // Si el producto no existe, se añade como un nuevo registro
                await _context.CartContent.AddAsync(cartContent);
            }


            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();


        }

    }
}
