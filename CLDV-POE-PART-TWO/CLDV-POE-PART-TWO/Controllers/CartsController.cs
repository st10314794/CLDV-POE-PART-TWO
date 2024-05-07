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




        //public async Task<IActionResult> AddToCart(int productId)
        //{
        //    var product = await _context.Products
        //                                .Where(p => p.ProductID == productId && p.InStock)
        //                                .FirstOrDefaultAsync();

        //    if (product == null)
        //    {
        //        return NotFound("Product not available.");
        //    }

        //    product.InStock = false;  // Mark the product as unavailable

        //    var user = await _userManager.GetUserAsync(User);
        //    var cart = await _context.Carts
        //                             .Include(c => c.CartItems)
        //                             .SingleOrDefaultAsync(c => c.UserID == user.Id);

        //    if (cart == null)
        //    {
        //        cart = new Cart { UserID = user.Id };
        //        _context.Carts.Add(cart);
        //    }

        //    cart.CartItems.Add(new CartItem { ProductID = productId });

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("ViewCart");
        //}
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();  // Redirects to login if not logged in

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound("Product not available.");

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

            // Optionally, you can update the product stock status
            cartItem.Products.InStock = true;  // Mark the product as available again if needed

            // Remove the cart item from the database
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            // Redirect to the cart view
            return RedirectToAction("ViewCart");
        }

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

            return View(cartViewModel);
        }



        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartID,UserID")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cart.UserID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartID,UserID")] Cart cart)
        {
            if (id != cart.CartID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartID == id);
        }
    }
}
