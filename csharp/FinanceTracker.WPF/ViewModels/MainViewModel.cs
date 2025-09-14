using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FinanceTracker.Models;
using FinanceTracker.Services;
using System.Linq;

namespace FinanceTracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly JsonStorage _storage = new(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "FinanceTracker", "transactions.json"));

        public ObservableCollection<Transaction> Transactions { get; } = new();

        public IEnumerable<string> Categories { get; } = new[] {
            "General","Food","Transport","Shopping","Bills","Salary","Freelance"
        };

        private Transaction _draft = new();
        public Transaction Draft
        {
            get => _draft;
            set { _draft = value; OnPropertyChanged(); }
        }

        public decimal TotalIncome => Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        public decimal TotalExpense => Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        public decimal Balance => TotalIncome - TotalExpense;

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteSelectedCommand { get; }

        private Transaction? _selected;
        public Transaction? Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); DeleteSelectedCommand.RaiseCanExecuteChanged(); }
        }

        public MainViewModel()
        {
            foreach (var t in _storage.Load()) Transactions.Add(t);
            AddCommand = new RelayCommand(Add);
            DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => Selected != null);
        }

        private void Add()
        {
            Transactions.Add(new Transaction
            {
                Date = Draft.Date,
                Category = Draft.Category,
                Description = Draft.Description,
                Amount = Draft.Amount,
                Type = Draft.Type
            });
            Draft = new Transaction();
            Save();
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpense));
            OnPropertyChanged(nameof(Balance));
        }

        private void DeleteSelected()
        {
            if (Selected != null) Transactions.Remove(Selected);
            Save();
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpense));
            OnPropertyChanged(nameof(Balance));
        }

        private void Save() => _storage.Save(Transactions);

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
