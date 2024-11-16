namespace Server.Models
{
    public class ProductsToBuy
    {

        public int Id { get; set; }

        public IEnumerable<CartContent> CartContents { get; set; } = new List<CartContent>();
    }
}
