using System;
using System.Collections.Generic;

namespace Server.Models;

public class PaymentsType
{
    public string Name { get; set; }

    public int Id { get; set; }

    public ICollection<TemporalOrder> TemporalOrders { get; set; } = new List<TemporalOrder>();
}
