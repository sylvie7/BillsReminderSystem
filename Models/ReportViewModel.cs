using System;
using System.Collections.Generic;

namespace BillReminderSystem.Models
{
    public class CategorySummary
    {
        public string Category { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal Pending { get; set; }
    }

    public class ReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }

        public List<CategorySummary> Categories { get; set; } = new List<CategorySummary>();
    }
}