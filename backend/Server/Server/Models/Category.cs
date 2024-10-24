using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Server.Models;

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    public string Name { get; set; }

    public int Id { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
