using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{

    public sealed class ApplyMembershipUpgrade : CodeActivity
    {
        public InArgument<Membership> MembershipUpgrade { get; set; }
        public InArgument<Customer> Customer { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var upgrade = MembershipUpgrade.Get(context);
            var customer = Customer.Get(context);

            var activator = context.GetExtension<IMembershipActivator>();
            activator.Upgrade(upgrade.ActivationURL, customer);
        }
    }
}
