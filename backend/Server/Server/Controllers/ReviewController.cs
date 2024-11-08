using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Enums;
using Server.Repositories;
using Server.Models;
using Server.Services;
using Microsoft.Extensions.ML;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("AllProductReviews")]
        public async Task<IEnumerable<Review>> GetAllProductReviews(int id)
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

        [HttpPost]
        public async Task AddReviewAsync([FromBody] Review review)
        {
            await  _reviewService.RateReview(review);
        }
    }
}