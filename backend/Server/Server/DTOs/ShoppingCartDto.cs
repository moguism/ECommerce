using Server.Models;

namespace Server.DTOs;

public class ShoppingCartDto
{
    public int Id { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }

}
