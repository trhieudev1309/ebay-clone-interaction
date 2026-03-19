using System;
using System.Collections.Generic;

namespace EbayChat.Entities;

public partial class Category
{
    public int id { get; set; }

    public string? name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
