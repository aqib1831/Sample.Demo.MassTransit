using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Contracts
{
    public interface OrderFulfillmentCompleted
    {
        Guid OrderId { get; }

        DateTime Timestamp { get; }
    }
}
