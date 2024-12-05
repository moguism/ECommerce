using Server.Models;
using System.Numerics;

namespace Server.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PaymentTypeId { get; set; }
        public int UserId { get; set; }
       
        public IEnumerable<CartContentDto> Products { get; set; } = new List<CartContentDto>();
        
        public long Total { get; set; }
        public decimal TotalETH { get; set; }

    }
}
