using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductTransactionRepository
    {
        Task ProduceAsync(string productionNumber, Product product, int quantity, double price, string doneBy);
        Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double price, string doneBy);

        Task<IEnumerable<ProductTransaction>> GetProductTransactionAsync(string productName, DateTime? dateForm,
            DateTime? dateTo, ProductTransactionType? transactionType);
    }
}