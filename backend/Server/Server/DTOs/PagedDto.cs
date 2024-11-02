using Server.Models;

namespace Server.DTOs;

public class PagedDto
{
    public IEnumerable<Product> Products { get; set; }
    public int TotalProducts { get; set; }
}
