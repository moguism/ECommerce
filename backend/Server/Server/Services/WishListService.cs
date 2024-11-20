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

        public async Task<Wishlist> GetWishlistById(int wishlistId)
        {
            return await _unitOfWork.WishlistRepository.GetFullByIdAsync(wishlistId);
        }

        public async Task<Wishlist> CreateNewWishList(IEnumerable<CartContentDto> products)
        {
            Wishlist wishlist = new Wishlist();

            IEnumerable<ProductsToBuy> productsToBuyList = await _productsToBuyMapper.ToEntity(products);

            await _unitOfWork.WishlistRepository.InsertAsync(wishlist);
            await _unitOfWork.SaveAsync();

            // Asignar el Id de la wishlist a los productos después de guardar
            foreach (var product in productsToBuyList)
            {
                // Asignamos correctamente el Id de la wishlist a cada producto
                product.WishlistId = wishlist.Id;
                product.Product = await _unitOfWork.ProductRepository.GetFullProductById(product.Id);
                await _unitOfWork.ProductsToBuyRepository.InsertAsync(product);
                
            }

            await _unitOfWork.SaveAsync();

            IEnumerable<ProductsToBuy> finalProducts = _unitOfWork.ProductsToBuyRepository
                .GetAllProductsByWishlistId(wishlist.Id);


            wishlist.Products = productsToBuyList;
            _unitOfWork.WishlistRepository.Update(wishlist);

            // Guardar los productos asociados en la base de datos

            // Devolver la wishlist creada
            return wishlist;
        }



    }
}
