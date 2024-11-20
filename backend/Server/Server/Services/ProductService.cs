using Server.Models;

namespace Server.Services
{
    public class ProductService
    {

        private readonly UnitOfWork _unitOfWork;

        public ProductService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> GetProductById(int id)
        {

            // Pilla el usuario de la base de datos
            return await _unitOfWork.ProductRepository.GetByIdAsync(id);
        }

    }
}
