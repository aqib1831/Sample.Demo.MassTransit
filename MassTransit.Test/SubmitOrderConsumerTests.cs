using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
