using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PyroSentryAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PyroSentryAI.ViewModels.SettingsViewModel;

namespace PyroSentryAI.ViewModels
{
    class HomeViewModel : ObservableObject, IRecipient<CameraAddedMessage>, IRecipient<CameraStatusChangedMessage>
    {
        public ObservableCollection<CameraViewModel> Cameras { get; } = new();
        public HomeViewModel()
        {
            // Test kameraları yükle
            LoadTestCameras();
        }
        private readonly IDatabaseService _dbService;
        private readonly IMessenger _messenger;
        private void LoadTestCameras()
        {

            Cameras.Add(new CameraViewModel("Kamera 1", "CornflowerBlue"));
            Cameras.Add(new CameraViewModel("Kamera 2", "DarkGoldenrod"));
            Cameras.Add(new CameraViewModel("Kamera 3", "IndianRed"));
            Cameras.Add(new CameraViewModel("Kamera 4", "LightGreen"));
            Cameras.Add(new CameraViewModel("Kamera 3", "IndianRed"));
            Cameras.Add(new CameraViewModel("Kamera 4", "LightGreen"));



        }

        public void Receive(CameraStatusChangedMessage message)
        {
            throw new NotImplementedException();
        }

        public void Receive(CameraAddedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
    
    