using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Payment
{
    public DateTime CreatedAt { get; set; }

    public int Id { get; set; }

    public double Total { get; set; }

    public int IsDone { get; set; }

    public int OrderId { get; set; }

    public int PaymentTypeId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PaymentsType PaymentType { get; set; } = null!;
}
