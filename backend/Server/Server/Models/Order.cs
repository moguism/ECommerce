using System;
using System.Collections.Generic;

namespace Server.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    //public decimal Total { get; set; }
    //public int UserId { get; set; }
    //public int PaymentTypeId { get; set; }
    //public int ShoppingCartId { get; set; }
    public int TemporalOrderId { get; set; }

    public TemporalOrder TemporalOrder { get; set; }

}
