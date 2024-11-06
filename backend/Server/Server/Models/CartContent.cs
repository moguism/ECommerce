namespace Server.Models
{
    public class CartContent
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ShoppingCartId { get; set; }

        public Product Product { get; set; }

    }
}
