using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Services.CourierActivities
{
    public interface AllocateInventoryArguments
    {
        Guid OrderId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}
