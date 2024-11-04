using Server.Models;

namespace Server.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
