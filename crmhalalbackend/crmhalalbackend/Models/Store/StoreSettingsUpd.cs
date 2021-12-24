using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CRMHalalBackEnd.Models.Store
{
    public class StoreSettingsUpd
    {
        public bool? Status { get; set; }
        public bool? IsSales { get; set; }
        public List<bool> CheckedPaymentType { get; set; }
        public int? PaymentTypeBinary { get; set; }
        public int RefundDayLimit { get; set; }
    }
}