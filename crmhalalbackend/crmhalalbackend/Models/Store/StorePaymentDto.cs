using System.Collections.Generic;
using CRMHalalBackEnd.Models.Payment;

namespace CRMHalalBackEnd.Models.Store
{
    public class StorePaymentDto
    {

        public string TenantId { get; set; }
        public List<PaymentMethodDto> PaymentMethods { get; set; }
    }
}