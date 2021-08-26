﻿using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Services
{
    public class RoutingSlipBatchEventConsumer :
        IConsumer<Batch<RoutingSlipCompleted>>
    {
        readonly ILogger<RoutingSlipBatchEventConsumer> _logger;

        public RoutingSlipBatchEventConsumer(ILogger<RoutingSlipBatchEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Batch<RoutingSlipCompleted>> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.Log(LogLevel.Information, "Routing Slips Completed: {TrackingNumbers}",
                    string.Join(", ", context.Message.Select(x => x.Message.TrackingNumber)));
            }

            return Task.CompletedTask;
        }
    }
}
