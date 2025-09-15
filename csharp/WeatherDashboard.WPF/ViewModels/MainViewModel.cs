using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeatherDashboard.Models;
using WeatherDashboard.Services;

namespace WeatherDashboard.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly OpenWeatherService _svc = new();
        private string _city = "Jakarta";
        private string _apiKey = "";
        private WeatherResponse? _current;
        private string? _iconUrl;
        private bool _isLoading;

        public string City { get => _city; set { _city = value; OnPropertyChanged(); } }
        public string ApiKey { get => _apiKey; set { _apiKey = value; OnPropertyChanged(); FetchCommand.RaiseCanExecuteChanged(); } }
        public WeatherResponse? Current { get => _current; set { _current = value; OnPropertyChanged(); } }
        public string? IconUrl { get => _iconUrl; set { _iconUrl = value; OnPropertyChanged(); } }
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

        public ObservableCollection<string> History { get; } = new();

        public RelayCommand FetchCommand { get; }

        public MainViewModel()
        {
            FetchCommand = new RelayCommand(async () => await FetchAsync(), () => !string.IsNullOrWhiteSpace(ApiKey));
        }

        private async Task FetchAsync()
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return;
            try
            {
                IsLoading = true;
                var resp = await _svc.FetchAsync(City, ApiKey);
                Current = resp;
                IconUrl = resp?.Weather?.FirstOrDefault()?.Icon is string i ? _svc.IconUrl(i) : null;
                if (resp?.Name is string nm && !History.Contains(nm)) History.Insert(0, nm);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
