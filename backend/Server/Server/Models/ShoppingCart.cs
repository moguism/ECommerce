using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int UserId { get; set; }

    }
}
