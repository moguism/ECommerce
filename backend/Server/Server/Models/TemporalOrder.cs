﻿using Server.DTOs;

namespace Server.Models
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool Quick { get; set; }
        public string HashOrSession { get; set; }
    }
}
