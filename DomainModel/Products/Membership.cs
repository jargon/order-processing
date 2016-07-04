using System;

namespace OrderProcessing.Domain.Products
{
    /// <summary>
    /// A <see cref="Membership"/> represents a user membership of some service offered by the company or an upgrade to
    /// an existing membership. It must have an activation URL, so that the user can be activated in the service to
    /// receive the newly purchased privileges.
    /// </summary>
    public class Membership : Product
    {
        public Uri ActivationURL { get; }
        public bool IsUpgrade { get; }

        public Membership(string stockKeepingUnit, string serviceName, Uri activationURL, bool isUpgradeToExistingMembership) :
            base(Product.Type.Service, stockKeepingUnit, productName: serviceName)
        {
            if (activationURL == null)
                throw new ArgumentNullException(paramName: nameof(activationURL), message: "Memberships must have an activation URL");

            this.ActivationURL = activationURL;
            this.IsUpgrade = isUpgradeToExistingMembership;
        }
    }
}
