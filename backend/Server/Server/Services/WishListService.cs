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

            List<ProductsToBuy> productsToBuyList = _productsToBuyMapper.ToDto(products.ToList()).ToList();

            //Añade la wishlist a los productos
            foreach (var product in productsToBuyList)
            {
                product.WishlistId = wishlist.Id;
                product.Wishlist = wishlist;

            }

            wishlist.Products = productsToBuyList;
            _unitOfWork.WishlistRepository.Add(wishlist);
            await _unitOfWork.SaveAsync();


            return wishlist;
        }


    }
}
