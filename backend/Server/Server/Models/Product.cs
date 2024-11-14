using System;
using System.Collections.Generic;

namespace Server.Models;

public class Product
{
    public string Name { get; set; }

    public int Id { get; set; }

    public string Description { get; set; }

    public long Price { get; set; }

    public int Stock { get; set; }

    public double Average { get; set; }

    public string Image {  get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<CartContent> CartContents { get; set; } = new List<CartContent>();


}
