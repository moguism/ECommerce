using Server.DTOs;

namespace Server.Models
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }


    }
}
