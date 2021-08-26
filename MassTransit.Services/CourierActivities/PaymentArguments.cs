using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Services.CourierActivities
{
    public interface PaymentArguments
    {
        Guid OrderId { get; }
        decimal Amount { get; }
        string CardNumber { get; }
    }
}
