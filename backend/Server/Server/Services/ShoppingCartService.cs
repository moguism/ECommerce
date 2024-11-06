using Server.DTOs;
using Server.Models;

namespace Server.Services
{
    public class ShoppingCartService
    {

        UnitOfWork _unitOfWork;

        public ShoppingCartService(UnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddProductsToShoppingCart(ShoppingCart shoppingCart, CartContentDto cartContentDto)
        {
            IEnumerable<CartContent> cartContents = await _unitOfWork.CartContentRepository
                .GetCartContentByShoppingCartIdAsync(shoppingCart.Id);

            CartContent cartContent = cartContents.Where(c => c.ProductId == shoppingCart.Id)
                .FirstOrDefault();

            //Si el producto no se ha añadido anteriormente, crea un contenido nuevo al carrito
            if (cartContent == null)
            {
                _unitOfWork.CartContentRepository.Add(new CartContent()
                {
                    ProductId = cartContentDto.ProductId,
                    Quantity = cartContentDto.Quantity,
                    ShoppingCartId = shoppingCart.Id,
                    Product = await _unitOfWork.ProductRepository.GetProductById(cartContentDto.ProductId)

                });
            }
            //Sino, actualiza la canitdad
            else
            {
                cartContent.Quantity += cartContentDto.Quantity;
                _unitOfWork.CartContentRepository.Update(cartContent);


            }

            await _unitOfWork.SaveAsync();

        }
    }
}
