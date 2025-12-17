using System.Security.Claims;
using BillReminderSystem.Data;
using BillReminderSystem.Models;
using BillReminderSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BillReminderSystem.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppEmailSender _emailSender;

        public BillsController(ApplicationDbContext context, IAppEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // -------------------------------------------------------------------
        // LIST ALL BILLS FOR LOGGED-IN USER
        // -------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bills = await _context.Bills
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.DueDate)
                .ToListAsync();

            return View(bills);
        }

        // -------------------------------------------------------------------
        // BILL DETAILS
        // -------------------------------------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bill = await _context.Bills
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (bill == null) return NotFound();

            return View(bill);
        }

        // -------------------------------------------------------------------
        // CREATE (GET)
        // -------------------------------------------------------------------
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Bill
            {
                DueDate = DateTime.Today.AddDays(1),
                Currency = "USD"
            });
        }

        // -------------------------------------------------------------------
        // CREATE (POST)
        // -------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bill bill)
        {
            bill.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ModelState.Remove("UserId");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                return View(bill);
            }

            try
            {
                _context.Add(bill);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bill created successfully. Check your email for confirmation.";

                // -----------------------------
                // SEND EMAIL CONFIRMATION
                // -----------------------------
                var email = User.Identity?.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    var subject = $"New Bill Added: {bill.Title}";
                    var body =
                        $"Hello,\n\n" +
                        $"You added a new bill to your Bill Reminder System.\n\n" +
                        $"Title   : {bill.Title}\n" +
                        $"Amount  : {bill.Amount:0.00} {bill.Currency}\n" +
                        $"Due Date: {bill.DueDate:d}\n" +
                        $"Category: {bill.Category}\n" +
                        $"Status  : {bill.Status}\n\n" +
                        $"You will receive reminders based on this due date.\n\n" +
                        $"Regards,\nBill Reminder System";

                    await _emailSender.SendAsync(email, subject, body);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong while saving the bill.";
                return View(bill);
            }
        }

        // -------------------------------------------------------------------
        // EDIT (GET)
        // -------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (bill == null) return NotFound();

            return View(bill);
        }


        // -------------------------------------------------------------------
        // EDIT (POST)
        // -------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bill updatedBill)
        {
            if (id != updatedBill.Id) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (bill == null) return NotFound();

            ModelState.Remove("UserId");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                return View(updatedBill);
            }

            try
            {
                bill.Title = updatedBill.Title;
                bill.Amount = updatedBill.Amount;
                bill.Currency = updatedBill.Currency;
                bill.DueDate = updatedBill.DueDate;
                bill.Category = updatedBill.Category;
                bill.Status = updatedBill.Status;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Bill updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Something went wrong while updating the bill.";
                return View(updatedBill);
            }
        }


        // -------------------------------------------------------------------
        // DELETE (GET)
        // -------------------------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bill = await _context.Bills
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (bill == null) return NotFound();

            return View(bill);
        }


        // -------------------------------------------------------------------
        // DELETE (POST)
        // -------------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (bill != null)
            {
                _context.Bills.Remove(bill);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Bill deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Could not find the bill to delete.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}