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

        public async Task<List<Product>> GetProductsByName(string name = "")
        {
            return await _db.Products.Where(p =>
                    p.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(name))
                .ToListAsync();
        }
    }
}