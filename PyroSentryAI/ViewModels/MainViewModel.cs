using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace PyroSentryAI.ViewModels
{
    public partial class MainViewModel : ObservableObject

    {
        [ObservableProperty]
        private object _currentView;

        public MainViewModel()
        {

            // Uygulama ilk açıldığında ana sayfayı göster
            CurrentView = new HomeViewModel();
        }
        [RelayCommand] 
        private void NavigateToHome()
        {
            CurrentView = new HomeViewModel();
        }

        [RelayCommand] 
        private void NavigateToLogs()
        {
            CurrentView = new LogsViewModel();
        }

        [RelayCommand] 
        private void NavigateToSettings()
        {
            CurrentView = new SettingsViewModel();
        }




    }
}