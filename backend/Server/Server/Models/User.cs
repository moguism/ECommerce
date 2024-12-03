using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string Role { get; set; }

    public string Address { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<TemporalOrder> TemporalOrders { get; set; } = new List<TemporalOrder>();
    public ICollection<Order> Orders { get; set;} = new List<Order>();
    public ShoppingCart ShoppingCart { get; set; }
}