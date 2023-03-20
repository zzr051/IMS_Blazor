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
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private readonly IMSContext _db;
        private readonly IProductRepository _productRepository;

        public ProductTransactionRepository(IMSContext db, IProductRepository productRepository)
        {
            _db = db;
            _productRepository = productRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, double price,
            string doneBy)
        {
            //refactor
            //var prod = this._productRepository.GetProductByIdAsync(product.ProductId);
            var prod = await _db.Products
                .Include(p => p.ProductInventories)
                !.ThenInclude(pi => pi.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (prod != null)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    pi.Inventory.Quantity -= quantity * pi.InventoryQuantity;
                }
            }

            this._db.ProductTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });
            await this._db.SaveChangesAsync();
        }
    }
}