namespace CRMHalalBackEnd.Models
{
    public class PaymentCompleteResponse
    {

        public string RESULT { get; set; }
        public string RESULT_PS { get; set; }
        public string RESULT_CODE { get; set; }
        public string ThreeDSecure { get; set; }
        public string RRN { get; set; }
        public string APPROVAL_CODE { get; set; }
        public string CARD_NUMBER { get; set; }
        public string RECC_PMNT_ID { get; set; }
        public string RECC_PMNT_EXPIRY { get; set; }

    }
}