using Server.Models;

namespace Server.Services
{
    public class WishListService
    {
        UnitOfWork _unitOfWork;
        public WishListService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<ProductsToBuy> GetAllProductsByWishlistIdAsync(int wishlistId)
        {
            return _unitOfWork.ProductsToBuyRepository.GetAllProductsByWishlistId(wishlistId);
        }

    }
}
