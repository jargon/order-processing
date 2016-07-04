using System;
using OrderProcessing.Domain;

namespace OrderProcessing.Domain.Services
{
    public interface IMembershipActivator
    {
        void Activate(Uri activationURL, Customer customer);
        void Upgrade(Uri upgradeURL, Customer customer);
    }
}
