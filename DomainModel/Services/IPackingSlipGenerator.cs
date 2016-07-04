using OrderProcessing.Domain;
using OrderProcessing.Domain.Products;

namespace OrderProcessing.Domain.Services
{
    public interface IPackingSlipGenerator
    {
        void CreateSlipForShipping(Payment payment);
        void CreateSlipForRoyalty(Payment payment);
        void CreateSlipForShippingWithExtraProduct(Payment payment, Product product);
    }
}
