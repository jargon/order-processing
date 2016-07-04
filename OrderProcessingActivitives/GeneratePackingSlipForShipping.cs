using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{

    public sealed class GeneratePackingSlipForShipping : CodeActivity
    {
        public InArgument<Payment> Payment { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var payment = Payment.Get(context);

            var slipGenerator = context.GetExtension<IPackingSlipGenerator>();
            slipGenerator.CreateSlipForShipping(payment);
        }
    }
}
