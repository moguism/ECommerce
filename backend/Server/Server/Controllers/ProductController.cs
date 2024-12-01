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
        private readonly ProductMapper _productMapper;
        private readonly SmartSearchService _smartSearchService;
        private readonly UserService _userService;
        private readonly ImageService _imageService;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductController(ProductMapper productmapper, SmartSearchService smartSearchService, UserService userService, ImageService imageService, ProductService productService, CategoryService categoryService)
        {
            _productMapper = productmapper;
            _smartSearchService = smartSearchService;
            _userService = userService;
            _imageService = imageService;
            _productService = productService;
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("complete")]
        public async Task<IEnumerable<ProductDto>> GetCompleteProducts()
        {
            User user = await GetAuthorizedUser();

            if (user == null)
            {
                return null;
            }

            IEnumerable<Product> products = await _productService.GetFullProducts();

            IEnumerable<Product> correctProducts = _productMapper.AddCorrectPath(products);
            return _productMapper.ToDto(correctProducts);
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
            
            PagedDto pagedDto = _productService.GetAllProductsByCategory(productType, query.ActualPage, query.ProductPageSize, products);
            pagedDto.Products = _productMapper.AddCorrectPath(pagedDto.Products);

            return pagedDto;
        }

        [HttpGet("{id}")]
        public async Task<Product> GetProductById(int id)
        {
            Product product = await _productService.GetFullProductById(id);
            foreach (Review review in product.Reviews)
            {
                User user = await _userService.GetUserByIdAsync(review.UserId);
                review.User = new User()
                {
                    Name = user.Name
                };
            }
            return _productMapper.AddCorrectPath(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ProductDto> CreateProduct([FromForm] ProductToInsert newProduct)
        {
            try
            {
                User user = await GetAuthorizedUser();
                if (user == null)
                {
                    return null;
                }
                Product product = _productMapper.ToEntity(newProduct);

                Category category = await _categoryService.GetByName(newProduct.CategoryName);
                if(category == null)
                {
                    return null;
                }

                product.CategoryId = category.Id;

                product.Image = await _imageService.InsertAsync(newProduct.Image);
                Product savedProduct = await _productService.InsertProduct(product);
                return _productMapper.ToDto(savedProduct);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Ha ocurrido una excepción:");
                Console.WriteLine(e);
                return null;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ProductDto> UpdateProduct([FromForm] ProductToInsert productToUpdate)
        {
            try
            {
                User user = await GetAuthorizedUser();
                if (user == null)
                {
                    return null;
                }

                Product product = await _productService.GetProductById(Int32.Parse(productToUpdate.Id));
                if (product == null)
                {
                    return null;
                }

                //product = _productMapper.ToEntity(productToUpdate);
                product.Name = productToUpdate.Name;
                product.Description = productToUpdate.Description;
                product.Price = Int64.Parse(productToUpdate.Price);
                product.Stock = Int32.Parse(productToUpdate.Stock);
                Category category = await _categoryService.GetByName(productToUpdate.CategoryName);
                if (category == null)
                {
                    return null;
                }

                product.CategoryId = category.Id;

                if (productToUpdate.Image != null)
                {
                    product.Image = await _imageService.InsertAsync(productToUpdate.Image);
                }
                
                await _productService.UpdateProduct(product);
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
