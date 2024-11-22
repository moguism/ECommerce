using Server.Models;

namespace Server.DTOs
{
    public class TemporalOrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public IEnumerable<CartContentDto> CartContentDtos  { get; set; }

        public bool Quick { get; set; }
        public string SessionId { get; set; }
    }
}
