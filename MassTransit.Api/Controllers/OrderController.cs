using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Contracts;
using MassTrasit.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MassTrasit.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly IRequestClient<SubmitOrderPayload> _submitOrderRequestClient;
        readonly IPublishEndpoint _publishEndpoint;
        readonly IRequestClient<CheckOrder> _checkOrderClient;
        readonly ISendEndpointProvider _sendEndpointProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="submitOrderRequestClient"></param>
        /// <param name="publishEndpoint"></param>
        /// <param name="checkOrderClient"></param>
        /// <param name="sendEndpointProvider"></param>
        public OrderController(IRequestClient<SubmitOrderPayload> submitOrderRequestClient, IPublishEndpoint publishEndpoint, IRequestClient<CheckOrder> checkOrderClient, ISendEndpointProvider sendEndpointProvider)
        {
            _submitOrderRequestClient = submitOrderRequestClient;
            _publishEndpoint = publishEndpoint;
            _checkOrderClient = checkOrderClient;
            _sendEndpointProvider = sendEndpointProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var (status, notFound) = await _checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new { OrderId = id });

            if (status.IsCompletedSuccessfully)
            {
                var response = await status;
                return Ok(response.Message);
            }
            else
            {
                var response = await notFound;
                return NotFound(response.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(OrderViewModel model)
        {
            var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = model.Id,
                InVar.Timestamp,
                model.CustomerNumber,
                model.PaymentCardNumber,
                model.Notes
            });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;

                return Accepted(response);
            }

            if (accepted.IsCompleted)
            {
                await accepted;

                return Problem("Order was not accepted");
            }
            else
            {
                var response = await rejected;

                return BadRequest(response.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(Guid id)
        {
            await _publishEndpoint.Publish<OrderAccepted>(new
            {
                OrderId = id,
                InVar.Timestamp,
            });

            return Accepted();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("OrderConfirm")]
        public async Task<IActionResult> Confirm(OrderViewModel model)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:fulfill-order"));

            await endpoint.Send<FulfillOrder>(new
            {
                OrderId = model.Id,
                InVar.Timestamp,
                model.CustomerNumber,
                model.PaymentCardNumber
            });

            return Accepted();
        }



    }
}
