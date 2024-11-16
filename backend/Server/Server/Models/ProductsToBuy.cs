namespace Server.Models
{
    public class ProductsToBuy
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }

        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }
    }
}
