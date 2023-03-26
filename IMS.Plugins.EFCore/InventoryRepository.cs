using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCore
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMSContext _db;

        public InventoryRepository(IMSContext db)
        {
            this._db = db;
        }

        public async Task<Inventory?> GetInventoryByIdAsync(int inventoryId)
        {
            return await this._db.Inventories.FindAsync(inventoryId);
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            //return await this._db.Inventories
            //    .Where(x => x.InventoryName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
            //                string.IsNullOrWhiteSpace(name))
            //    .ToListAsync();

            return await _db.Inventories.Where(x =>
                x.InventoryName.ToLower()
                    .IndexOf(name.ToLower()) >= 0).ToListAsync();

            //await _db.Inventories.Where(x=>x.InventoryName==name||name==null).ToListAsync();
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            //To prevent to add same inventory
            if (_db.Inventories.Any(x =>
                    x.InventoryName.ToLower() == inventory.InventoryName.ToLower())) return;

            await this._db.Inventories.AddAsync(inventory);
            await this._db.SaveChangesAsync();
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            //To prevent different inventories from having the same name
            if (_db.Inventories.Any(x =>
                    x.InventoryId != inventory.InventoryId &&
                    x.InventoryName.ToLower() != inventory.InventoryName.ToLower())) return;

            var inv = await this._db.Inventories.FindAsync(inventory.InventoryId);
            if (inv != null)
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Price = inventory.Price;
                inv.Quantity = inventory.Quantity;

                await _db.SaveChangesAsync();
            }
        }
    }
}