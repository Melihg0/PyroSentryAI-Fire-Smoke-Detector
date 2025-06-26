using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

namespace PyroSentryAI.ViewModels
{

    public partial class MainViewModel : ObservableObject

    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private object _currentView;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            //Newleme yerine DI Container'dan alıyoruz artık.
            // Uygulama ilk açıldığında ana sayfayı göster
            CurrentView = _serviceProvider.GetRequiredService<HomeViewModel>();
        }
        [RelayCommand] 
        private void NavigateToHome()
        {
            CurrentView = _serviceProvider.GetRequiredService<HomeViewModel>();
        }

        [RelayCommand] 
        private void NavigateToLogs()
        {
            CurrentView = _serviceProvider.GetRequiredService<LogsViewModel>();
        }

        [RelayCommand] 
        private void NavigateToSettings()
        {
            CurrentView = _serviceProvider.GetRequiredService<SettingsViewModel>();
        }




    }
}