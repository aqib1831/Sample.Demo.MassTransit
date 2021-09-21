using System;

namespace MassTransit.Services.CourierActivities
{
    public interface AllocateInventoryLog
    {
        Guid AllocationId { get; }
    }
}
