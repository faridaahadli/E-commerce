namespace CRMHalalBackEnd.Models.Order
{
    public class CommonStoreData
    {
        public string TenantId { get; set; }
        public string StoreName { get; set; }
        public string Domain { get; set; }
        public decimal DeliveryPrice { get; set; }
        public string Status { get; set; }
    }
}