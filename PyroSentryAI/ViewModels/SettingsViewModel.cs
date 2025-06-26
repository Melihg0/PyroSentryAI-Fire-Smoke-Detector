using CommunityToolkit.Mvvm.ComponentModel;
using PyroSentryAI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PyroSentryAI.ViewModels
{
    public partial class SettingsViewModel: ObservableObject
    {
        //Geçici Degisikliler.
        [ObservableProperty]
        private ObservableCollection<TblCamera> _cameras = new();

        public List<int> FpsOptions { get; } = new List<int> { 1, 2, 4, 5, 8, 10 };

        public SettingsViewModel()
        {
            Cameras = new ObservableCollection<TblCamera>
                {
                    new TblCamera { CameraId = 1, CameraName = "Lobi Kamerası", IsActive = true, Rtspurl = "rtsp://..." },
                    new TblCamera { CameraId = 2, CameraName = "Otopark Giriş", IsActive = true, Rtspurl = "rtsp://..." },
                    new TblCamera { CameraId = 3, CameraName = "Koridor 7 (Pasif)", IsActive = false, Rtspurl = "rtsp://..." }
                };
        }
    }
}
