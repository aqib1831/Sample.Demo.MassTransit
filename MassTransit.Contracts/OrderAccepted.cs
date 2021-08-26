using System;

namespace MassTransit.Contracts
{
    public interface OrderAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}
