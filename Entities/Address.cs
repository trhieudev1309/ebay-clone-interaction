using System;
using System.Collections.Generic;

namespace EbayChat.Entities;

public partial class Address
{
    public int id { get; set; }

    public int? userId { get; set; }

    public string? fullName { get; set; }

    public string? phone { get; set; }

    public string? street { get; set; }

    public string? city { get; set; }

    public string? state { get; set; }

    public string? country { get; set; }

    public bool? isDefault { get; set; }

    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    public virtual User? user { get; set; }
}
