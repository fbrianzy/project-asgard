# Notes Desktop (WPF, MVVM Toolkit, .NET 8)

A minimal but clean WPF notes app using MVVM (CommunityToolkit.Mvvm).  
Features: add / update / remove notes, two-way binding, observable collection, and commands.

## Tech
- .NET 8
- WPF (`UseWPF`)
- CommunityToolkit.Mvvm (ObservableObject, RelayCommand, source generators)

## Run
```bash
dotnet run --project src/Notes.App
```
## Build
```bash
dotnet build src/Notes.App
```

## Structure
```bash
src/Notes.App/
  App.xaml, App.xaml.cs
  MainWindow.xaml, MainWindow.xaml.cs
  Models/Note.cs
  ViewModels/MainViewModel.cs
```


---

