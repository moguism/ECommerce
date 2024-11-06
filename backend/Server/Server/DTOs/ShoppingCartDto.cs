using Server.Models;

namespace Server.DTOs;

public class ShoppingCartDto
{
    public int Id { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<int> Quantity { get; set; } = new List<int>();

}
