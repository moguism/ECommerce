using System;
using System.Collections.Generic;

namespace Server.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    public int PaymentTypeId { get; set; }
    public PaymentsType PaymentsType { get; set; }
    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; }

}
