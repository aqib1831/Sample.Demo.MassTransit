using System;

namespace MassTransit.Contracts
{
    public interface OrderFulfillmentFaulted
    {
        Guid OrderId { get; }

        DateTime Timestamp { get; }
    }
}
