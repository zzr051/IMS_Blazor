using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.CoreBusiness
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required] public string ProductName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater or equal to {0}")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "price must be greater or equal to {0}")]
        public int Price { get; set; }

        //construct many to many
        public List<ProductInventory>? ProductInventories { get; set; }

        public bool ValidatePricing()
        {
            if (ProductInventories == null || ProductInventories.Count <= 0)
            {
                return true;
            }

            double priceOfAllInvs = this.ProductInventories.Sum(x => x.Inventory?.Price ?? 0);
            if (priceOfAllInvs < Price)
            {
                return false;
            }

            return true;
        }
    }
}