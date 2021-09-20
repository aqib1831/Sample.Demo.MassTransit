using System;

namespace MassTransit.Contracts
{
    public interface CustomerAccountClosed
    {
        Guid CustomerId { get; }
        string CustomerNumber { get; }
    }
}
