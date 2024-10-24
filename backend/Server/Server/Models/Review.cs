﻿using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Review
{
    public int Id { get; set; }

    public string Text { get; set; }

    public int Score { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; }

    public virtual User User { get; set; }
}
