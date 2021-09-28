using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTrasit.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CustomerController :
        ControllerBase
    {
        readonly IPublishEndpoint _publishEndpoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishEndpoint"></param>
        public CustomerController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id, string customerNumber)
        {
            await _publishEndpoint.Publish<CustomerAccountClosed>(new
            {
                CustomerId = id,
                CustomerNumber = customerNumber
            });

            return Ok();
        }
    }
}
