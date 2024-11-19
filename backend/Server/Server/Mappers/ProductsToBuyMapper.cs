using Server.DTOs;
using Server.Models;

namespace Server.Mappers
{
    public class ProductsToBuyMapper
    {

        public CartContentDto ToDto(ProductsToBuy productsToBuy)
        {

            return new CartContentDto
            {
                ProductId = productsToBuy.ProductId,
                Quantity = productsToBuy.Quantity,
            };
        }

        public IEnumerable<CartContentDto> ToDto(IEnumerable<ProductsToBuy> productsToBuy)
        {
            if(productsToBuy == null)
            {
                return null;
            }

            List<CartContentDto> result = new List<CartContentDto>();

            foreach (ProductsToBuy product in productsToBuy)
            {
                result.Add(ToDto(product));
            }
            return result;
        }


        public ProductsToBuy ToEntity(CartContentDto cartContentDto)
        {

            return new ProductsToBuy
            {
                ProductId = cartContentDto.ProductId,
                Quantity = cartContentDto.Quantity,
            };
        }

        public IEnumerable<ProductsToBuy> ToEntity(IEnumerable<CartContentDto> cartContentDtos)
        {

            List<ProductsToBuy> result = new List<ProductsToBuy>();

            foreach (CartContentDto product in cartContentDtos)
            {
                result.Add(ToEntity(product));
            }
            return result;
        }
    }
}
