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
    public class ProductRepository : IProductRepository
    {
        private readonly IMSContext _db;

        public ProductRepository(IMSContext db)
        {
            this._db = db;
        }

        public async Task<List<Product>> GetProductsByNameAsync(string name = "")
        {
            return await _db.Products.Where(p =>
                    p.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(name))
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            //To prevent to add same product
            if (_db.Products.Any(p =>
                    p.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase))) return;

            await this._db.Products.AddAsync(product);
            await this._db.SaveChangesAsync();

            #region test是否真的完整的添加了product

            //var prods = _db.Products.Include(p => p.ProductInventories)
            //    .ThenInclude(pi => pi.Inventory).ToList();

            //foreach (var pro in prods)
            //{
            //    Console.WriteLine($"产品是：{pro.ProductName}");
            //    foreach (var item in pro.ProductInventories)
            //    {
            //        Console.WriteLine($"部件是：{item.Inventory.InventoryName}");
            //    }
            //}

            #endregion
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _db.Products
                .Include(p => p.ProductInventories)
                .ThenInclude(pr => pr.Inventory)
                .FirstOrDefaultAsync(x => x.ProductId == productId);
        }
    }
}