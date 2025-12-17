using System.Diagnostics;
using System.Security.Claims;
using BillReminderSystem.Data;
using BillReminderSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BillReminderSystem.Controllers
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
            // If not logged in, just show a simple welcome page
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View(model: null);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var today = DateTime.Today;
            var threeDaysFromNow = today.AddDays(3);

            var billsQuery = _context.Bills.Where(b => b.UserId == userId);

            var totalBills = await billsQuery.CountAsync();
            var overdueBills = await billsQuery.CountAsync(b => b.Status != BillStatus.Paid && b.DueDate < today);
            var dueSoonBills = await billsQuery.CountAsync(b =>
                b.Status != BillStatus.Paid &&
                b.DueDate >= today &&
                b.DueDate <= threeDaysFromNow);
            var paidBills = await billsQuery.CountAsync(b => b.Status == BillStatus.Paid);

            var upcomingBills = await billsQuery
                .Where(b => b.Status != BillStatus.Paid && b.DueDate >= today)
                .OrderBy(b => b.DueDate)
                .Take(5)
                .ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalBills = totalBills,
                OverdueBills = overdueBills,
                DueSoonBills = dueSoonBills,
                PaidBills = paidBills,
                UpcomingBills = upcomingBills
            };

            return View(vm);
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