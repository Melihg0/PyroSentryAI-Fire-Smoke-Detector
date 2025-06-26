using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PyroSentryAI.Models; // DbContext'imiz bu namespace içinde
using PyroSentryAI.Services.Implementations;
using PyroSentryAI.Services.Interfaces;
using PyroSentryAI.ViewModels; 
using PyroSentryAI.Views; 
using System.Configuration;
using System.Data;
using System.Windows;

namespace PyroSentryAI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }
        public App()
        {
            Core.Initialize();
          
           
            //Genel Müdürün hazır olan el kitabına ConfigureServices((context, services) services parametresi ile personelleri ve görevlerini yazar.
            //DI Container'ını oluşturur ve yapılandırır.
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((context, services) => {

                string connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContextFactory<PyroSentryAiDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                var vlcOptions = new string[] { "--no-osd", "--no-snapshot-preview" };
                services.AddSingleton(new LibVLC(vlcOptions));

                //Singleton tek bir örneği kullanmasını sağlar. Yani uygulama boyunca tek bir AuthenticationService örneği olacak.
                services.AddSingleton<IAuthenticationService, AuthenticationService>();
                services.AddSingleton<IAIAnalysisService, AIAnalysisService>();
                services.AddScoped<IDatabaseService, DatabaseService>();
                services.AddSingleton<ICameraViewModelFactory, CameraViewModelFactory>();
                //Singleton bir servis kendisinden daha kısa ömürlü olan servislere bagımlı olamaz o yuzden scoped kullandık. burada 
                //ViewModel'leri ve view'leri tanıt
                services.AddTransient<LoginViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<HomeViewModel>();
                services.AddTransient<LogsViewModel>();
                services.AddTransient<SettingsViewModel>();
                

                services.AddTransient<LoginView>();
                services.AddTransient<MainView>();
                //DataTemplate yapısı ile yukardaki 3 ViewModel'in hangi View ile eşleşeceğini belirlenir.Bu iş xaml dünyasında olur.
                //Viewdan ViewModele olsaydı burada c# ile temsil ederdik.
                services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default); //WeakReferenceMessenger, ViewModel'ler arasında mesajlaşmayı sağlar.
                services.AddSingleton<IServiceProvider>(sp => sp); //Burası artık bağımsızlıgında bağımsızlıgı kımse AppHost.Services'e bağımlı degıl artık.
            })
                .Build();
        }
        //Override ile ata sınıfı ezdik.
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            var startupForm = AppHost.Services.GetRequiredService<LoginView>();//komi
            startupForm.Show();
            base.OnStartup(e);
            //ezilmemiş halinide base ile çalıştırdık. sağlama aldık
        }
    }
    
}

