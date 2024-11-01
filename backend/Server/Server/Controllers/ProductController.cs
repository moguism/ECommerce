using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Enums;
using Server.Mappers;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductMapper _productMapper;
        private readonly SmartSearchService _smartSearchService;

        public ProductController(UnitOfWork unitOfWork, ProductMapper productmapper, SmartSearchService smartSearchService)
        {
            _unitOfWork = unitOfWork;
            _productMapper = productmapper;
            _smartSearchService = smartSearchService;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetAllProducts([FromQuery] QueryDto query)
        {
            // 1) Busca 2) Ordena 3) Pagina

            IEnumerable<Product> products = await _smartSearchService.Search(query.Search);

            switch (query.OrdinationType)
            {
                case OrdinationType.NAME:
                    products = query.OrdinationDirection == OrdinationDirection.ASC
                        ? products.OrderBy(product => product.Name)
                        : products.OrderByDescending(product => product.Name);
                    break;
                case OrdinationType.PRICE:
                    products = query.OrdinationDirection == OrdinationDirection.ASC
                        ? products.OrderBy(product => product.Price)
                        : products.OrderByDescending(product => product.Price);
                    break;
            }

            string productType = query.ProductType.ToString().ToLower();

            products = _unitOfWork.ProductRepository.GetAllProductsByCategory(productType, query.ActualPage, query.ProductPageSize, products);

            return _productMapper.AddCorrectPath(products);
         }

        /*
        [HttpGet("vegetables")]
        public async Task<IEnumerable<Product>> GetAllVegetables()
        {
            IEnumerable<Product> products = await _unitOfWork.ProductRepository.GetAllProductsByCategory("vegetables");
            return _productMapper.AddCorrectPath(products);
        }
        [HttpGet("fruits")]
        public async Task<IEnumerable<Product>> GetAllFruits()
        {
            IEnumerable<Product> products = await _unitOfWork.ProductRepository.GetAllProductsByCategory("fruits");
            return _productMapper.AddCorrectPath(products);
        }
        [HttpGet("meat")]
        public async Task<IEnumerable<Product>> GetAllMeat()
        {
            IEnumerable<Product> products = await _unitOfWork.ProductRepository.GetAllProductsByCategory("meat");
            return _productMapper.AddCorrectPath(products);
        }
        */
    }
}
