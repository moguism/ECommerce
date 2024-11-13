namespace Server.Models
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        //public int UserId { get; set; }
        public int PaymentType { get; set; }

        //public User User { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Finished { get; set; }
    }
}
