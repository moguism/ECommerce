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

        public async Task AddProductsToShoppingCart(User user, CartContentDto cartContentDto)
        {
            ShoppingCart cart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id);

            //Si el usuario es nuevo y no tenía carrito, le crea uno nuevo
            if (cart == null)
            {
                cart = await _unitOfWork.ShoppingCartRepository.CreateShoppingCartAsync(user);
                await _unitOfWork.SaveAsync();
            }

            //añade el producto a este
            await _unitOfWork.CartContentRepository.AddProductosToCartAsync(cart, cartContentDto);

            await _unitOfWork.SaveAsync();


        }


        public async Task RemoveProductFromShoppingCart(User user, int productId)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id);
            await _unitOfWork.CartContentRepository.RemoveProductFromCartAsync(shoppingCart, productId);
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
            return await _unitOfWork.UserRepository.GetAllInfoById(Int32.Parse(stringId));
        }

    }
}