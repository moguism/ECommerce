using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services
{
    public class ShoppingCartService
    {

        UnitOfWork _unitOfWork;
        ShoppingCartMapper _shoppingCartMapper;

        public ShoppingCartService(UnitOfWork unitOfWork, ShoppingCartMapper shoppingCartMapper) 
        {
            _unitOfWork = unitOfWork;
            _shoppingCartMapper = shoppingCartMapper;
        }

        public async Task AddProductsToShoppingCart(User user, CartContentDto cartContentDto)
        {
            ShoppingCart cart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id);
            if (cart == null)
            {
                cart = new ShoppingCart();
                cart.UserId = user.Id;
                cart = await _unitOfWork.ShoppingCartRepository.InsertAsync(cart);
                await _unitOfWork.SaveAsync();
            }
            
            await _unitOfWork.CartContentRepository.AddProductosToCartAsync(cart, cartContentDto);

            await _unitOfWork.SaveAsync();


        }


        public async Task RemoveProductFromShoppingCart(User user, int productId)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id);
            await _unitOfWork.CartContentRepository.RemoveProductFromCartAsync(shoppingCart, productId);
            await _unitOfWork.SaveAsync();
        }


        public async Task<ShoppingCartDto> GetShoppingCartByUserIdAsync(int id)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(id);

            if (shoppingCart == null)
            {
                return null;
            }

            return _shoppingCartMapper.ToDto(shoppingCart);
        }

        public async Task<User> GetUserFromDbByStringId(string stringId)
        {

            // Pilla el usuario de la base de datos
            return await _unitOfWork.UserRepository.GetAllInfoById(Int32.Parse(stringId));
        }



    }
}
