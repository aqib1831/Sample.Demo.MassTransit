using MassTransit.Services.StateMachine;
using MassTransit.Testing;
using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Contracts;
using Xunit;

namespace MassTransit.Test
{
    public class OrderStateMachineTests
    {
        [Fact]
        public async Task Should_create_a_state_instance()
        {
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<OrderSubmittedEvent>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.True(saga.Created.Select(x => x.CorrelationId == orderId).Any());

                var instanceId = await saga.Exists(orderId, x => x.Submitted);
                Assert.NotNull(instanceId);

                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.Equal("12345", instance.CustomerNumber);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_respond_to_status_checks()
        {
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<OrderSubmittedEvent>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.True(saga.Created.Select(x => x.CorrelationId == orderId).Any());

                var instanceId = await saga.Exists(orderId, x => x.Submitted);

                Assert.NotNull(instanceId);

                var requestClient = await harness.ConnectRequestClient<CheckOrder>();

                var response = await requestClient.GetResponse<OrderStatus>(new { OrderId = orderId });

                Assert.Equal(response.Message.State, orderStateMachine.Submitted.Name);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_accept_when_order_is_accepted()
        {
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<OrderSubmittedEvent>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.True(saga.Created.Select(x => x.CorrelationId == orderId).Any());

                var instanceId = await saga.Exists(orderId, x => x.Submitted);
                Assert.NotNull(instanceId);

                await harness.Bus.Publish<OrderAccepted>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                });

                instanceId = await saga.Exists(orderId, x => x.Accepted);
                Assert.NotNull(instanceId);
            }
            finally
            {
                await harness.Stop();
            }
        }

    }
}
