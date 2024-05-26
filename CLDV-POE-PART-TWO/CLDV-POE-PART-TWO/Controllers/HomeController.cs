using CLDV_POE_PART_TWO.Data;
using CLDV_POE_PART_TWO.Models;
using CLDV_POE_PART_TWO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CLDV_POE_PART_TWO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            if (User?.Identity?.IsAuthenticated ?? false && User.IsInRole("Admin"))
            {
                var model = new AdminDashboardViewModel
                {
                    TotalProducts = await _context.Products.CountAsync(),
                    TotalOrders = await _context.Order.CountAsync(),
                    TotalUsers = await _context.Users.CountAsync(),
                    RecentActivities = await GetRecentActivities()
                };
                return View( model);
            }
            return View();
        }

        private async Task<List<string>> GetRecentActivities()
        {
            var recentActivities = new List<string>();

            var recentOrders = await _context.Order
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedDate)
                .Take(4)
                .ToListAsync();

            foreach (var order in recentOrders)
            {
                recentActivities.Add($"Order #{order.OrderID} by {order.User.UserName} - {order.OrderStatus} - R {order.TotalPrice}");
            }

            return recentActivities;
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
