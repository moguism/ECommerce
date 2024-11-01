using Server.Enums;

namespace Server.DTOs
{ 
    public class QueryDto
    {
        public ProductType ProductType { get; set; }
        public OrdinationType OrdinationType { get; set; }
        public OrdinationDirection OrdinationDirection { get; set; }
        public int ProductPageSize { get; set; }
        public int ActualPage { get; set; }
        public string Search { get; set; }
    }
}
