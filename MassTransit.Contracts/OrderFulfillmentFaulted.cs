using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Contracts
{
    public interface OrderFulfillmentFaulted
    {
        Guid OrderId { get; }

        DateTime Timestamp { get; }
    }
}
