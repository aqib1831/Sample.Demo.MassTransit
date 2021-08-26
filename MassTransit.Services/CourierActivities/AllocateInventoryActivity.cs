using System;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace MassTransit.Services.CourierActivities
{
    public class AllocateInventoryActivity :
        IActivity<AllocateInventoryArguments, AllocateInventoryLog>
    {

        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArguments> context)
        {
            var orderId = context.Arguments.OrderId;

            var itemNumber = context.Arguments.ItemNumber;
            if (string.IsNullOrEmpty(itemNumber))
                throw new ArgumentNullException(nameof(itemNumber));

            var quantity = context.Arguments.Quantity;
            if (quantity <= 0.0m)
                throw new ArgumentNullException(nameof(quantity));

            var allocationId = NewId.NextGuid();


            return context.Completed(new { AllocationId = allocationId });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            return context.Compensated();
        }
    }
}
