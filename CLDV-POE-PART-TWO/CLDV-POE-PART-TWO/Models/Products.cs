﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV_POE_PART_TWO.Models
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }
        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }
        [Display(Name = "Description")]
        public string? ProductDescription { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "In Stock")]
        public bool InStock { get; set; }
        public string? ImagePath { get; set; }
        public int CategoryID { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        //Navigation Properties
        public virtual Category? Category { get; set; }
    }
}
