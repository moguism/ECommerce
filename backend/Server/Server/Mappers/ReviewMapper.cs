using Server.DTOs;
using Server.Models;

namespace Server.Mappers
{
    public class ReviewMapper
    {
        public ReviewDto ToDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                Text = review.Text,
                Score = review.Score,
                UserId = review.UserId,
                ProductId = review.ProductId
            };
        }
        public Review ToEntity(ReviewDto reviewDto)
        {
            return new Review
            {
                Id = reviewDto.Id,
                Text = reviewDto.Text,
                Score = reviewDto.Score,
                UserId = reviewDto.UserId,
                ProductId = reviewDto.ProductId
            };
        }

    }
}
