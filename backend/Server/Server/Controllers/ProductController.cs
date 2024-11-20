using Microsoft.AspNetCore.Authorization;
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
        private readonly UserService _userService;
        private readonly ImageService _imageService;

        public ProductController(UnitOfWork unitOfWork, ProductMapper productmapper, SmartSearchService smartSearchService, UserService userService, ImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _productMapper = productmapper;
            _smartSearchService = smartSearchService;
            _userService = userService;
            _imageService = imageService;
        }

        [Authorize]
        [HttpGet("complete")]
        public async Task<IEnumerable<ProductDto>> GetCompleteProducts()
        {
            User user = await GetAuthorizedUser();

            if (user == null || !user.Role.Equals("Admin"))
            {
                return null;
            }

            IEnumerable<Product> products = await _unitOfWork.ProductRepository.GetFullProducts();

            return _productMapper.ToDto(products);
        }

        [HttpGet]
        public async Task<PagedDto> GetAllProducts([FromQuery] QueryDto query)
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

            PagedDto pagedDto = _unitOfWork.ProductRepository.GetAllProductsByCategory(productType, query.ActualPage, query.ProductPageSize, products);
            pagedDto.Products = _productMapper.AddCorrectPath(pagedDto.Products);

            return pagedDto;
        }

        [HttpGet("{id}")]
        public async Task<Product> GetProductById(int id)
        {
            Product product = await _unitOfWork.ProductRepository.GetFullProductById(id);
            foreach (Review review in product.Reviews)
            {
                User user = await _unitOfWork.UserRepository.GetByIdAsync(review.UserId);
                review.User = new User()
                {
                    Name = user.Name
                };
            }
            return _productMapper.AddCorrectPath(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<ProductDto> CreateProduct([FromBody] ProductToInsert newProduct)
        {
            User user = await GetAuthorizedUser();
            if(user == null || !user.Role.Equals("Admin"))
            {
                return null;
            }
            Product product = _productMapper.ToEntity(newProduct);
            product.Image = await _imageService.InsertAsync(newProduct.Image);
            Product savedProduct = await _unitOfWork.ProductRepository.InsertAsync(product);
            await _unitOfWork.SaveAsync();
            return _productMapper.ToDto(savedProduct);
        }

        [Authorize]
        [HttpPut]
        public async Task UpdateProduct([FromBody] ProductToInsert productToUpdate)
        {
            User user = await GetAuthorizedUser();
            if (user == null || !user.Role.Equals("Admin"))
            {
                return;
            }

            Product product = await _unitOfWork.ProductRepository.GetFullProductById(productToUpdate.Id);
            if (product == null)
            {
                return;
            }

            product = _productMapper.ToEntity(productToUpdate);
            product.Image = await _imageService.InsertAsync(productToUpdate.Image);
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveAsync();
        }

        private async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _userService.GetUserFromDbByStringId(idString);
        }

    }
}
