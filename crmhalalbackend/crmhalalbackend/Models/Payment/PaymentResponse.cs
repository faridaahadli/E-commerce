using System;
using System.Collections.Generic;

namespace CRMHalalBackEnd.Models.Payment
{
    public class PaymentResponse
    {
        public string PaymentMessage { get; set; }
        public bool PaymentStatus { get; set; }
        public string UserFullName { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public List<PaymentDetails> PaymentDetails { get; set; }

    }
}