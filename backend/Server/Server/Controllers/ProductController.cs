using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Mappers;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        FarminhouseContext _context;

        public ProductController(UnitOfWork unitOfWork, FarminhouseContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _unitOfWork.ProductRepository.GetAllAsync();
        }

        [HttpGet("vegetables")]
        public async Task<IEnumerable<Product>> GetAllVegetables()
        {
            return await _unitOfWork.ProductRepository.GetAllProductsByCategory("vegetables");
        }
        [HttpGet("fruits")]
        public async Task<IEnumerable<Product>> GetAllFruits()
        {
            return await _unitOfWork.ProductRepository.GetAllProductsByCategory("fruits");

        }
        [HttpGet("meat")]
        public async Task<IEnumerable<Product>> GetAllMeat()
        {
            return await _unitOfWork.ProductRepository.GetAllProductsByCategory("meat");

        }
    }
}
