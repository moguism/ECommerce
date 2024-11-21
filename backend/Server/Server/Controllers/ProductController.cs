using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ProductDto> CreateProduct([FromForm] ProductToInsert newProduct)
        {
            try
            {
                User user = await GetAuthorizedUser();
                if (user == null || !user.Role.Equals("Admin"))
                {
                    return null;
                }
                Product product = _productMapper.ToEntity(newProduct);

                // TODO: Agregar lógica
                switch (newProduct.CategoryName)
                {

                }

                product.Image = await _imageService.InsertAsync(newProduct.Image);
                product.CategoryId = 1; // TODO: Cambiar
                Product savedProduct = await _unitOfWork.ProductRepository.InsertAsync(product);
                await _unitOfWork.SaveAsync();
                return _productMapper.ToDto(savedProduct);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Ha ocurrido una excepción:");
                Console.WriteLine(e);
                return null;
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<ProductDto> UpdateProduct([FromForm] ProductToInsert productToUpdate)
        {
            try
            {
                User user = await GetAuthorizedUser();
                if (user == null || !user.Role.Equals("Admin"))
                {
                    return null;
                }

                Product product = await _unitOfWork.ProductRepository.GetFullProductById(Int32.Parse(productToUpdate.Id));
                if (product == null)
                {
                    return null;
                }
                else
                {
                    _unitOfWork.Context.Entry(product).State = EntityState.Detached;
                }

                //product = _productMapper.ToEntity(productToUpdate);
                product.Name = productToUpdate.Name;
                product.Description = productToUpdate.Description;
                product.Price = Int64.Parse(productToUpdate.Price);
                product.Stock = Int32.Parse(productToUpdate.Stock);
                product.CategoryId = 1; // TODO: Cambiar

                if (productToUpdate.Image != null)
                {
                    product.Image = await _imageService.InsertAsync(productToUpdate.Image);
                }
                
                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveAsync();
                return _productMapper.ToDto(product);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Ha ocurrido una excepción:");
                Console.WriteLine(e);
                return null;
            }
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
