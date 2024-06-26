﻿namespace CLDV_POE_PART_TWO.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemID { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice
        {
            get
            {
                return $"R{Price:N2}";
            }
        }
        public string? ImagePath { get; set; }
    }
}
