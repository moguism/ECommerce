using System;
using System.Collections.Generic;

namespace Server.Models;

public class Product
{
    public string Name { get; set; }

    public int Id { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public int Stock { get; set; }

    public double Average { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; }

    public string Image { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
