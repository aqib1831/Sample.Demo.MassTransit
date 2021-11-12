using System;

namespace MassTransit.Contracts
{
    public interface OrderFulfillmentCompleted
    {
        Guid OrderId { get; }

        DateTime Timestamp { get; }
    }
}
