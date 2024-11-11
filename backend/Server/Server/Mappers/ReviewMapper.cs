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
                Text = review.Text,
                ProductId = review.ProductId
            };
        }
        public Review ToEntity(ReviewDto reviewDto)
        {
            return new Review
            {
                Text = reviewDto.Text,
                ProductId = reviewDto.ProductId
            };
        }

    }
}
