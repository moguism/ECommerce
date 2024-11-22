using Server.Models;
using Server.DTOs;

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

        public async Task<Product> InsertProduct(Product product)
        {
            Product newProduct = await _unitOfWork.ProductRepository.InsertAsync(product);
            await _unitOfWork.SaveAsync();
            return newProduct;
        }

        public async Task UpdateProduct(Product product)
        {
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Product> GetFullProductById(int id)
        {

            // Pilla el usuario de la base de datos
            return await _unitOfWork.ProductRepository.GetFullProductById(id);
        }

        public async Task<IEnumerable<Product>> GetFullProducts()
        {
            return await _unitOfWork.ProductRepository.GetFullProducts();
        }

        public PagedDto GetAllProductsByCategory(string productCategory, int pageNumber, int pageSize, IEnumerable<Product> products)
        {
            return _unitOfWork.ProductRepository.GetAllProductsByCategory(productCategory,pageNumber,pageSize,products);
        }
    }
}
