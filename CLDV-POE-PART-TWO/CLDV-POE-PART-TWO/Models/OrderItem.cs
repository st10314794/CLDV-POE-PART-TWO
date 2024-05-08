using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV_POE_PART_TWO.Models
{
    public class OrderItem
    {
        
        public int OrderItemID { get; set; }

       
        public int OrderID { get; set; }
       
        public int ProductID { get; set; }
     
        public decimal Price { get; set; }

        //Navigation Properties
        public virtual Order Order { get; set; }
        public virtual Products Product { get; set; }
    }
}
