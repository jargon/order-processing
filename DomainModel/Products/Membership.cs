using System;

namespace OrderProcessing.Domain.Products
{
    public class Membership : Product
    {
        public Uri ActivationURL { get; }
        public bool IsUpgrade { get; }

        public Membership(string stockKeepingUnit, string serviceName, Uri activationURL, bool isUpgradeToExistingMembership) :
            base(ProductType.Service, stockKeepingUnit, productName: serviceName)
        {
            this.ActivationURL = activationURL;
            this.IsUpgrade = isUpgradeToExistingMembership;
        }
    }
}
