using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Order
{
    public DateTime CreatedAt { get; set; }

    public int IsReserved { get; set; }

    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
