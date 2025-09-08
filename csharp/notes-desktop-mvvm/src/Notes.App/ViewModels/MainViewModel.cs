using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Notes.App.Models;
using System.Collections.ObjectModel;

namespace Notes.App.ViewModels;

/// <summary>
/// ViewModel utama: mengelola daftar catatan dan state editor.
/// Memakai MVVM Toolkit (source generator) agar kode ringkas dan strongly-typed.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    // ===== State editor =====
    [ObservableProperty] private string title = "";
    [ObservableProperty] private string content = "";

    // Item terpilih di ListBox
    [ObservableProperty] private Note? selected;

    // Koleksi catatan yang ter-observe (langsung bind ke ListBox)
    public ObservableCollection<Note> Items { get; } = new();

    public MainViewModel()
    {
        // Seed contoh awal biar UI langsung kelihatan
        Items.Add(new Note { Title = "Welcome 👋", Content = "This is your first note.", UpdatedAt = DateTime.Now });
        Items.Add(new Note { Title = "MVVM Toolkit", Content = "Uses ObservableObject + RelayCommand.", UpdatedAt = DateTime.Now });
    }

    // Saat properti Selected berubah, sinkronkan Title/Content editor
    partial void OnSelectedChanged(Note? value)
    {
        if (value is null)
        {
            Title = string.Empty;
            Content = string.Empty;
        }
        else
        {
            Title = value.Title;
            Content = value.Content;
        }
    }

    // ===== Commands =====

    [RelayCommand]
    private void New()
    {
        Selected = null;         // otomatis kosongkan editor via OnSelectedChanged
        Title = string.Empty;    // redundant tapi eksplisit
        Content = string.Empty;
    }

    [RelayCommand]
    private void Add()
    {
        var t = (Title ?? "").Trim();
        if (string.IsNullOrWhiteSpace(t)) return;

        var n = new Note
        {
            Title = t,
            Content = (Content ?? "").Trim(),
            UpdatedAt = DateTime.Now
        };

        Items.Add(n);
        Selected = n; // pilih item baru
    }

    [RelayCommand]
    private void Update()
    {
        if (Selected is null) return;

        var t = (Title ?? "").Trim();
        if (string.IsNullOrWhiteSpace(t)) return;

        Selected.Title = t;
        Selected.Content = (Content ?? "").Trim();
        Selected.UpdatedAt = DateTime.Now;

        // Beritahu UI kalau ada perubahan di item
        OnPropertyChanged(nameof(Items));
    }

    [RelayCommand]
    private void Remove()
    {
        if (Selected is null) return;
        var idx = Items.IndexOf(Selected);
        if (idx >= 0) Items.RemoveAt(idx);

        Selected = null; // kosongkan editor
    }
}
