using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PyroSentryAI.Models;
using PyroSentryAI.Services.Implementations;
using PyroSentryAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PyroSentryAI.Messages;


namespace PyroSentryAI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {

        private readonly IDatabaseService _dbService;
        private readonly IMessenger _messenger;
        private readonly ICameraViewModelFactory _cameraFactory;
        //Ayarlar için proplar.
        [ObservableProperty]
        private string _confidenceThreshold; //String tanımladık dikkat.

        [ObservableProperty]
        private int _analysisFps;

        public List<int> FpsOptions { get; } = new List<int> { 1, 2, 4, 5, 8, 10 };

        //Yeni Kamera için proplar.
        [ObservableProperty]
        private string _newCameraName;

        [ObservableProperty]
        private string _newCameraRtspUrl;


        // AYARLAR BÖLÜMÜ İÇİN HATA
        [ObservableProperty]
        private string? settingsErrorMessage; // Hata yazısı için

        [ObservableProperty]
        private bool hasSettingsError; // Görünürlük için


        // KAMERA EKLEME BÖLÜMÜ İÇİN HATA 
        [ObservableProperty]
        private string? cameraErrorMessage;

        [ObservableProperty]
        private bool hasCameraError;

        //Kameralar listesi
        public ObservableCollection<CameraViewModel> Cameras { get;} = new();

        
        public SettingsViewModel(IDatabaseService dbService, IMessenger messenger, ICameraViewModelFactory cameraFactory)
        {
            _dbService = dbService;
            _messenger = messenger;
            _cameraFactory = cameraFactory;

            // ViewModel oluşturulduğunda mevcut verileri yükle
           
        }

        public async Task LoadInitialDataAsync()
        {
            try
            {
                // Ayarları yükle ve ekrandaki alanlara doldur
                var settings = await _dbService.GetSettingsAsync();
                if (settings != null)
                {
                    ConfidenceThreshold = settings.ConfidenceThreshold.ToString(CultureInfo.InvariantCulture);
                    AnalysisFps = settings.AnalysisFps;
                }

                // Kameraları yükle ve ekrandaki listeyi doldur
                var camerasFromDb = await _dbService.GetAllVisibleCamerasAsync();
                Cameras.Clear();
                foreach (var cam in camerasFromDb)
                {
                    Cameras.Add(_cameraFactory.Create(cam,initializePlayer: false));
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Uygulama verileri yüklenemedi: {ex.Message}", "Kritik Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            HasSettingsError = false;
            
            if (string.IsNullOrWhiteSpace(ConfidenceThreshold))
            {
                SettingsErrorMessage = "Hata: Güven Eşiği alanı boş bırakılamaz.";
                HasSettingsError = true;
                return;
            }
            ConfidenceThreshold = ConfidenceThreshold.Replace(',', '.');
            decimal parsedConfidence;
            if (!decimal.TryParse(ConfidenceThreshold,
                          System.Globalization.NumberStyles.Any,
                          System.Globalization.CultureInfo.InvariantCulture,
                          out parsedConfidence))
            {
                SettingsErrorMessage = "Hata: Güven Eşiği alanına geçerli bir sayı girin (örn: 0.75).";
                HasSettingsError = true;
                return;
            }
           if(parsedConfidence < 0 || parsedConfidence > 1)
            {
                SettingsErrorMessage = "Hata: Güven Eşiği 0 ile 1 arasında bir değer olmalıdır.";
                HasSettingsError = true;
                return; // İşlemi durdur
            }

            try
            {
                var settingsToUpdate = await _dbService.GetSettingsAsync();
                if (settingsToUpdate != null)
                {
                    settingsToUpdate.ConfidenceThreshold = parsedConfidence;
                    settingsToUpdate.AnalysisFps = this.AnalysisFps;

                    // Değiştirilmiş nesneyi, artık zorla güncelleme yapacak olan servise yolla
                    await _dbService.UpdateSettingsAsync(settingsToUpdate);

                    MessageBox.Show("Ayarlar başarıyla kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                else
                {
                    MessageBox.Show("Hata: Ayarlar veritabanında bulunamadı. Güncelleme yapılamadı.", "Veritabanı Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                SettingsErrorMessage = $"Ayarlar kaydedilemedi: {ex.Message}";
                HasSettingsError = true;
            }
        }
              
        [RelayCommand]
        private async Task AddCameraAsync()
        {
            HasCameraError = false;
            if (string.IsNullOrWhiteSpace(NewCameraName) || string.IsNullOrWhiteSpace(NewCameraRtspUrl))
            {
                CameraErrorMessage = "Kamera adı ve RTSP adresi boş olamaz.";
                HasCameraError = true;

                return;
            }
            if (!Uri.IsWellFormedUriString(NewCameraRtspUrl, UriKind.Absolute))
            {
                CameraErrorMessage = "Lütfen geçerli bir RTSP adresi girin (örn: rtsp://...).";
                HasCameraError = true;
                return; 
            }
            try
            {
                var newCam = new TblCamera
                {
                    CameraName = this.NewCameraName,
                    Rtspurl = this.NewCameraRtspUrl,
                    IsActive = true // Yeni eklenen kamera varsayılan olarak aktif olsun
                };

                var addedCamera = await _dbService.AddCameraAsync(newCam);


                // Ekrandaki listeyi anında güncellenir. ObservableCollection ile
                Cameras.Add(_cameraFactory.Create(addedCamera, initializePlayer: false));

                // Ekleme sonrası formu temizle
                NewCameraName = "";
                NewCameraRtspUrl = "";

                // Diğer ekranlara haber ver!
                _messenger.Send(new CameraAddedMessage(addedCamera));

            }
            catch
            {
                CameraErrorMessage = "Kamera eklenirken bir hata oluştu. Lütfen RTSP adresini kontrol edin.";
                HasCameraError = true;
                return;
            }
        }

        [RelayCommand]
        private async Task ToggleCameraStatusAsync(CameraViewModel cameraVM ) // Parametre olarak tıklanan kamera gelir
        {
            
            if (cameraVM == null)
                {
                    return;
                }
            try
            {
                cameraVM.IsActive = !cameraVM.IsActive;
                cameraVM.cameraModel.IsActive = cameraVM.IsActive;
                await _dbService.UpdateCameraAsync(cameraVM.cameraModel);
                // Adım 3: Diğer ekranlara haber ver!
                _messenger.Send(new CameraStatusChangedMessage(cameraVM.cameraModel));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kamera durumu güncellenemedi: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
           
        }
    }
}
