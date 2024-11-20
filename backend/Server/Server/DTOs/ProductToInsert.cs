using Server.Models;

namespace Server.DTOs;

public class ProductToInsert
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public long Price { get; set; }

    public int Stock { get; set; }

    public IFormFile Image { get; set; }

    public int CategoryId { get; set; }

}
