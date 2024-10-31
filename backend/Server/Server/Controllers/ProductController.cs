using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Enums;
using Server.Mappers;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductMapper _productMapper;
        FarminhouseContext _context;

        public ProductController(UnitOfWork unitOfWork, FarminhouseContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetAllProducts(QueryDto query)
        {
            IEnumerable<Product> products;

            products = await _unitOfWork.ProductRepository.GetAllProductsByCategory(query.ProductType.ToString().ToLower());





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
