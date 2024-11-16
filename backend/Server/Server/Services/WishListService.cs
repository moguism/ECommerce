using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services
{
    public class WishListService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductsToBuyMapper _productsToBuyMapper;
        public WishListService(UnitOfWork unitOfWork, ProductsToBuyMapper productsToBuyMapper)
        {
            _unitOfWork = unitOfWork;
            _productsToBuyMapper = productsToBuyMapper;
        }


        public IEnumerable<ProductsToBuy> GetAllProductsByWishlistIdAsync(int wishlistId)
        {
            return _unitOfWork.ProductsToBuyRepository.GetAllProductsByWishlistId(wishlistId);
        }

        public async Task<Wishlist> CreateNewWishList(IEnumerable<CartContentDto> products)
        {
            Wishlist wishlist = new Wishlist();

            List<ProductsToBuy> productsToBuyList = _productsToBuyMapper.ToEntity(products).ToList();


            _unitOfWork.WishlistRepository.Add(wishlist);
            await _unitOfWork.SaveAsync();

            // Asignar el Id de la wishlist a los productos después de guardar
            foreach (var product in productsToBuyList)
            {
                // Asignamos correctamente el Id de la wishlist a cada producto
                product.WishlistId = wishlist.Id;
            }

            // Si es necesario guardar los productos asociados, puedes hacerlo ahora
            foreach (var product in productsToBuyList)
            {
                // Aquí asegúrate de que la operación de agregar esté correctamente manejada
                _unitOfWork.ProductsToBuyRepository.Add(product);
            }

            // Guardar los productos asociados en la base de datos
            await _unitOfWork.SaveAsync();

            // Devolver la wishlist creada
            return wishlist;
        }



    }
}
