using Automatonymous;
using MassTransit.Saga;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
namespace MassTransit.Services.StateMachine
{
    public class OrderState :
        SagaStateMachineInstance,
        ISagaVersion
    {
        public string CurrentState { get; set; }

        public string CustomerNumber { get; set; }
        public string PaymentCardNumber { get; set; }

        public string FaultReason { get; set; }

        public DateTime? SubmitDate { get; set; }
        public DateTime? Updated { get; set; }

        public int Version { get; set; }

        [BsonId]
        public Guid CorrelationId { get; set; }
    }
}