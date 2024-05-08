using System.ComponentModel.DataAnnotations;

namespace CLDV_POE_PART_TWO.Models
{
    public class Order
    {
        
        public int OrderID { get; set; }
        public string? UserID { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string? Address { get; set; }  // New address field


        //Navigation Properties

        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}
