namespace CRMHalalBackEnd.Models
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"{nameof(PaymentId)}: {PaymentId}, {nameof(Amount)}: {Amount}, {nameof(Currency)}: {Currency}, {nameof(Language)}: {Language}, {nameof(IpAddress)}: {IpAddress}";
        }
    }
}