namespace OrderProcessing.Domain.Products
{
    /// <summary>
    /// A product offered for sale or subscription by The Company.
    /// </summary>
    public abstract class Product
    {
        public enum Type
        {
            PhysicalGood,
            DigitalGood,
            Service
        }

        public Type ProductType { get; }
        public string SKU { get; }
        public string Name { get; }

        protected Product(Type productType, string stockKeepingUnit, string productName)
        {
            this.ProductType = productType;
            this.SKU = stockKeepingUnit;
            this.Name = productName;
        }

        public bool IsA<T>() where T : Product
        {
            var thisType = GetType();
            var thatType = typeof(T);
            return thisType == thatType || thisType.IsSubclassOf(thatType);
        }

        public T AsA<T>() where T : Product
        {
            return (T)this;
        }

        /// <summary>
        /// Returns <code>true</code> iff <paramref name="obj"/> is also a <see cref="Product"/> and the products have
        /// the same SKU number.
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as Product;

            if (other == null)
                return false;

            // This implementation assumes that SKU numbers are guaranteed to be unique
            return this.SKU.Equals(other.SKU);
        }

        public override int GetHashCode()
        {
            // NOTE: hash calculations are allowed to overflow
            unchecked
            {
                // Semi-standard hash calculation with some random primes
                return 37 * SKU.GetHashCode() + 971;
            };
        }
    }
}
