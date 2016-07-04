using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using OrderProcessing.Domain.Services;

namespace OrderProcessingModule
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Create an actual payment receiver here
            //var paymentReceiver = ...
            //var program = new Program(paymentReceiver);
        }

        private readonly IPaymentReceiver paymentReceiver;

        Program(IPaymentReceiver paymentReceiver)
        {
            this.paymentReceiver = paymentReceiver;
        }

        private void MainLoop(CancellationToken cancellationToken)
        {
            // TODO: The program is likely to perform a lot better if we batched payments
            while (!cancellationToken.IsCancellationRequested)
            {
                var payment = paymentReceiver.BlockUntilPaymentReceived();
                var workflowParameters = new Dictionary<string, object> { { "payment", payment } };

                string basePath = ".";
                string fileName = "OrderProcessingActivity.xaml";

                // TODO: Cache the workflow, but in a way that allows for dynamic update
                var workflow = ActivityXamlServices.Load(Path.Combine(basePath, fileName),
                    new ActivityXamlServicesSettings { CompileExpressions = true });

                var wfApp = new WorkflowApplication(workflow, workflowParameters);

                // TODO: Register actual service implementations here

                wfApp.RunAndBlockUntilComplete();
            }
        }
    }
}
