using System.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Services;

namespace OrderProcessing.Activities
{

    public sealed class ReceivePayment : CodeActivity<Payment>
    {
        protected override Payment Execute(CodeActivityContext context)
        {
            var receiver = context.GetExtension<IPaymentReceiver>();
            return receiver.BlockUntilPaymentReceived();
        }
    }
}
