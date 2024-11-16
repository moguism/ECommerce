using Server.DTOs;

namespace Server.Models
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductToById { get; set; }
        public ProductsToBuy ProductsToBuy { get; set; }


    }
}
