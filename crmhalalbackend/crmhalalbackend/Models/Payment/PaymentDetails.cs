namespace CRMHalalBackEnd.Models.Payment
{
    public class PaymentDetails
    {
        public int PaymentTypeId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentType { get; set; }
        public string PaymentResponseMessage { get; set; }
        public string PaymentResponseCode { get; set; }
    }
}