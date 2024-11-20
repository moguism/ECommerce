namespace Server.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public IEnumerable<ProductsToBuy> Products { get; set; } = new List<ProductsToBuy>();

    }
}
