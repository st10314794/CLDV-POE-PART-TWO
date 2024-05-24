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
using Microsoft.AspNetCore.Authorization;
using CLDV_POE_PART_TWO.Enums;

namespace CLDV_POE_PART_TWO.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminIndex");
                }
                else
                {
                    return RedirectToAction("ClientIndex");
                }
            }
            else
            {
                return RedirectToAction("ClientIndex"); // Default view for non-logged in users
            }
        }

        //  [Authorize(Roles = "Admin")]
        ////  GET: Products
        //public async Task<IActionResult> AdminIndex()
        //{
        //    return View(await _context.Order.ToListAsync());
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(OrderStatus? statusFilter)
        {
            // Start the query with the necessary includes
            var query = _context.Order
                                .Include(o => o.User)
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product) // Include related Product if needed
                                .AsQueryable();

            // Apply the status filter if provided
            if (statusFilter.HasValue)
            {
                query = query.Where(o => o.OrderStatus == statusFilter.Value);
            }

            // Store the current filter in ViewData to use in the view
            ViewData["CurrentFilter"] = statusFilter;

            // Execute the query and get the list of orders
            var orders = await query.ToListAsync();

            // Return the view with the list of orders
            return View(orders);
        }



        public async Task<IActionResult> ClientIndex()
        {
            var user = await _userManager.GetUserAsync(User);


            var userOrders = await _context.Order
                .Where(o => o.UserID == user.Id && o.OrderStatus == OrderStatus.Pending)
                .ToListAsync();
            return View(userOrders);
        }

        public async Task<IActionResult> ClientOrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var orders = await _context.Order
                                       .Where(o => o.UserID == user.Id && o.OrderStatus != OrderStatus.Pending)
                                       .ToListAsync();
            return View(orders);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int orderID, OrderStatus orderStatus)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Order.FindAsync(orderID);
            if (order == null)
            {
                return NotFound();
            }



            order.OrderStatus = orderStatus;
            order.ModifiedDate = DateTime.Now;

            _context.Update(order);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(AdminIndex));

        }



        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                 .Include(o => o.User)
                 .Include(o => o.OrderItems)
                 .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (!isAdmin)
            {
                if (order == null || order.UserID != user.Id)
                {
                    return NotFound();
                }
            }


            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }


        //Removed because createfromcart method in checkoutcontroller

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderID,UserID,TotalPrice,OrderDate,Status,CreatedDate,ModifiedDate")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(order);
        //}





        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,UserID,TotalPrice,Status,CreatedDate,ModifiedDate")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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
            return View(order);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product)
                                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderItems != null && order.OrderItems.Any())
            {

                foreach (var orderItem in order.OrderItems)
                {

                    if (orderItem.Product != null)
                    {
                        orderItem.Product.InStock = true; 
                        _context.Products.Update(orderItem.Product); 
                    }

                
                }
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //// GET: Orders/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    Console.WriteLine($"Order Delete Action.");

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _userManager.GetUserAsync(User);

        //    var order = await _context.Order
        //        .FirstOrDefaultAsync(m => m.OrderID == id);
        //    if (order == null || order.UserID != user.Id)
        //    {
        //        return NotFound();
        //    }



        //    return View(order);
        //}


        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var order = await _context.Order
        //                              .Include(o => o.OrderItems)
        //                              .ThenInclude(oi => oi.Product)
        //                              .FirstOrDefaultAsync(o => o.OrderID == id);

        //    //if (order != null)
        //    //{
        //    //    foreach (var orderItem in order.OrderItems)
        //    //    {
        //    //        // Assuming you have a property `Availability` in the Product model
        //    //        orderItem.Product.InStock = true; // Update availability to true
        //    //        _context.Products.Update(orderItem.Product); // Update the product in the context
        //    //    }

        //    //    if (order != null)
        //    //    {
        //    //        // Update the availability of the products before deleting the order
        //    //        foreach (var orderItem in order.OrderItems)
        //    //        {
        //    //            if (orderItem.Product != null)
        //    //            {
        //    //                orderItem.Product.InStock = true; // Update availability to true
        //    //                _context.Products.Update(orderItem.Product); // Update the product in the context
        //    //            }
        //    //        }

        //    //        await _context.SaveChangesAsync();

        //    //    _context.Order.Remove(order);
        //    //    await _context.SaveChangesAsync();
        //    //}
        //    Console.WriteLine($"Order DeleteConfirmed Action.");


        //    if (order != null)
        //    {
        //        foreach (var orderItem in order.OrderItems)
        //        {
        //            if (orderItem.Product != null)
        //            {
        //                orderItem.Product.InStock = true; // Update availability to true
        //                _context.Products.Update(orderItem.Product); // Update the product in the context
        //                Console.WriteLine($"Updated Product {orderItem.Product.ProductID} InStock to true.");
        //            }
        //        }

        //        await _context.SaveChangesAsync();

        //        _context.Order.Remove(order);
        //        await _context.SaveChangesAsync();

        //        Console.WriteLine($"Order {order.OrderID} deleted.");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Order {id} not found.");
        //    }

        //    return RedirectToAction(nameof(Index));
        //}


        // POST: Orders/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {


        //     var order = await _context.Order
        //.Include(o => o.OrderItems)
        //.ThenInclude(oi => oi.Product)
        //.FirstOrDefaultAsync(o => o.OrderID == id);

        //     //var order = await _context.Order
        //     //    .FindAsync(id);



        //     if (order != null)
        //     {
        //         _context.Order.Remove(order);

        //     }

        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }
    }
}
