using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Contracts
{
    public interface CustomerAccountClosed
    {
        Guid CustomerId { get; }
        string CustomerNumber { get; }
    }
}
