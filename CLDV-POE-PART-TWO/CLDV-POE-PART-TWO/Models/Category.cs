using System.ComponentModel.DataAnnotations;

namespace CLDV_POE_PART_TWO.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }


        // Navigation property
        public virtual ICollection<Products> Products { get; set; }
    }
}
