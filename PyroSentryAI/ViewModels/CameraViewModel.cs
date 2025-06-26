using CommunityToolkit.Mvvm.ComponentModel;

namespace PyroSentryAI.ViewModels
{
    public partial class CameraViewModel : ObservableObject
    {
        public string CameraName { get; set; }
        public string PlaceholderColor { get; set; }
        public CameraViewModel(string name, string color)
        {
            this.CameraName = name;
            this.PlaceholderColor = color;
        }
    }
}

