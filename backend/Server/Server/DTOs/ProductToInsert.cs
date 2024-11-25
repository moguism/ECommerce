using Server.Models;

namespace Server.DTOs;

public class ProductToInsert
{
    public string Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public string Price { get; set; }

    public string Stock { get; set; }

    public IFormFile Image { get; set; }

    public string CategoryName { get; set; }

}
