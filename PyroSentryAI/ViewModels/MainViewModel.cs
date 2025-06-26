using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PyroSentryAI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<CameraViewModel> Cameras { get; } = new();

        public MainViewModel()
        {
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