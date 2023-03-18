using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.CoreBusiness
{
    public class InventoryTransaction
    {
        public int InventoryTransactionId { get; set; }
        
        public int InventoryId { get; set; }

        public int QuantityBefore { get; set; }
        
        public InventoryTransactionType InventoryType { get; set; }

        //This is the action taken

        public int QuantityAfter { get; set; }

        public DateTime TransactionDate { get; set; }

        public string DoneBy { get; set; }

        //Navigation Properties

        public Inventory Inventory { get; set; }
    }
}