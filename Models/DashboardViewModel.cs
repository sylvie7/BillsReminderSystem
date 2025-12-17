using System.Collections.Generic;

namespace BillReminderSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalBills { get; set; }
        public int OverdueBills { get; set; }
        public int DueSoonBills { get; set; }
        public int PaidBills { get; set; }

        public List<Bill> UpcomingBills { get; set; } = new List<Bill>();
    }
}