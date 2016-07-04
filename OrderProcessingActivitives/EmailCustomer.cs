using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{

    public sealed class EmailCustomer : CodeActivity
    {
        public InArgument<Customer> Customer { get; set; }
        public InArgument<Membership> Membership { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var customer = Customer.Get(context);
            var membership = Membership.Get(context);

            var emailSender = context.GetExtension<IEmailSender>();
            if (membership.IsUpgrade)
            {
                emailSender.SendMembershipUpgraded(customer, membership);
            }
            else
            {
                emailSender.SendMembershipActivated(customer, membership);
            }
        }
    }
}
