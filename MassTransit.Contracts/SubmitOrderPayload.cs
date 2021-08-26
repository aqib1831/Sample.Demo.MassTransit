using System;

namespace MassTransit.Contracts
{
    public interface SubmitOrderPayload
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
        MessageData<string> Notes { get; }
    }
}
