using Server.Models;

namespace Server.DTOs
{
    public class TemporalOrderDto
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public int UserId { get; set; }
    }
}
