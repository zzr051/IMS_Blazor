using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.CoreBusiness;
using IMS.UseCases.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Reprots
{
    public class SearchInventoryTransactionUseCase : ISearchInventoryTransactionUseCase
    {
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

        public SearchInventoryTransactionUseCase(IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> ExecuteAsync(
            string inventoryName,
            DateTime? dateForm,
            DateTime? dateTo,
            InventoryTransactionType? transactionType)
        {
            return await _inventoryTransactionRepository
                .GetInventoryTransactionsAsync(inventoryName, dateForm, dateTo,
                transactionType);
        }
    }
}