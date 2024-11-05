namespace Server.Models
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public int userId { get; set; }
        public int paymentType { get; set; }

    }
}
