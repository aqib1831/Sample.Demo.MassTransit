using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Contracts
{
    public interface OrderStatus
    {
        Guid OrderId { get; }

        string State { get; }
    }
}
