using System;
using System.Collections.Generic;

namespace Server.Models;

public class Payment
{
    public DateTime CreatedAt { get; set; }

    public int Id { get; set; }

    public double Total { get; set; }

    public int IsDone { get; set; }

    public int OrderId { get; set; }

    public int PaymentTypeId { get; set; }

    public Order Order { get; set; }

    public PaymentsType PaymentType { get; set; }
}
