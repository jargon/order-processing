using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{
    /// <summary>
    /// Generates packing slips for the royalty DEPARTMENT, not for the Danish royal family ;)
    /// </summary>
    public sealed class GeneratePackingSlipForRoyalty : CodeActivity
    {
        public InArgument<Payment> Payment { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var payment = Payment.Get(context);

            var slipGenerator = context.GetExtension<IPackingSlipGenerator>();
            slipGenerator.CreateSlipForRoyalty(payment);
        }
    }
}
