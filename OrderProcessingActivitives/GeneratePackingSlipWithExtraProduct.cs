using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{

    public sealed class GeneratePackingSlipWithExtraProduct<T> : CodeActivity where T : Product
    {
        public InArgument<Payment> Payment { get; set; }
        public InArgument<string> ExtraProductName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var payment = Payment.Get(context);
            var extraProductName = ExtraProductName.Get(context);

            var productFinder = context.GetExtension<IProductFinder>();
            var extraProduct = productFinder.FindBySubtypeAndName<T>(extraProductName);

            var generator = context.GetExtension<IPackingSlipGenerator>();
            generator.CreateSlipForShippingWithExtraProduct(payment, extraProduct);
        }
    }
}
