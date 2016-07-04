using System;
using OrderProcessing.Domain.Products;

namespace OrderProcessing.Domain
{
    /// <summary>
    /// Represents a payment for some product offered by The Company.
    /// </summary>
    public class Payment
    {
        public Agent Agent { get; }
        public Customer Customer { get; }
        public Product PurchasedProduct { get; }
        public decimal Amount { get; }
        public string CurrencyCode { get; }

        public Payment(Agent agent, Customer customer, Product purchasedProduct, decimal amount, string currencyCode)
        {
            if (agent == null)
                throw new ArgumentNullException(paramName: nameof(agent), message: "Payment must have an agent");
            if (customer == null)
                throw new ArgumentNullException(paramName: nameof(customer), message: "Payment must have a customer");
            if (purchasedProduct == null)
                throw new ArgumentNullException(paramName: nameof(purchasedProduct), message: "Payment must have a product");
            if (string.IsNullOrEmpty(currencyCode))
                throw new ArgumentException(paramName: nameof(currencyCode), message: "Payment must have a currency");

            this.Agent = agent;
            this.Customer = customer;
            this.PurchasedProduct = purchasedProduct;
            this.Amount = amount;
            this.CurrencyCode = currencyCode;
        }
    }
}
