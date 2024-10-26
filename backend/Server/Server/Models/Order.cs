using System;
using System.Collections.Generic;

namespace Server.Models;

public class Order
{
    public DateTime CreatedAt { get; set; }

    public int IsReserved { get; set; }

    public int Id { get; set; }

    public int UserId { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public User User { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
