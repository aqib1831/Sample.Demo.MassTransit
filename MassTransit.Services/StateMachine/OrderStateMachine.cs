using System;
using Automatonymous;
using MassTransit.Contracts;

namespace MassTransit.Services.StateMachine
{
    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderAccepted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => FulfillmentCompleted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => FulfillmentFaulted, x => x.CorrelateById(m => m.Message.OrderId));
            //Event(() => FulfillOrderFaulted, x => x.CorrelateById(m => m.Message.Message.OrderId));
            //Event(() => FulfillOrderFault, x => x.CorrelateById(m => m.Message.Message.OrderId));
            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
                    }
                }));
            });
            Event(() => AccountClosed, x => x.CorrelateBy((saga, context) => saga.CustomerNumber == context.Message.CustomerNumber));

            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Instance.SubmitDate = context.Data.Timestamp;
                        context.Instance.CustomerNumber = context.Data.CustomerNumber;
                        context.Instance.PaymentCardNumber = context.Data.PaymentCardNumber;

                        context.Instance.Updated = DateTime.UtcNow;
                    })
                    .TransitionTo(Submitted));

            During(Submitted,
                Ignore(OrderSubmitted),
                When(AccountClosed)
                    .TransitionTo(Canceled),
                When(OrderAccepted).Then(context =>
                {
                    context.Instance.SubmitDate = context.Data.Timestamp;
                })
                    .Activity(x => x.OfType<AcceptOrderActivity>())
                    .TransitionTo(Accepted));

            During(Accepted,
                //When(FulfillOrderFaulted)
                //    .Then(context =>
                //    {
                //        Console.WriteLine("Fulfill Order Faulted: {0}", context.Data.Exceptions.FirstOrDefault()?.Message);
                //    })
                //    .TransitionTo(Faulted),
                When(FulfillmentFaulted)
                    .Then(context =>
                    {
                        Console.WriteLine("Fulfill Order Faulted: {0}");
                    })
                    .TransitionTo(Faulted),
                When(FulfillmentCompleted)
                    .Then(context =>
                    {
                        Console.WriteLine("Fulfill Order Faulted: {0}");
                    })
                    .TransitionTo(Completed));

            DuringAny(
                When(OrderStatusRequested)
                    .RespondAsync(x => x.Init<OrderStatus>(new
                    {
                        OrderId = x.Instance.CorrelationId,
                        State = x.Instance.CurrentState
                    }))
            );

            DuringAny(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Instance.SubmitDate ??= context.Data.Timestamp;
                        context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
                    })
             );
        }

        public State Submitted { get; private set; }
        public State Accepted { get; private set; }
        public State Canceled { get; private set; }
        public State Faulted { get; private set; }
        public State Completed { get; private set; }

        public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }
        public Event<OrderFulfillmentCompleted> FulfillmentCompleted { get; private set; }
        public Event<OrderFulfillmentFaulted> FulfillmentFaulted { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }
        public Event<CustomerAccountClosed> AccountClosed { get; private set; }

        //public Event<Fault<FulfillOrder>> FulfillOrderFaulted { get; private set; }

        //public Event<Fault<OrderSubmitted>> FulfillOrderFault { get; private set; }
    }
}