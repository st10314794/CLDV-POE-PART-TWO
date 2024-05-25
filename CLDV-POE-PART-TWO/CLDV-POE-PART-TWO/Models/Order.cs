using CLDV_POE_PART_TWO.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV_POE_PART_TWO.Models
{
    public class Order
    {
        
        public int OrderID { get; set; }
        [Display(Name = "Order Number")]
        public int UserOrderNumber { get; set; }
        [Display(Name = "User")]
        public string? UserID { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        [NotMapped]
        public string FormattedPrice
        {
            get
            {
                return $"R{TotalPrice:N2}";
            }
        }

        // public string? Status { get; set; }

        [Display(Name = "Order Created")]

        public DateTime CreatedDate { get; set; }
        [Display(Name = "Order Modified")]

        public DateTime ModifiedDate { get; set; }
       

        //Navigation Properties

        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual IdentityUser? User { get; set; }
        [Display(Name = "Status")]
        public OrderStatus OrderStatus { get; set; }

    }
}
