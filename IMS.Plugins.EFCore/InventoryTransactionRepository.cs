using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCore
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly IMSContext _db;

        public InventoryTransactionRepository(IMSContext db)
        {
            _db = db;
        }

        public async Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, double price, string doneBy)
        {
            this._db.InventoryTransactions.Add(new InventoryTransaction
            {
                PONumber = poNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                QuantityAfter = inventory.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price * quantity
            });

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(
            string inventoryName,
            DateTime? dateForm,
            DateTime? dateTo,
            InventoryTransactionType? transactionType)
        {
            var query = from it in _db.InventoryTransactions
                join inv in _db.Inventories on it.InventoryId equals inv.InventoryId
                where (string.IsNullOrWhiteSpace(inventoryName) ||
                       inv.InventoryName.Contains(inventoryName, StringComparison.OrdinalIgnoreCase)) &&
                      (!dateForm.HasValue || it.TransactionDate >= dateForm.Value.Date) &&
                      (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date) &&
                      (!transactionType.HasValue || it.ActivityType == transactionType)
                select it;


            return await query.Include(x => x.Inventory).ToListAsync();
        }
    }
}