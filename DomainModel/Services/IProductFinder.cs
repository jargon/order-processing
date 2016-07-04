using System;
using OrderProcessing.Domain.Products;

namespace OrderProcessing.Domain.Services
{
    public interface IProductFinder
    {
        T FindBySubtypeAndName<T>(string productName);
    }
}
