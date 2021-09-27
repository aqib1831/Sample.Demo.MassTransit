using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Contracts;
using MassTransit.Services.Consumers;
using MassTransit.Testing;
using Xunit;

namespace MassTransit.Test
{
    public class SubmitOrderConsumerTests
    {
        [Fact]
        public async Task Should_respond_with_acceptance_if_ok()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                var requestClient = await harness.ConnectRequestClient<SubmitOrderPayload>();

                var response = await requestClient.GetResponse<OrderSubmissionAccepted>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.Equal(response.Message.OrderId, orderId);

                Assert.True(consumer.Consumed.Select<SubmitOrderPayload>().Any());

                Assert.True(await harness.Sent.Any<OrderSubmissionAccepted>());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_respond_with_rejected_if_test()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                var requestClient = await harness.ConnectRequestClient<SubmitOrderPayload>();

                var response = await requestClient.GetResponse<OrderSubmissionRejected>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "TEST123"
                });

                Assert.Equal(response.Message.OrderId, orderId);

                Assert.True(consumer.Consumed.Select<SubmitOrderPayload>().Any());

                Assert.True(harness.Sent.Select<OrderSubmissionRejected>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_consume_submit_order_commands()
        {
            var harness = new InMemoryTestHarness { TestTimeout = TimeSpan.FromSeconds(5) };
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrderPayload>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.True(consumer.Consumed.Select<SubmitOrderPayload>().Any());

                Assert.False(harness.Sent.Select<OrderSubmissionAccepted>().Any());
                Assert.False(harness.Sent.Select<OrderSubmissionRejected>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_not_publish_order_submitted_event_when_rejected()
        {
            var harness = new InMemoryTestHarness { TestTimeout = TimeSpan.FromSeconds(5) };
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrderPayload>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "TEST123"
                });

                Assert.True(consumer.Consumed.Select<SubmitOrderPayload>().Any());

                Assert.False(harness.Published.Select<OrderSubmittedEvent>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_publish_order_submitted_event()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrderPayload>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.True(await harness.Published.Any<OrderSubmittedEvent>());
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
