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
        private readonly ReviewMapper _reviewMapper;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _model;


        public ReviewController(ReviewService reviewService, UnitOfWork unitOfWork, ReviewMapper reviewMapper, PredictionEnginePool<ModelInput, ModelOutput> model)
        {
            _reviewService = reviewService;
            _reviewMapper = reviewMapper;
            _model = model;
        }

        [HttpGet("AllProductReviews")]
        public async Task<IEnumerable<Review>> GetAllProductReviews([FromBody] int id)
        {
            IEnumerable<Review> reviews = await _reviewService.GetAllProductReviewsAsync(id);

            return reviews;
        }

        [HttpGet("AllUserReviews")]
        public async Task<IEnumerable<Review>> GetAllUserReviews(int id)
        {
            IEnumerable<Review> reviews = await _reviewService.GetAllUserReviewsAsync(id);

            return reviews;
        }

        //Pruebas
        [HttpGet("AllReviews")]
        public async Task<IEnumerable<Review>> GetAllReviews()
        {
            IEnumerable<Review> reviews = await _reviewService.GetAllReviewsAsync();

            return reviews;
        }


        [HttpGet("{id}")]
        public async Task<IEnumerable<Review>> GetReviewByProductId(int id)
        {
            return await _reviewService.GetAllProductReviewsAsync(id);
        }

        [Authorize]
        [HttpPost]
        public async Task AddReviewAsync([FromBody] ReviewDto reviewDto)
        {
            Review review = _reviewMapper.ToEntity(reviewDto);
            await  _reviewService.RateReview(review);
        }

        [HttpGet]
        public ModelOutput Predict(string text)
        {
            ModelInput input = new ModelInput
            {
                Text = text
            };

            ModelOutput output = _model.Predict(input);

            return output;
        }

    }
}