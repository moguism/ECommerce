using Server.Models;
using Server.DTOs;
using Server.Mappers;

namespace Server.Services
{
    public class ProductService
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly ProductMapper _productMapper;

        public ProductService(UnitOfWork unitOfWork, ProductMapper productMapper)
        {
            _unitOfWork = unitOfWork;
            _productMapper = productMapper;
        }

        public async Task<Product> GetProductById(int id)
        {
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
            return await _unitOfWork.ProductRepository.GetAllAsync();
        }

        public PagedDto GetAllProductsByCategory(int pageNumber, int pageSize, IEnumerable<Product> products)
        {
            return _unitOfWork.ProductRepository.GetAllProductsByCategory(pageNumber,pageSize,products);
        }

        public IEnumerable<Product> AddCorrectPath(IEnumerable<Product> products)
        {
            return _productMapper.AddCorrectPath(products);
        }

        public Product AddCorrectPath(Product product)
        {
            return _productMapper.AddCorrectPath(product);
        }

        public IEnumerable<ProductDto> ToDto(IEnumerable<Product> products)
        {
            return _productMapper.ToDto(products);
        }

        public ProductDto ToDto(Product product)
        {
            return _productMapper.ToDto(product);
        }

        public IEnumerable<Product> ToEntity(IEnumerable<ProductDto> products)
        {
            return _productMapper.ToEntity(products);
        }

        public Product ToEntity(ProductDto product)
        {
            return _productMapper.ToEntity(product);
        }

        public Product ToEntity(ProductToInsert product)
        {
            return _productMapper.ToEntity(product);
        }
    }
}
