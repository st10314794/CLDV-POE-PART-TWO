using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CLDV_POE_PART_TWO.Models;

namespace CLDV_POE_PART_TWO.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CLDV_POE_PART_TWO.Models.Products> Products { get; set; } = default!;
    }
}
