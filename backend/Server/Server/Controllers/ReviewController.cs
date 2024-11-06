using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Enums;
using Server.Repositories;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ReviewController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IEnumerable<Review>> GetAllProductReviews(int id)
        {
            IEnumerable<Review> reviews = await _unitOfWork.ReviewRepository.GetByProductIdAsync(id);

            return reviews;
        }

        [HttpGet]
        public async Task<IEnumerable<Review>> GetAllUserReviews(int id)
        {
            IEnumerable<Review> reviews = await _unitOfWork.ReviewRepository.GetByUserIdAsync(id);

            return reviews;
        }


    }
}
