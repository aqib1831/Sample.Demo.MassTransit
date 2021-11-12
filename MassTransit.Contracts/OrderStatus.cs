using System;

namespace MassTransit.Contracts
{
    public interface OrderStatus
    {
        Guid OrderId { get; }

        string State { get; }
    }
}
