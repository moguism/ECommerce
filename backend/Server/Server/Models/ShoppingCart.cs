namespace Server.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

    }
}
