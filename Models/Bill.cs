using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BillReminderSystem.Models
{
    public class Bill
    {
        public int Id { get; set; }

        // Set in controller from logged-in user
        public string UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; } = "USD";

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        public BillStatus Status { get; set; } = BillStatus.Pending;

        // UI-only reminder state
        [NotMapped]
        public BillReminderState ReminderState
        {
            get
            {
                if (Status == BillStatus.Paid) return BillReminderState.None;

                var today = DateTime.Today;
                if (DueDate < today) return BillReminderState.Overdue;

                var days = (DueDate - today).TotalDays;
                if (days <= 3) return BillReminderState.DueSoon;

                return BillReminderState.None;
            }
        }
    }

    public enum BillStatus
    {
        Pending = 0,
        Paid = 1
    }

    public enum BillReminderState
    {
        None = 0,
        DueSoon = 1,
        Overdue = 2
    }
}