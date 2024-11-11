﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Server.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace Server.Services
{
    public class ReviewService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _model;

        public ReviewService(UnitOfWork unitOfWork, PredictionEnginePool<ModelInput, ModelOutput> model)
        {
            _unitOfWork = unitOfWork;
            _model = model;
        }

        //Pruebas
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            IEnumerable<Review> reviews = await _unitOfWork.ReviewRepository.GetAllWithFullDataAsync();

            return reviews;
        }

        public async Task<IEnumerable<Review>> GetAllProductReviewsAsync(int id)
        {
            IEnumerable<Review> reviews = await _unitOfWork.ReviewRepository.GetByProductIdAsync(id);

            return reviews;
        }

        public async Task<IEnumerable<Review>> GetAllUserReviewsAsync(int id)
        {
            IEnumerable<Review> reviews = await _unitOfWork.ReviewRepository.GetByUserIdAsync(id);

            return reviews;
        }

        
        public ModelOutput Predict(string text)
        {
            ModelInput input = new ModelInput
            {
                Text = text
            };

            ModelOutput output = _model.Predict(input);

            return output;
        }

        
        public async Task AddReview(Review review)
        {
            //añade la review al usuario
            if (review.User != null)
            {
                review.User.Reviews.Add(review);
            }

            //Añade la review al producto
            if (review.Product != null)
            {
                review.Product.Reviews.Add(review);
            }

            await _unitOfWork.ReviewRepository.InsertAsync(review);
            await _unitOfWork.SaveAsync();
        }


        public async Task<Review> RateReview(Review review)
        {
            string finalText = Regex.Replace(review.Text, @"\s{2,}", " "); 
            finalText = _unitOfWork.ReviewRepository.DeleteAcents(finalText);
            ModelInput modelInput = new ModelInput { Text = finalText };
            ModelOutput modelOutput = Predict(finalText);

            review.Score = (int)modelOutput.PredictedLabel;

            //Almacena el producto como objeto por su ID
            review.Product = await _unitOfWork.ProductRepository.GetFullProductById(review.ProductId);

            return review;
        }
        
    }
}