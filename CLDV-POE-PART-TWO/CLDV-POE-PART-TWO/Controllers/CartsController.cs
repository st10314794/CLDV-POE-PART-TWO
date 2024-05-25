using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDV_POE_PART_TWO.Data;
using CLDV_POE_PART_TWO.Models;
using Microsoft.AspNetCore.Identity;
using CLDV_POE_PART_TWO.ViewModels;

namespace CLDV_POE_PART_TWO.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public CartsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            //Makes user login if not already
            if (user == null) return Challenge();  

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound("Product not available.");

            //Making product unavailable
            product.InStock = false; 

            var cart = await _context.Carts.Include(c => c.CartItems)
                                           .SingleOrDefaultAsync(c => c.UserID == user.Id);

            if (cart == null)
            {
                cart = new Cart { UserID = user.Id };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductID == productId);
            if (cartItem == null)
            {
                cart.CartItems.Add(new CartItem { ProductID = productId });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            // Find the cart item in the database
            var cartItem = await _context.CartItems
                                         .Include(ci => ci.Products)
                                         .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId);

            if (cartItem == null)
            {
                return NotFound();
            }

           //Making product available again when removed from cart
            cartItem.Products.InStock = true;  

            // Remove the cart item from the database
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

           
            return RedirectToAction("ViewCart");
        }


        [HttpGet]
        public async Task<IActionResult> ReturnToCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .ThenInclude(ci => ci.Products)
                                     .FirstOrDefaultAsync(c => c.UserID == user.Id);

            if (cart == null)
            {
                return View(new CartViewModel());
            }

            var cartViewModel = new CartViewModel
            {
                CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemID = ci.CartItemID,
                    ProductName = ci.Products.ProductName,
                    Price = ci.Products.Price,
                    ImagePath = ci.Products.ImagePath
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(item => item.Products.Price)
            };

            return View("ViewCart", cartViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .ThenInclude(ci => ci.Products)
                                     .FirstOrDefaultAsync(c => c.UserID == user.Id);

            if (cart == null)
            {
                return View(new CartViewModel());
            }

            var cartViewModel = new CartViewModel
            {
                CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemID = ci.CartItemID,
                    ProductName = ci.Products.ProductName,
                    Price = ci.Products.Price,
                    ImagePath = ci.Products.ImagePath
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(item => item.Products.Price)
            };

            return View("ViewCart", cartViewModel);
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartID == id);
        }
    }
}
