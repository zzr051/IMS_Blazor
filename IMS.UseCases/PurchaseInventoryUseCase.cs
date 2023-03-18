using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases
{
    public class PurchaseInventoryUseCase
    {
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;


        public PurchaseInventoryUseCase(IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task ExecuteAsync(string poNumber, double price, int inventoryId, int quantity)
        {
            await this._inventoryTransactionRepository.ExecuteAsync(poNumber, price, inventoryId, quantity);
        }
    }
}