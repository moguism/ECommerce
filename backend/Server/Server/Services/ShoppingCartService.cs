using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services
{
    public class ShoppingCartService
    {

        private readonly UnitOfWork _unitOfWork;

        public ShoppingCartService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddProductsToShoppingCart(User user, CartContent cartContent, bool add)
        {
            CartContent existingCartContent = user.ShoppingCart.CartContent.FirstOrDefault(c => c.ProductId == cartContent.ProductId);
            if (existingCartContent != null)
            {
                Product p = existingCartContent.Product;
                if(add)
                {
                    if (cartContent.Quantity + existingCartContent.Quantity > p.Stock)
                    {
                        return;
                    }
                    existingCartContent.Quantity += cartContent.Quantity;
                }
                else
                {
                    if (cartContent.Quantity > p.Stock)
                    {
                        return;
                    }
                    existingCartContent.Quantity = cartContent.Quantity;
                }
                
                _unitOfWork.CartContentRepository.Update(existingCartContent);
                await _unitOfWork.SaveAsync();
                return;
            }

            // Si no existía previamente            
            Product product = await _unitOfWork.ProductRepository.GetByIdAsync(cartContent.ProductId);
            if (product == null || cartContent.Quantity > product.Stock)
            {
                return;
            }

            //añade el producto a este
            await _unitOfWork.CartContentRepository.AddProductosToCartAsync(user.ShoppingCart, cartContent);

            await _unitOfWork.SaveAsync();
        }


        public async Task RemoveProductFromShoppingCart(User user, int productId)
        {
            _unitOfWork.CartContentRepository.RemoveProductFromCartAsync(user, productId);
            await _unitOfWork.SaveAsync();
        }


        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(int id)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(id);

            if (shoppingCart == null)
            {
                return null;
            }

            return shoppingCart;
        }

        public async Task<User> GetUserFromDbByStringId(string stringId)
        {

            // Pilla el usuario de la base de datos
            return await _unitOfWork.UserRepository.GetAllInfoButOrdersById(Int32.Parse(stringId));
        }

    }
}