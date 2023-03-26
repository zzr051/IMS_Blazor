using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                    (p.ProductName.ToLower().IndexOf(name.ToLower()) >= 0 ||
                     string.IsNullOrWhiteSpace(name)) &&
                    p.IsActive == true)
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            //To prevent to add same product
            if (_db.Products.Any(p =>
                    p.ProductName.ToLower() == product.ProductName.ToLower())) return;

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

        public async Task UpdateProductAsync(Product product)
        {
            var res = _db.Products.Any(p =>
                p.ProductName.ToLower() == product.ProductName.ToLower());

            if (res == true)
            {
                return;
            }

            var pro = await _db.Products.FindAsync(product.ProductId);
            if (pro != null)
            {
                pro.ProductName = product.ProductName;
                pro.Price = product.Price;
                pro.Quantity = product.Quantity;
                pro.ProductInventories = product.ProductInventories;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            //soft delete
            var product = await _db.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }
    }
}