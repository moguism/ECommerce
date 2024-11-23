using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Enums;
using Server.Repositories;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Microsoft.Extensions.ML;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        private readonly ReviewService _reviewService;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly ReviewMapper _reviewMapper;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _model;
        private readonly ProductService _productService;


        public ReviewController(ReviewService reviewService, UnitOfWork unitOfWork, ReviewMapper reviewMapper, PredictionEnginePool<ModelInput, ModelOutput> model,
            ShoppingCartService shoppingCartService, ProductService productService)
        {
            _reviewService = reviewService;
            _reviewMapper = reviewMapper;
            _model = model;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
        }



        //Pruebas
        [HttpGet("AllReviews")]
        public async Task<IEnumerable<Review>> GetAllReviews()
        {
            IEnumerable<Review> reviews = await _reviewService.GetAllReviewsAsync();

            return reviews;
        }


        [HttpGet("Product/{id}")]
        public async Task<IEnumerable<Review>> GetReviewByProductId(int id)
        {
            return await _reviewService.GetAllProductReviewsAsync(id);
        }

        [Authorize]
        [HttpPost("AddReview")]
        public async Task AddReviewAsync([FromBody]ReviewDto reviewDto)
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                Console.WriteLine("No hay usuario");
                return;
            }

            Review review = _reviewMapper.ToEntity(reviewDto);
            review = _reviewService.RateReview(review);
            review.UserId = user.Id;
            review.DateTime = DateTime.UtcNow;

            Product product = await _productService.GetFullProductById(review.ProductId);

            List<Review> reviews = (List<Review>)await _reviewService.GetAllProductReviewsAsync(review.ProductId);
            reviews.Add(review);
            double averageScore = reviews.Average(r => r.Score);
            product.Average = averageScore;

            await _productService.UpdateProduct(product);

            //guarda la review con todos los datos
            await _reviewService.AddReview(review);
        }



        private async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _shoppingCartService.GetUserFromDbByStringId(idString);
        }

    }
}