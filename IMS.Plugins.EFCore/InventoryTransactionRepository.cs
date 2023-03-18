using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.EFCore
{
    public class InventoryTransactionRepository:IInventoryTransactionRepository
    {
        public Task ExecuteAsync(string poNumber, double price, int inventoryId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
