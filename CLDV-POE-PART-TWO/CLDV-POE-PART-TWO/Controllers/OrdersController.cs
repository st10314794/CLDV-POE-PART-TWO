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

        [Authorize(Roles = "Admin")]
        // GET: Products
        public async Task<IActionResult> AdminIndex()
        {
            return View(await _context.Order.ToListAsync());
        }

        public async Task<IActionResult> ClientIndex()
        {
            var user = await _userManager.GetUserAsync(User);


            var userOrders = await _context.Order
                .Where(o=> o.UserID == user.Id)
                .ToListAsync();
            return View(userOrders);
        }

        //public ActionResult Index()
        //{
        //    if (User.IsInRole("Admin"))
        //    {
        //        return View("AdminIndex", _context.Order.ToList());
        //    }
        //    else
        //    {
        //        var userOrders = _context.Order.Where(o => o.UserID == User.Identity.Name).ToList();
        //        return View("ClientIndex", userOrders);
        //    }
        //}
        // GET: Orders
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Order.ToListAsync());
        //}

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                 .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            
            if (order == null || order.UserID != user.Id)
            {
                return NotFound();
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

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null || order.UserID != user.Id)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {


            var order = await _context.Order
       .Include(o => o.OrderItems)
       .ThenInclude(oi => oi.Product)
       .FirstOrDefaultAsync(o => o.OrderID == id);

            //var order = await _context.Order
            //    .FindAsync(id);
               

                
            if (order != null)
            {
                _context.Order.Remove(order);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }
    }
}
