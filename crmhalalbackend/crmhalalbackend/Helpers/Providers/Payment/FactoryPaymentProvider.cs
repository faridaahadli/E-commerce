using CRMHalalBackEnd.Helpers.Providers.Payment;
using CRMHalalBackEnd.Interfaces;

namespace CRMHalalBackEnd.Helpers
{
    public class FactoryPaymentProvider
    {
        public static IPaymentProvider Build(string provider)
        {
            if (provider.ToLower().Equals("pashabank"))
            {
                return new PashabankPaymentProvider();
            }
            else if (provider.ToLower().Equals("smspacket")){
                return new SmsPacketPaymentProvider();
            }
            return null;
        }
    }
}