namespace OrderProcessing.Domain
{
    /// <summary>
    /// A customer of The Company. Customers purchase <see cref="Product"/>s and make <see cref="Payment"/>s for them.
    /// </summary>
    public class Customer
    {
        public string EmailAddress { get; }

        public Customer(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }
    }
}
