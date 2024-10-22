using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Product
{
    public string Name { get; set; } = null!;

    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public int Stock { get; set; }

    public double Average { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Order> Products { get; set; } = new List<Order>();
}
