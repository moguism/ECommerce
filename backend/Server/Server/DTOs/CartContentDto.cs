using Server.Models;

namespace Server.DTOs
{
    public class CartContentDto
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
