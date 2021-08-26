using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Services.CourierActivities
{
    public interface PaymentLog
    {
        string AuthorizationCode { get; }
    }
}
