using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.CoreBusiness.Validations;

namespace IMS.CoreBusiness
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required] public string ProductName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater or equal to {0}")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "price must be greater or equal to {0}")]
        [Product_EnsurePriceIsGreaterThanInventoriesPrice]
        public int Price { get; set; }


        //construct many to many
        public List<ProductInventory>? ProductInventories { get; set; }

        public bool IsActive { get; set; } = true;

        public double TotalInventoryCost()
        {
            double priceOfAllInvs = this.ProductInventories
                .Sum(x => x.Inventory?.Price * x.InventoryQuantity ?? 0);
            return priceOfAllInvs;
        }

        public bool ValidatePricing()
        {
            if (ProductInventories == null || ProductInventories.Count <= 0)
            {
                return true;
            }

            if (TotalInventoryCost() > Price)
            {
                return false;
            }

            return true;
        }
    }
}