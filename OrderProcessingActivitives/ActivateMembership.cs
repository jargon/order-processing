using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{

    public sealed class ActivateMembership : CodeActivity
    {
        public InArgument<Membership> PurchasedMembership { get; set; }
        public InArgument<Customer> Customer { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var membership = PurchasedMembership.Get(context);
            var customer = Customer.Get(context);

            var activator = context.GetExtension<IMembershipActivator>();
            activator.Activate(membership.ActivationURL, customer);
        }
    }
}
