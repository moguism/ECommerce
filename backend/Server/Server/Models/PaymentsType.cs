using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class PaymentsType
{
    public string Name { get; set; } = null!;

    public int Id { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
