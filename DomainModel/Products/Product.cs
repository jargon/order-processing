namespace OrderProcessing.Domain.Products
{
    public abstract class Product
    {
        public ProductType Type { get; }
        public string SKU { get; }
        public string Name { get; }

        protected Product(ProductType type, string stockKeepingUnit, string productName)
        {
            this.Type = type;
            this.SKU = stockKeepingUnit;
            this.Name = productName;
        }
    }
}
