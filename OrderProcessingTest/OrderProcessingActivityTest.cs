using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;
using OrderProcessing.Domain.Services;
using OrderProcessingModule;
using static OrderProcessingModule.WorkflowExtensions;

namespace OrderProcessing.Test
{
    [TestFixture]
    public class OrderProcessingActivityTest
    {
        private static Lazy<HashSet<Type>> requiredServices = new Lazy<HashSet<Type>>(() =>
        {
            var serviceAssembly = typeof(ICommissionService).Assembly;
            var serviceNamespace = typeof(ICommissionService).Namespace;

            var allServices = serviceAssembly.GetTypes()
                    .Where(t => t.Namespace.Equals(serviceNamespace));
            return new HashSet<Type>(allServices);
        });

        [Test]
        public void DiscVideo_GeneratesPackingSlipForShipping()
        {
            var discPayment = CreateVideoDiscPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", discPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.Received().CreateSlipForShipping(discPayment);
        }

        [Test]
        public void PrintBook_GeneratesPackingSlipForShipping()
        {
            var bookPayment = CreatePaperBookPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", bookPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);
            
            wfApp.RunAndBlockUntilComplete();

            slipGenerator.Received().CreateSlipForShipping(bookPayment);
        }

        /// <summary>Non-physical products must not generate packing slips for shipping.</summary>
        [Test]
        public void EBook_DoesNotGeneratePackingSlipForShipping()
        {
            var ebookPayment = CreateEBookPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", ebookPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.DidNotReceive().CreateSlipForShipping(Arg.Any<Payment>());
        }

        /// <summary>
        /// As a result of the quick-and-dirty design of the IPackingSlipGenerator interface, we need to exclude the
        /// particular video "Learning to Ski" from generating a normal packing slip, since it generates a specially
        /// amended slip.
        /// </summary>
        [Test]
        public void LearningToSki_DoesNotGenerateNormalPackingSlipForShipping()
        {
            var videoPayment = CreateLearningToSkiPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", videoPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.DidNotReceive().CreateSlipForShipping(Arg.Any<Payment>());
        }

        [Test]
        public void PrintBook_GeneratesRoyaltySlip()
        {
            var bookPayment = CreatePaperBookPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", bookPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.Received().CreateSlipForRoyalty(bookPayment);
        }

        [Test]
        public void EBook_GeneratesRoyaltySlip()
        {
            var ebookPayment = CreateEBookPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", ebookPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.Received().CreateSlipForRoyalty(ebookPayment);
        }

        /// <summary>Non-books must not generate packing slips for the royalty dept (for now).</summary>
        [Test]
        public void Video_DoesNotGenerateRoyaltySlip()
        {
            var videoPayment = CreateVideoDiscPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", videoPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.DidNotReceive().CreateSlipForRoyalty(Arg.Any<Payment>());
        }

        [Test]
        public void Membership_IsActivated()
        {
            var membershipPayment = CreateMembershipPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", membershipPayment } });

            var membershipActivator = Substitute.For<IMembershipActivator>();
            wfApp.Extensions.Add(membershipActivator);
            RegisterMocksForAllServicesExcept<IMembershipActivator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            membershipActivator.Received().Activate(membershipPayment.PurchasedProduct.AsA<Membership>().ActivationURL, membershipPayment.Customer);
        }

        /// <summary>Upgrades to memberships should be handled specially.</summary>
        [Test]
        public void Upgrade_IsNotActivated()
        {
            var upgradePayment = CreateUpgradePayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", upgradePayment } });

            var membershipActivator = Substitute.For<IMembershipActivator>();
            wfApp.Extensions.Add(membershipActivator);
            RegisterMocksForAllServicesExcept<IMembershipActivator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            membershipActivator.DidNotReceive().Activate(Arg.Any<Uri>(), Arg.Any<Customer>());
        }

        [Test]
        public void Upgrade_IsApplied()
        {
            var upgradePayment = CreateUpgradePayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", upgradePayment } });

            var membershipActivator = Substitute.For<IMembershipActivator>();
            wfApp.Extensions.Add(membershipActivator);
            RegisterMocksForAllServicesExcept<IMembershipActivator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            membershipActivator.Received().Upgrade(upgradePayment.PurchasedProduct.AsA<Membership>().ActivationURL, upgradePayment.Customer);
        }

        /// <summary>New memberships should be handled differently from upgrades to existing ones.</summary>
        [Test]
        public void Membership_IsNotApplied()
        {
            var membershipPayment = CreateMembershipPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", membershipPayment } });

            var membershipActivator = Substitute.For<IMembershipActivator>();
            wfApp.Extensions.Add(membershipActivator);
            RegisterMocksForAllServicesExcept<IMembershipActivator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            membershipActivator.DidNotReceive().Upgrade(Arg.Any<Uri>(), Arg.Any<Customer>());
        }

