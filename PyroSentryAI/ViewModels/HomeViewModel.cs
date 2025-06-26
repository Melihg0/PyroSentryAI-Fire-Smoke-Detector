using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.ViewModels
{
    class HomeViewModel : ObservableObject
    {
        public ObservableCollection<CameraViewModel> Cameras { get; } = new();
        public HomeViewModel()
        {
            // Test kameraları yükle
            LoadTestCameras();
        }
        private void LoadTestCameras()
        {

            Cameras.Add(new CameraViewModel("Kamera 1", "CornflowerBlue"));
            Cameras.Add(new CameraViewModel("Kamera 2", "DarkGoldenrod"));
            Cameras.Add(new CameraViewModel("Kamera 3", "IndianRed"));
            Cameras.Add(new CameraViewModel("Kamera 4", "LightGreen"));
            Cameras.Add(new CameraViewModel("Kamera 3", "IndianRed"));
            Cameras.Add(new CameraViewModel("Kamera 4", "LightGreen"));



        }
    }
}
    
    