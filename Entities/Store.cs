using System;
using System.Collections.Generic;

namespace EbayChat.Entities;

public partial class Store
{
    public int id { get; set; }

    public int? sellerId { get; set; }

    public string? storeName { get; set; }

    public string? description { get; set; }

    public string? bannerImageURL { get; set; }

    public virtual User? seller { get; set; }
}
