using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Enums;
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

        /*
        [HttpGet]
        public async Task<IEnumerable<Review>> GetAllReviews(Product product)
        {
            IEnumerable<Review> reviews;

            foreach (Review review in reviews)
            {
                
            }


            return reviews;
        }

        */
    }
}
