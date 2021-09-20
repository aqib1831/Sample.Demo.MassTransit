using System;

namespace MassTransit.Contracts
{
    public interface OrderNotFound
    {
        Guid OrderId { get; }
    }
}
