using System.Configuration;
using System.Data;
using System.Windows;
using LibVLCSharp.Shared;

namespace PyroSentryAI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Bu, uygulamanın çalışması için gereken en önemli adımlardan biridir.
            // Diğer her şeyden önce VLC motorunu başlatır.
            Core.Initialize();
        }
    }
}

