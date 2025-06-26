using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using PyroSentryAI.Messages;
using PyroSentryAI.Models;
using PyroSentryAI.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace PyroSentryAI.ViewModels
{
    public partial class CameraViewModel : ObservableObject
    {
        public TblCamera cameraModel { get; }
        private readonly IAIAnalysisService _aiAnalysisService;
        private readonly IMessenger _messenger;
        private readonly LibVLC _vlcEngine;

        public int CameraId
        {
            get
            {
                return cameraModel.CameraId;
            }
        }

        public string CameraName
        {
            get
            {
                return cameraModel.CameraName;
            }
        }

        public MediaPlayer MediaPlayer { get; private set; }


        [ObservableProperty]
        private bool _isAnalysisActive;

        [ObservableProperty]
        private bool _isObjectDetected;
        [ObservableProperty]
        private bool _isActive;
        [ObservableProperty]
        private bool _isInAlarmState;
        private CancellationTokenSource _analysisCts;

        public CameraViewModel(TblCamera camera, IAIAnalysisService aiAnalysisService, IMessenger messenger, LibVLC vlcEngine, bool initializePlayer = true)
        {
            cameraModel = camera;
            _isActive = cameraModel.IsActive ?? false;
            _aiAnalysisService = aiAnalysisService;
            _messenger = messenger;
            _vlcEngine = vlcEngine;
            // Başlangıçta analiz çalışmıyor olacak ve video oynatılacak.
            if (initializePlayer)
            {
                InitializeAndPlay();
            }

        }
        private void InitializeAndPlay()
        {
            if (string.IsNullOrWhiteSpace(cameraModel.Rtspurl) || !Uri.IsWellFormedUriString(cameraModel.Rtspurl, UriKind.Absolute))
            {
                //URI (Uniform Resource Identifier),standart adres formatıdır. rtsp:// gibi bir adres bekliyoruz.
                return;
            }

            // App.xaml.cs'de oluşturacağımız ana VLC motorunu kullanarak...
            var media = new Media(_vlcEngine, new Uri(cameraModel.Rtspurl));
            this.MediaPlayer = new MediaPlayer(media);

            // Videoyu hemen oynatmaya başla.
            MediaPlayer.Play();

        }
        public void Cleanup()
        {
            StopAnalysis();
            // MediaPlayer null değilse, önce videoyu durdur.
            MediaPlayer?.Stop();
            // Sonra kullandığı tüm kaynakları hafızadan temizle.
            MediaPlayer?.Dispose();
        }

        public void StartAnalysis()
        {
            if (IsAnalysisActive)
            {
                return;
            }
            IsAnalysisActive = true;
            _analysisCts = new CancellationTokenSource();
            var token = _analysisCts.Token; //Telsiz Mantıgı
            Task.Run(async () =>
            {
                var stopwatch = new Stopwatch();
                const int targetCycleTimeMs = 250;
                // 5. Sonsuz Devriye Döngüsü: İptal sinyali gelene kadar devam et.
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        stopwatch.Restart();
                        Image<Rgb24> image = null; // Görüntüyü tutacak değişken, başlangıçta boş.
                        string tempSnapshotPath = Path.GetTempFileName() + ".png";
                        MediaPlayer.TakeSnapshot(0, tempSnapshotPath, 0, 0);//vlc kutuphanesi (video_akış_numarası, dosya_yolu, genişlik, yükseklik)
                        try                                                 //0 orjinal bırak demek burada 3. 4. kısımda
                        {
                            if (File.Exists(tempSnapshotPath))
                            {
                                image = Image.Load<Rgb24>(tempSnapshotPath);
                            }
                        }
                        finally
                        {
                            // 3. Ne olursa olsun, dosyayı diskten sil.
                            if (File.Exists(tempSnapshotPath))
                            {
                                File.Delete(tempSnapshotPath);
                            }
                        }

                        if (image != null)
                        {
                            using (image)
                            {
                                var detections = await _aiAnalysisService.DetectObjectsAsync(image);
                                bool isDetectedNow = detections.Any();
                                // Eğer 'detections' listesinin içinde en az bir eleman varsa (.Any()),
                                // IsObjectDetected özelliğini 'true' yap, yoksa 'false' yap.

                                if (IsObjectDetected != isDetectedNow)
                                {
                                    IsObjectDetected = isDetectedNow;
                                    var labels = new List<string>();
                                    if (isDetectedNow)
                                    {
                                        // Tespit edilen nesnelerin etiketlerini listeye ekle.
                                        labels = detections.Select(d => d.Label).ToList();
                                    }

                                    _messenger.Send(new AlarmStateChangedMessage(this.CameraId, this.CameraName, labels));
                                }
                            }
                        }
                        stopwatch.Stop();
                        long elapsedMs = stopwatch.ElapsedMilliseconds;
                        long delay = targetCycleTimeMs - elapsedMs;

                        if (delay > 0)
                        {
                            await Task.Delay((int)delay, token);
                        }

                    }
                    catch (OperationCanceledException)
                    {
                        // İptal sinyali gelince bu hatayı alırız.
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Kamera {CameraName} analiz hatası: {ex.Message}");
                    }
                }

            }, token);
        }

        public void StopAnalysis()
        {
            _analysisCts?.Cancel();
            _analysisCts?.Dispose();
            IsAnalysisActive = false;

            if (IsObjectDetected)
            {
                IsObjectDetected = false;
                _messenger.Send(new AlarmStateChangedMessage(this.CameraId, this.CameraName, new List<string>()));
            }
        }
    }
}

