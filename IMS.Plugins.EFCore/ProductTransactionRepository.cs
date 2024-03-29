﻿using System;
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
            var prod = await _productRepository.GetProductByIdAsync(product.ProductId);
            //var prod = await _db.Products
            //    .Include(p => p.ProductInventories)
            //    !.ThenInclude(pi => pi.Inventory)
            //    .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (prod != null)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    int qtyBefore = pi.Inventory.Quantity;
                    pi.Inventory.Quantity -= quantity * pi.InventoryQuantity;

                    this._db.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductionNumber = productionNumber,
                        InventoryId = pi.Inventory.InventoryId,
                        QuantityBefore = qtyBefore,
                        ActivityType = InventoryTransactionType.ProduceProduct,
                        QuantityAfter = pi.Inventory.Quantity,
                        TransactionDate = DateTime.Now,
                        DoneBy = doneBy,
                        UnitPrice = price
                    });
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

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double price,
            string doneBy)
        {
            _db.ProductTransactions.Add(new ProductTransaction
            {
                SalesOrderNumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price,
                ActivityType = ProductTransactionType.SellProduct
            });

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionAsync(
            string productName,
            DateTime? dateForm,
            DateTime? dateTo,
            ProductTransactionType? transactionType)
        {
            if (dateTo.HasValue)
            {
                dateTo = dateTo.Value.AddDays(1);
            }

            var query =
                from pt in _db.ProductTransactions
                join prod in _db.Products on pt.ProductId equals prod.ProductId
                where (string.IsNullOrWhiteSpace(productName) ||
                       prod.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0) &&
                      (!dateForm.HasValue || pt.TransactionDate >= dateForm.Value.Date) &&
                      (!dateTo.HasValue || pt.TransactionDate <= dateTo.Value.Date) &&
                      (!transactionType.HasValue || pt.ActivityType == transactionType)
                select pt;

            return await query.Include(x => x.Product).ToListAsync();
        }
    }
}