using Server.Models;

namespace Server.DTOs
{
    public class ShoppingCartDto
    {
        public int Id { get; set; }
        public User User { get; set; }
        public IEnumerable<CartContent> CartContent { get; set; }
    }
}
