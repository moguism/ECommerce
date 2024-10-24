using System;
using System.Collections.Generic;

namespace Server.Models;

public class PaymentsType
{
    public string Name { get; set; }

    public int Id { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
