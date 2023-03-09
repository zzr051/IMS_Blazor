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


        private int price;

        [Range(0, double.MaxValue, ErrorMessage = "price must be greater or equal to {0}")]
        [Product_EnsurePriceIsGreaterThanInventoriesPrice]
        public int Price
        {
            get { return price; }
            set
            {
                Console.WriteLine("hello");
                price = value;
            }
        }


        //construct many to many
        public List<ProductInventory>? ProductInventories { get; set; }

        public bool ValidatePricing()
        {
            if (ProductInventories == null || ProductInventories.Count <= 0)
            {
                return true;
            }

            double priceOfAllInvs = this.ProductInventories.Sum(x => x.Inventory?.Price ?? 0);
            if (priceOfAllInvs > Price)
            {
                return false;
            }

            return true;
        }
    }
}