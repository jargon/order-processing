using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;

namespace OrderProcessing.Domain.Services
{
    public interface IEmailSender
    {
        void SendMembershipActivated(Customer customer, Membership membership);
        void SendMembershipUpgraded(Customer customer, Membership upgrade);
    }
}
