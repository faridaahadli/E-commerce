using System.Web;

namespace CRMHalalBackEnd.Interfaces
{
    public interface IPaymentProvider
    {
        string GetPaymentPageUrl(string lang, int paymentId, int userId, string tenantId);
        string Complete(string transId);
    }
}