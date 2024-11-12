using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services
{
    public class ShoppingCartService
    {

        UnitOfWork _unitOfWork;

        public ShoppingCartService(UnitOfWork unitOfWork, ShoppingCartMapper shoppingCartMapper)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddProductsToShoppingCart(User user, CartContentDto cartContentDto, bool add)
        {
            ShoppingCart cart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id, false);
            if (cart == null)
            {
                cart = await CreateShoppingCart(user, false);
            }

            await _unitOfWork.CartContentRepository.AddProductosToCartAsync(cart, cartContentDto, add);

            await _unitOfWork.SaveAsync();


        }

        public async Task<ShoppingCart> CreateShoppingCart(User user, bool Temporal)
        {
            ShoppingCart cart = new ShoppingCart();
            cart.UserId = user.Id;
            cart.Temporal = Temporal;
            cart = await _unitOfWork.ShoppingCartRepository.InsertAsync(cart);
            await _unitOfWork.SaveAsync();
            return cart;
        }


        public async Task RemoveProductFromShoppingCart(User user, int productId, bool temporal)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id, temporal);
            await _unitOfWork.CartContentRepository.RemoveProductFromCartAsync(shoppingCart, productId);
            await _unitOfWork.SaveAsync();
        }


        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(int id, bool temporal)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(id, temporal);

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


        public async Task<ShoppingCart> ChangeTemporalAttribute(ShoppingCart cart, bool isTemporal)
        {
            cart.Temporal = isTemporal;
            _unitOfWork.ShoppingCartRepository.Update(cart);
            await _unitOfWork.SaveAsync();
            return cart;
        }
    }
}