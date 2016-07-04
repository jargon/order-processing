using System;
using System.Activities;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using OrderProcessing.Activities;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;
using OrderProcessing.Domain.Services;
using static OrderProcessingModule.WorkflowExtensions;

namespace OrderProcessing.Test
{
    /// <summary>
    /// The code activities act as simple adapters to outside services, so there is little to test here.
    /// </summary>
    [TestFixture]
    public class CodeActivityTest
    {
        [Test]
        public void ActivateMembership_CallsActivateWithCorrectArgs()
        {
            var membership = new Membership("mmm", "bonusPlusXxtra",
                new Uri("http://acme.com/membership/activate"), isUpgradeToExistingMembership: false);
            var customer = new Customer("roadrunner@acme.com");

            var wfParams = new Dictionary<string, object>
            {
                { "PurchasedMembership", membership },
                { "Customer", customer }
            };

            var activity = new ActivateMembership();
            var wfApp = new WorkflowApplication(activity, wfParams);

            var mock = Substitute.For<IMembershipActivator>();
            wfApp.Extensions.Add(mock);

            wfApp.RunAndBlockUntilComplete();

            mock.Received().Activate(membership.ActivationURL, customer);
        }

        [Test]
        public void ApplyMembershipUpgrade_CallsUpgradeWithCorrectArgs()
        {
            var upgrade = new Membership("mmm", "bonusPlusXxtraSPECIAL",
                new Uri("http://acme.com/membership/activate"), isUpgradeToExistingMembership: true);
            var customer = new Customer("roadrunner@acme.com");

            var wfParams = new Dictionary<string, object>
            {
                { "MembershipUpgrade", upgrade },
                { "Customer", customer }
            };

            var activity = new ApplyMembershipUpgrade();
            var wfApp = new WorkflowApplication(activity, wfParams);

            var mock = Substitute.For<IMembershipActivator>();
            wfApp.Extensions.Add(mock);

            wfApp.RunAndBlockUntilComplete();

            mock.Received().Upgrade(upgrade.ActivationURL, customer);
        }

        [Test]
        public void EmailCustomer_CallsSendMembershipActivated_ForNewMembership()
        {
            var membership = new Membership("mmm", "Members Club",
                new Uri("http://acme.com/membership/activate"), isUpgradeToExistingMembership: false);
            var customer = new Customer("iwanttobeamember@example.com");

            var wfParams = new Dictionary<string, object>
            {
                { "Membership", membership },
                { "Customer", customer }
            };

            var activity = new EmailCustomer();
            var wfApp = new WorkflowApplication(activity, wfParams);

            var mock = Substitute.For<IEmailSender>();
            wfApp.Extensions.Add(mock);

            wfApp.RunAndBlockUntilComplete();

            mock.Received().SendMembershipActivated(customer, membership);
        }

        [Test]
        public void EmailCustomer_CallsSendMembershipUpgraded_ForUpgrade()
        {
            var upgrade = new Membership("mmm", "VIP Club",
                new Uri("http://acme.com/membership/activate"), isUpgradeToExistingMembership: true);
            var customer = new Customer("iwanttobeamember@example.com");

            var wfParams = new Dictionary<string, object>
            {
                { "Membership", upgrade },
                { "Customer", customer }
            };

            var activity = new EmailCustomer();
            var wfApp = new WorkflowApplication(activity, wfParams);

            var mock = Substitute.For<IEmailSender>();
            wfApp.Extensions.Add(mock);

            wfApp.RunAndBlockUntilComplete();

            mock.Received().SendMembershipUpgraded(customer, upgrade);
        }

        [Test]
        public void GenerateCommission_CallsCommissionServiceWithPayment()
        {
            var payment = CreateVideoTestPayment();
            var wfParams = new Dictionary<string, object> { { "Payment", payment } };

            var activity = new GenerateCommission();
            var wfApp = new WorkflowApplication(activity, wfParams);

            var mock = Substitute.For<ICommissionService>();
            wfApp.Extensions.Add(mock);

            wfApp.RunAndBlockUntilComplete();

            mock.Received().GenerateCommissionFor(payment);
        }

        [Test]
        public void GeneratePackingSlipForRoyalty_CallsCreateSlipForRoyaltyWithPayment()
        {
            var payment = CreateVideoTestPayment();
            var wfParams = new Dictionary<string, object> { { "Payment", payment } };

            var activity = new GeneratePackingSlipForRoyalty();
            var wfApp = new WorkflowApplication(activity, wfParams);
            
            var mock = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(mock);

            wfApp.RunAndBlockUntilComplete();

            mock.Received().CreateSlipForRoyalty(payment);
        }

        [Test]
        public void GeneratePackingSlipForShipping_CallsCreateSlipForShippingWithPayment()
        {
            var payment = CreateVideoTestPayment();
            var wfParams = new Dictionary<string, object> { { "Payment", payment } };

            var activity = new GeneratePackingSlipForShipping();
            var wfApp = new WorkflowApplication(activity, wfParams);

            var mock = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(mock);

            wfApp.RunAndBlockUntilComplete();

            mock.Received().CreateSlipForShipping(payment);
        }

        [Test]
        public void GeneratePackingSlipWithExtraProduct_LooksUpProductFromGivenNameAndType_AndCallsCreateWithExtraProductWithIt()
        {
            var payment = CreateVideoTestPayment();
            var wfParams = new Dictionary<string, object>
            {
                { "Payment", payment },
                { "ExtraProductName", "Mr. Trololo original upload" }
            };

            var expectedExtraProduct = CreateAmazingVideo();

            var activity = new GeneratePackingSlipWithExtraProduct<Video>();
            var wfApp = new WorkflowApplication(activity, wfParams);

            var productFinder = Substitute.For<IProductFinder>();
            productFinder.FindBySubtypeAndName<Video>("Mr. Trololo original upload").Returns(expectedExtraProduct);
            wfApp.Extensions.Add(productFinder);
            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.Received().CreateSlipForShippingWithExtraProduct(payment, expectedExtraProduct);
        }

        private Payment CreateVideoTestPayment()
        {
            var product = new Video(Video.Medium.Disc, stockKeepingUnit: "Huttelihut",
                videoTitle: "The best of Danish Sports Commentary", streamingURL: null);

            var payment = new Payment(new Agent("Mr Salesman"), new Customer("ratatat@serialbuyer.com"), product, 27.50m, "DKK");
            return payment;
        }

        private Video CreateAmazingVideo()
        {
            var vid = new Video(Video.Medium.Online, stockKeepingUnit: "Trololo",
                videoTitle: "Mr. Trololo original upload", streamingURL: new Uri("https://www.youtube.com/watch?v=oavMtUWDBTM"));
            return vid;
        }
    }
}
