using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CLDV_POE_PART_TWO.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }

        //Navigation Properties
        public virtual Products Products { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
