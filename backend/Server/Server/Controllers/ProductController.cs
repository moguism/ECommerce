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
using System.Xml.Linq;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SmartSearchService _smartSearchService;
        private readonly UserService _userService;
        private readonly ProductService _productService;

        public ProductController(SmartSearchService smartSearchService, UserService userService, ProductService productService)
        {
            _smartSearchService = smartSearchService;
            _userService = userService;
            _productService = productService;
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

            //IEnumerable<Product> correctProducts = _productService.AddCorrectPath(products);
            return _productService.ToDto(products);
        }

        [HttpGet]
        public async Task<PagedDto> GetAllProducts([FromQuery] QueryDto query)
        {
            // 1) Busca 2) Ordena 3) Pagina

            IEnumerable<Product> products = await _smartSearchService.Search(query.Search, query.ProductType.ToString().ToLower());

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
           
            PagedDto pagedDto = _productService.GetAllProductsByCategory(query.ActualPage, query.ProductPageSize, products);
            //pagedDto.Products = _productService.AddCorrectPath(pagedDto.Products);

            return pagedDto;
        }

        [HttpGet("{id}")]
        public async Task<Product> GetProductById(int id)
        {
            Product product = await _productService.GetFullProductById(id);
            return product;
            //return _productService.AddCorrectPath(product);
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

                long price = Int64.Parse(newProduct.Price);
                int stock = Int32.Parse(newProduct.Stock);

                if (stock < 0 || price < 5)
                {
                    return null;
                }

                Product product = _productService.ToEntity(newProduct);
                switch (newProduct.CategoryName)
                {
                    case "Frutas":
                        product.CategoryId = 1;
                        break;
                    case "Verduras":
                        product.CategoryId = 2;
                        break;
                    case "Carne":
                        product.CategoryId = 3;
                        break;
                    default:
                        return null;
                }

                ImageService imageService = new ImageService();

                product.Image = "/" + await imageService.InsertAsync(newProduct.Image);
                Product savedProduct = await _productService.InsertProduct(product);
                return _productService.ToDto(savedProduct);
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

                long price = Int64.Parse(productToUpdate.Price);
                int stock = Int32.Parse(productToUpdate.Stock);

                if (stock < 0 || price < 5)
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
                product.Price = price;
                product.Stock = stock;
                
                switch (productToUpdate.CategoryName)
                {
                    case "Frutas":
                        product.CategoryId = 1;
                        break;
                    case "Verduras":
                        product.CategoryId = 2;
                        break;
                    case "Carne":
                        product.CategoryId = 3;
                        break;
                    default:
                        return null;
                }

                if (productToUpdate.Image != null)
                {
                    ImageService imageService = new ImageService();
                    product.Image = "/" + await imageService.InsertAsync(productToUpdate.Image);
                }
                
                await _productService.UpdateProduct(product);
                return _productService.ToDto(product);
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
