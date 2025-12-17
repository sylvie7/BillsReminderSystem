using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BillReminderSystem.Data;
using BillReminderSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BillReminderSystem.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // -----------------------------------------------------------
        // MAIN REPORT VIEW
        // -----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var from = fromDate ?? DateTime.Today.AddMonths(-1);
            var to = toDate ?? DateTime.Today;

            var bills = await _context.Bills
                .Where(b => b.UserId == userId &&
                            b.DueDate.Date >= from.Date &&
                            b.DueDate.Date <= to.Date)
                .OrderBy(b => b.DueDate)
                .ToListAsync();

            var model = new ReportViewModel
            {
                FromDate = from,
                ToDate = to,
                TotalAmount = bills.Sum(b => b.Amount),
                TotalPaid = bills.Where(b => b.Status == BillStatus.Paid).Sum(b => b.Amount),
                TotalPending = bills.Where(b => b.Status == BillStatus.Pending).Sum(b => b.Amount),
                Categories = bills
                    .GroupBy(b => b.Category)
                    .Select(g => new CategorySummary
                    {
                        Category = g.Key,
                        Total = g.Sum(b => b.Amount),
                        Paid = g.Where(b => b.Status == BillStatus.Paid).Sum(b => b.Amount),
                        Pending = g.Where(b => b.Status == BillStatus.Pending).Sum(b => b.Amount)
                    })
                    .OrderByDescending(c => c.Total)
                    .ToList()
            };

            return View(model);
        }

        // -----------------------------------------------------------
        // EXPORT REPORT TO PDF
        // -----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> ExportPdf(DateTime? fromDate, DateTime? toDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var from = fromDate ?? DateTime.Today.AddMonths(-1);
            var to = toDate ?? DateTime.Today;

            var bills = await _context.Bills
                .Where(b => b.UserId == userId &&
                            b.DueDate.Date >= from.Date &&
                            b.DueDate.Date <= to.Date)
                .OrderBy(b => b.DueDate)
                .ToListAsync();

            var categories = bills
                .GroupBy(b => b.Category)
                .Select(g => new CategorySummary
                {
                    Category = g.Key,
                    Total = g.Sum(b => b.Amount),
                    Paid = g.Where(b => b.Status == BillStatus.Paid).Sum(b => b.Amount),
                    Pending = g.Where(b => b.Status == BillStatus.Pending).Sum(b => b.Amount)
                })
                .OrderByDescending(c => c.Total)
                .ToList();

            var total = bills.Sum(b => b.Amount);
            var paid = bills.Where(b => b.Status == BillStatus.Paid).Sum(b => b.Amount);
            var pending = bills.Where(b => b.Status == BillStatus.Pending).Sum(b => b.Amount);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    page.Header().Text("Bill Reminder System – Spending Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken4);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Date range: {from:d} to {to:d}").FontSize(12);

                        col.Item().PaddingTop(10).Text(text =>
                        {
                            text.Span("Total Amount: ").SemiBold();
                            text.Span($"{total:0.00}");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Total Paid: ").SemiBold();
                            text.Span($"{paid:0.00}");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Total Pending: ").SemiBold();
                            text.Span($"{pending:0.00}");
                        });

                        col.Item().PaddingVertical(15).Text("By Category").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Category");
                                header.Cell().Element(HeaderCell).Text("Total");
                                header.Cell().Element(HeaderCell).Text("Paid");
                                header.Cell().Element(HeaderCell).Text("Pending");

                                static IContainer HeaderCell(IContainer container)
                                {
                                    return container.DefaultTextStyle(t => t.SemiBold())
                                        .Padding(5)
                                        .Background(Colors.Grey.Lighten3)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Medium);
                                }
                            });

                            foreach (var c in categories)
                            {
                                table.Cell().Element(BodyCell).Text(c.Category);
                                table.Cell().Element(BodyCell).Text($"{c.Total:0.00}");
                                table.Cell().Element(BodyCell).Text($"{c.Paid:0.00}");
                                table.Cell().Element(BodyCell).Text($"{c.Pending:0.00}");
                            }

                            static IContainer BodyCell(IContainer container)
                            {
                                return container.Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text($"Generated on {DateTime.Now:g}")
                        .FontSize(10).FontColor(Colors.Grey.Darken1);
                });
            });

            var pdfBytes = document.GeneratePdf();
            var filename = $"SpendingReport_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            return File(pdfBytes, "application/pdf", filename);
        }
    }
}