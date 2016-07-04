using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{

    public sealed class GenerateCommission : CodeActivity
    {
        public InArgument<Payment> Payment { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var payment = Payment.Get(context);
            var commissionService = context.GetExtension<ICommissionService>();
            commissionService.GenerateCommissionFor(payment);
        }
    }
}
