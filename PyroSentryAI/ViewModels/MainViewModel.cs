using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using PyroSentryAI.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;

namespace PyroSentryAI.ViewModels
{

    public partial class MainViewModel : ObservableObject, IRecipient<GlobalAlarmInfoMessage>

    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private bool _isAlarmBannerVisible; // Rozeti göster gizle

        [ObservableProperty]
        private bool _isAlarmPanelOpen;  //Rozete tıklandığında alarm panelini açma kapama

        [ObservableProperty]
        private string _alarmBadgeText;  // Rozette gösterilecek metin

        public ObservableCollection<string> AlarmPanelDetails { get; } = new(); //Rozete tıklandığında gösterilecek detaylar

        public MainViewModel(IServiceProvider serviceProvider, IMessenger messenger)
        {
            _serviceProvider = serviceProvider;
            //Newleme yerine DI Container'dan alıyoruz artık.
            // Uygulama ilk açıldığında ana sayfayı göster
            CurrentView = _serviceProvider.GetRequiredService<HomeViewModel>();
            _messenger = messenger;
            _messenger.RegisterAll(this);
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
        private async Task NavigateToSettings()
        {
            var viewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
            await viewModel.LoadInitialDataAsync(); // Verileri yüklemeden geçme
            CurrentView = viewModel;
        }
        [RelayCommand]
        private void ToggleAlarmPanel()
        {
            IsAlarmPanelOpen = !IsAlarmPanelOpen;
        }

        public async void Receive(GlobalAlarmInfoMessage message)
        {
            IsAlarmBannerVisible = message.IsAlarmActive;
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (message.IsAlarmActive)
                {
                    int alarmCount = message.AlarmDetails.Count;
                    AlarmBadgeText = $"{alarmCount} AKTİF ALARM";
                    AlarmPanelDetails.Clear();

                    foreach (var detail in message.AlarmDetails)
                    {
                        AlarmPanelDetails.Add(detail);
                    }

                }
                else
                {
                    IsAlarmPanelOpen = false;
                    AlarmPanelDetails.Clear();
                }
            });
        }
    }
}