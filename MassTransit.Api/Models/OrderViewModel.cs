using System;
using System.ComponentModel.DataAnnotations;

namespace MassTrasit.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string CustomerNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string PaymentCardNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Notes { get; set; }
    }
}
