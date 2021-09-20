using System;

namespace MassTransit.Contracts
{
    public interface CheckOrder
    {
        Guid OrderId { get; }
    }
}
