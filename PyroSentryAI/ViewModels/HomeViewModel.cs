using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PyroSentryAI.Services.Implementations;
using PyroSentryAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PyroSentryAI.Messages;
using System.Windows.Threading;

namespace PyroSentryAI.ViewModels
{
    public partial class HomeViewModel : ObservableObject, IRecipient<CameraAddedMessage>, IRecipient<CameraStatusChangedMessage>, IRecipient<AlarmStateChangedMessage>
    {
        public ObservableCollection<CameraViewModel> Cameras { get; } = new();

        private readonly IDatabaseService _dbService;
        private readonly IMessenger _messenger;
        private readonly ICameraViewModelFactory _cameraFactory;
        private readonly DispatcherTimer _alarmCooldownTimer;

        [ObservableProperty]
        private bool _isGlobalAlarmActive;
        public ObservableCollection<string> ActiveAlarmDisplayTexts { get; } = new();

        public HomeViewModel(IDatabaseService dbService, IMessenger messenger, ICameraViewModelFactory cameraFactory)
        {
            _dbService = dbService;
            _messenger = messenger;
            _cameraFactory = cameraFactory;
            // Bu ViewModel'i gelen tüm mesajları dinlemesi için kaydediyoruz 
            _messenger.RegisterAll(this);

            // ViewModel ilk oluşturulduğunda aktif kameraları yüklüyoruz
            LoadActiveCamerasAsync();

            _alarmCooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Alarm 5 saniye sonra kapanacak
            };
            _alarmCooldownTimer.Tick += OnAlarmCooldownFinished;    //Burada tanımlıyoruz sadece _alarmCooldownTimer'ın .Tick olayının abone listesini bul
                                                                    // ve bu listeye(+=) ile OnAlarmCooldownFinished metodunun adresini ekle.
        }
        public async void Receive(CameraStatusChangedMessage message)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    var updatedCamera = message.UpdatedCamera;
                    var existingCameraVM = Cameras.FirstOrDefault(c => c.CameraId == updatedCamera.CameraId);
                    if (updatedCamera.IsActive != true)
                    {
                        if (existingCameraVM != null)
                        {
                            existingCameraVM.Cleanup();  // Kaynakları temizle
                            Cameras.Remove(existingCameraVM); // Koleksiyondan kaldır
                        }

                    }
                    else
                    {
                        if (existingCameraVM == null)
                        { // 1. ÖNLEYİCİ SAVUNMA: URL'yi VlcEngine'e göndermeden önce doğrula.
                            if (!Uri.IsWellFormedUriString(updatedCamera.Rtspurl, UriKind.Absolute))
                            {
                                // Anlamlı bir hata mesajı oluştur.
                                throw new ArgumentException($"Geçersiz RTSP adresi: '{updatedCamera.Rtspurl}'");
                            }
                            var cameraVM = _cameraFactory.Create(updatedCamera);
                            Cameras.Add(cameraVM);
                            cameraVM.StartAnalysis();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kamera aktif edilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        public async void Receive(CameraAddedMessage message)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    var newCamera = message.NewCamera;

                    if (Cameras.Any(c => c.CameraId == newCamera.CameraId))
                    {
                        return;   //Onceden olan bir camera Id'si varsa return.
                    }
                    var cameraVM = _cameraFactory.Create(newCamera);
                    {
                        Cameras.Add(cameraVM);
                        cameraVM.StartAnalysis();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kamera eklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
        public async void Receive(AlarmStateChangedMessage message)
        {
            //Bir metodun hangi thread'de çalıştığını, metodun kendi tanımı (public void Receive...) değil, o metodu çağıran (tetikleyen) kod belirler.
            //Bu kısma CameraViewModel'den alarm mesageleri gelir ama AI işlemi arka plan threadinde çalıştığı için bu kısımda arka plan threadinde çalışır.
            //Arka plan threadinden arayüz değiiklikleri yapamayız.Bu yüzden UI thread'ine geçiyoruz.
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
            //Eski alarmlar listesini hazırla ve sil.

            var oldAlarmsForThisCamera = ActiveAlarmDisplayTexts
                .Where(text => text.StartsWith(message.CameraName)).ToList();

            foreach (var oldAlarm in oldAlarmsForThisCamera)
            {
                ActiveAlarmDisplayTexts.Remove(oldAlarm);
            }

            foreach (var label in message.DetectedLabels)
            {
                string newAlarmText = $"{message.CameraName} - {label.ToUpper()}";
                    if (!ActiveAlarmDisplayTexts.Contains(newAlarmText))
                    {
                        ActiveAlarmDisplayTexts.Add(newAlarmText);
                    }

                }
                // Kameraların alarm bayraklarını, güncel listeye göre ayarla bu foreach ile çerçeveleri 5 saniye boyunca kırmızı yapıyoruz. IsInAlarmState özelliği cameraViewModel'de tanımlı.
                if (message.DetectedLabels.Any())
                {
                    var alarmCamera = Cameras.FirstOrDefault(c => c.CameraId == message.CameraId);
                    if (alarmCamera != null)
                    {
                        alarmCamera.IsInAlarmState = true;
                    }
                }

                bool isAnyAlarmActiveNow = ActiveAlarmDisplayTexts.Any();

                if (isAnyAlarmActiveNow) // Eğer listede gösterilecek en az bir alarm metni varsa sayacı durdur
                {
                    _alarmCooldownTimer.Stop();

                    if (!IsGlobalAlarmActive)
                    {
                        IsGlobalAlarmActive = true;
                        _messenger.Send(new GlobalAlarmInfoMessage(true, ActiveAlarmDisplayTexts.ToList())); //ToList kullanarak devamlı değişen bir colection
                                                                                                             //göndermemiş oluyoruz
                    }

                }
                else // Eğer colectionda metin yoksa timer'ı başlat
                {
                    _alarmCooldownTimer.Start();
                }
            });
        }

        private async void LoadActiveCamerasAsync()
        {
            try
            {
                var activeCamerasFromDb = await _dbService.GetActiveCamerasAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Cameras.Clear();
                    foreach (var dbCamera in activeCamerasFromDb)
                    {
                        try // Her bir kamera için ayrı try-catch, bir kamera patlarsa diğerleri eklenmeye devam etsin.
                        {
                            if (!Uri.IsWellFormedUriString(dbCamera.Rtspurl, UriKind.Absolute))
                            {
                                throw new ArgumentException($"Veritabanından gelen geçersiz RTSP adresi: '{dbCamera.Rtspurl}'");
                            }
                            var cameraVM = _cameraFactory.Create(dbCamera);
                            Cameras.Add(cameraVM);
                            cameraVM.StartAnalysis();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"'{dbCamera.CameraName}' kamerası yüklenemedi: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aktif kameralar yüklenemedi: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void OnAlarmCooldownFinished(object? sender, EventArgs e)
        {
            //C# sinifi eventi oldugu icin burada tanımladım.Button vb olsa xaml'de tanımlardım.
            _alarmCooldownTimer.Stop();

            if (!ActiveAlarmDisplayTexts.Any()) //5 saniye sonunda son kontrol olarak calısıyor.Sigorta olarak
            {
                IsGlobalAlarmActive = false;
                _messenger.Send(new GlobalAlarmInfoMessage(false, new List<string>()));
                // Tüm kameraların alarm bayraklarını kapat
                foreach (var camVM in Cameras)
                {
                    camVM.IsInAlarmState = false;
                }
            }
        }
    }
}

