using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Enums;
using Server.Repositories;
using Server.Models;
using Microsoft.Extensions.ML;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _model;

        public ReviewController(UnitOfWork unitOfWork, PredictionEnginePool<ModelInput, ModelOutput> model)
        {
            _unitOfWork = unitOfWork;
            _model = model;
        }

        [HttpGet("AllProductReviews")]
        public async Task<IEnumerable<Review>> GetAllProductReviews(int id)
        {
            IEnumerable<Review> reviews = await _unitOfWork.ReviewRepository.GetByProductIdAsync(id);

            return reviews;
        }

        [HttpGet("AllUserReviews")]
        public async Task<IEnumerable<Review>> GetAllUserReviews(int id)
        {
            IEnumerable<Review> reviews = await _unitOfWork.ReviewRepository.GetByUserIdAsync(id);

            return reviews;
        }

        [HttpGet("Predict")]
        public ModelOutput Predict(string text)
        {
            ModelInput input = new ModelInput
            {
                Text = text
            };

            ModelOutput output = _model.Predict(input);

            return output;
        }

        [HttpPost]
        public async Task AddReviewAsync([FromBody] Review review)
        {
            string finalText = Regex.Replace(text, @"\s{2,}", " "); // Si tiene más de un espacio lo combierte en uno solo
            finalText = _unitOfWork.ReviewRepository.DeleteAcents(finalText);
            review.Text = text;
            ModelOutput modelOutput = Predict(finalText);
            review.Score = (int) modelOutput.PredictedLabel;
            await _unitOfWork.ReviewRepository.InsertAsync(review);

            await _unitOfWork.SaveAsync();
        }

    }
}
