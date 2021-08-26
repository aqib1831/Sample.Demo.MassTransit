using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Contracts
{
    public interface CheckOrder
    {
        Guid OrderId { get; }
    }
}
