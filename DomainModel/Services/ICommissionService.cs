using OrderProcessing.Domain;

namespace OrderProcessing.Domain.Services
{
    public interface ICommissionService
    {
        void GenerateCommissionFor(Payment payment);
    }
}
