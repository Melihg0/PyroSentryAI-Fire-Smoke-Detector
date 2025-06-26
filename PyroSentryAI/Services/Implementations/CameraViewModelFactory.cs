using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using PyroSentryAI.Models;
using PyroSentryAI.Services.Interfaces;
using PyroSentryAI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PyroSentryAI.Services.Implementations
{
    public class CameraViewModelFactory : ICameraViewModelFactory
    {
        private readonly IAIAnalysisService _aiAnalysisService;
        private readonly IMessenger _messenger;
        private readonly LibVLC _vlcEngine;

        public CameraViewModelFactory(IAIAnalysisService aiAnalysisService, IMessenger messenger, LibVLC vlcEngine)
        {
            _aiAnalysisService = aiAnalysisService;
            _messenger = messenger;
            _vlcEngine = vlcEngine;
        }
        public CameraViewModel Create(TblCamera camera, bool initializePlayer = true)
        {
            return new CameraViewModel(camera, _aiAnalysisService, _messenger, _vlcEngine, initializePlayer);
        }
    }
}
