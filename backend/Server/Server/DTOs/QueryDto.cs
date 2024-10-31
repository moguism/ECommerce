using Server.Enums;

namespace Server.DTOs
{
    public class QueryDto
    {
        public ProductType ProductType { get; set; }
        public OrdinationType OrdinationType { get; set; }
        public OrdinationDirection OrdinationDirection { get; set; }
    }
}
