namespace CLDV_POE_PART_TWO.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; }
        public string FormattedTotalPrice
        {
            get
            {
                return $"R{TotalPrice:N2}";
            }
        }
    }
}
