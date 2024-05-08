using CLDV_POE_PART_TWO.Data;
using CLDV_POE_PART_TWO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLDV_POE_PART_TWO.Controllers
{

    public class CheckoutController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge(); // Ensures user is logged in

            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                         .ThenInclude(ci => ci.Products)
                                     .FirstOrDefaultAsync(c => c.UserID == user.Id);

            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Home"); // Handle empty cart scenario

            // Create a new order object from the cart data
            var order = new Order
            {
                UserID = user.Id,
                OrderDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Status = "Pending", // Initial status
                TotalPrice = cart.CartItems.Sum(item => item.Products.Price) // Calculate total price
            };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            // Optionally clear the cart here or mark as completed
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }


        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            // Retrieve the order using the provided order ID
            var order = await _context.Order
                                      .Include(o => o.OrderItems)  // Assuming you have a related collection of order items
                                      .ThenInclude(oi => oi.Product)  // Assuming each order item includes product details
                                      .FirstOrDefaultAsync(o => o.OrderID == orderId);

            // Check if the order exists
            if (order == null)
            {
                return NotFound(); // Return a NotFound view or a custom error message if the order doesn't exist
            }

            // Optionally check if the logged-in user has the right to view this order
            if (!order.UserID.Equals(user.Id))
            {
                return Unauthorized("You do not have permission to view this order.");
                //return Unauthorized(); // Prevent users from seeing other users' order confirmations
            }

            // Return the view with the order model
            return View(order);
        }

        public IActionResult Index()
        {
            return View();
        }

     


    }
}
