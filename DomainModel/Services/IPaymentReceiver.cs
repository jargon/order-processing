using OrderProcessing.Domain;

namespace OrderProcessing.Domain.Services
{
    /// <summary>
    /// This interface represents a way to get the next payment, in a way that does not depend on the source. This
    /// means that the rest of the system dos not depend whether payments are loaded from a file, taken from a queue.
    /// etc.
    /// </summary>
    public interface IPaymentReceiver
    {
        Payment BlockUntilPaymentReceived();
    }
}
