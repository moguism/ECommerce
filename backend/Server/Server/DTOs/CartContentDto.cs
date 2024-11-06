using Server.Models;

namespace Server.DTOs
{
    public class CartContentDto
    {
        public int Id { get; set; } //mismo id que el carrito
        public int CartContentId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
