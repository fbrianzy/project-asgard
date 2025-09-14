namespace FinanceTracker.Models
{
    public enum TransactionType { Income, Expense }

    public class Transaction
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public string Category { get; set; } = "General";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;
    }
}