        [Test]
        public void Membership_EmailsOwner()
        {
            var membershipPayment = CreateMembershipPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", membershipPayment } });

            var emailSender = Substitute.For<IEmailSender>();
            wfApp.Extensions.Add(emailSender);
            RegisterMocksForAllServicesExcept<IEmailSender>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            emailSender.Received().SendMembershipActivated(membershipPayment.Customer, membershipPayment.PurchasedProduct.AsA<Membership>());
        }

        [Test]
        public void LearningToSki_AddsFirstAidToPackingSlip()
        {
            var videoPayment = CreateLearningToSkiPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", videoPayment } });

            var slipGenerator = Substitute.For<IPackingSlipGenerator>();
            wfApp.Extensions.Add(slipGenerator);
            RegisterMocksForAllServicesExcept<IPackingSlipGenerator>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            slipGenerator.Received().CreateSlipForShippingWithExtraProduct(Arg.Any<Payment>(), Arg.Any<Product>());
        }

        [Test]
        public void Disc_GeneratesCommissionToAgent()
        {
            var discPayment = CreateVideoDiscPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", discPayment } });

            var commissionService = Substitute.For<ICommissionService>();
            wfApp.Extensions.Add(commissionService);
            RegisterMocksForAllServicesExcept<ICommissionService>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            commissionService.Received().GenerateCommissionFor(discPayment);
        }
        
        [Test]
        public void EBook_GeneratesCommissionToAgent()
        {
            var ebookPayment = CreateEBookPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", ebookPayment } });

            var commissionService = Substitute.For<ICommissionService>();
            wfApp.Extensions.Add(commissionService);
            RegisterMocksForAllServicesExcept<ICommissionService>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            commissionService.Received().GenerateCommissionFor(ebookPayment);
        }

        [Test]
        public void Membership_DoesNotGenerateCommission()
        {
            var membershipPayment = CreateMembershipPayment();
            var wfApp = CreateWorkflowAppFor(new Dictionary<string, object> { { "payment", membershipPayment } });

            var commissionService = Substitute.For<ICommissionService>();
            wfApp.Extensions.Add(commissionService);
            RegisterMocksForAllServicesExcept<ICommissionService>(wfApp);

            wfApp.RunAndBlockUntilComplete();

            commissionService.DidNotReceive().GenerateCommissionFor(Arg.Any<Payment>());
        }

        private Payment CreatePaperBookPayment()
        {
            var book = new Book(Book.Medium.Print, "How to waste trees", "asdf", "How to waste trees");
            return new Payment(new Agent("Krydderihans"), new Customer("dinkunde@kundeservice.dk"), book, 1m, "DKK");
        }

        private Payment CreateEBookPayment()
        {
            var book = new Book(Book.Medium.EBook, "Saving the Planet", "asdf1", "Saving the Planet", downloadURL: new Uri("http://acme.com/getsavep"));
            return new Payment(new Agent("Skærmtrolden Hugo"), new Customer("jepjep@kundeservice.dk"), book, 1m, "USD");
        }

        private Payment CreateVideoDiscPayment()
        {
            var video = new Video(Video.Medium.Disc, "Hmmmmmnah", "How to procrastinate better");
            return new Payment(new Agent("Magiske Oscar"), new Customer("endnuenkunde@kundeservice.dk"), video, 100m, "DKK");
        }

        private Payment CreateMembershipPayment()
        {
            var membership = new Membership("Members Club", "Members Club", new Uri("http://acme.com/acti"), isUpgradeToExistingMembership: false);
            return new Payment(new Agent("Membership Seller"), new Customer("iwantin@kundeservice.dk"), membership, 99m, "DKK");
        }

        private Payment CreateUpgradePayment()
        {
            var upgrade = new Membership("VIP Club", "VIP Club", new Uri("http://acme.com/acti"), isUpgradeToExistingMembership: true);
            return new Payment(new Agent("Membership Seller"), new Customer("iwantin@kundeservice.dk"), upgrade, 499m, "DKK");
        }

        private Payment CreateLearningToSkiPayment()
        {
            var video = new Video(Video.Medium.Disc, "Learning to Ski", "Learning to Ski");
            return new Payment(new Agent("Magiske Oscar"), new Customer("endnuenkunde@kundeservice.dk"), video, 100m, "DKK");
        }

        private WorkflowApplication CreateWorkflowAppFor(IDictionary<string, object> args)
        {
            string basePath = TestContext.CurrentContext.TestDirectory;
            string fileName = "OrderProcessingActivity.xaml";

            var workflow = ActivityXamlServices.Load(Path.Combine(basePath, fileName),
                new ActivityXamlServicesSettings { CompileExpressions = true });

            var wfApp = new WorkflowApplication(workflow, args);
            
            return wfApp;
        }

        private void RegisterMocksForAllServicesExcept<T>(WorkflowApplication wfApp)
        {
            foreach (var service in requiredServices.Value.Where(t => t != typeof(T)))
            {
                var mock = Substitute.For(typesToProxy: new Type[] { service }, constructorArguments: null);
                wfApp.Extensions.Add(mock);
            }
        }
    }
}
