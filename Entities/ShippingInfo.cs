using System;
using System.Collections.Generic;

namespace EbayChat.Entities;

public partial class ShippingInfo
{
    public int id { get; set; }

    public int? orderId { get; set; }

    public string? carrier { get; set; }

    public string? trackingNumber { get; set; }

    public string? status { get; set; }

    public DateTime? estimatedArrival { get; set; }

    public virtual OrderTable? order { get; set; }
}
