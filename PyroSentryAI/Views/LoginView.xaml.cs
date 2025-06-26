using Microsoft.Extensions.DependencyInjection;
using PyroSentryAI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PyroSentryAI.Views
{

    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent(); //Textboxlar butonlar vb bununla çizilir.
            var viewModel = App.AppHost.Services.GetRequiredService<LoginViewModel>(); // DI konteynerinden LoginViewModel'i olusturuyoruz buradan
            this.DataContext = viewModel; // LoginViewModel'i binding için DataContext olarak ayarlıyoruz.Aradaki kopru burada kuruluyor.
            viewModel.LoginSuccess += ViewModel_LoginSuccess; //Login başarılı olursa, ViewModel_LoginSuccess() metodunu tetikleyecek şekilde ayarlıyoruz.
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // DataContext'in LoginViewModel olduğunu kontrol ediyoruz.Eğer öyleyse,viewmodel adında yeni bir değişkene atıyoruz.(Yeni bir özellik)
            if (this.DataContext is LoginViewModel viewModel)
            {
                //Sender nesnesi bir passwordbox diyoruz.Password ile şifresini alıyoruz,
                //viewModel'deki SetPassword metoduna iletiyoruz.
                viewModel.SetPassword((sender as PasswordBox).Password);
            }
        }
        private void ViewModel_LoginSuccess()
        {
            // MainView'i DI konteynerinden istiyoruz.
            var mainView = App.AppHost.Services.GetRequiredService<MainView>();
            mainView.Show(); 

            this.Close();    
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            // Bu pencereyi Simge Durumuna küçült.
            this.WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {             
                this.DragMove();
        }
    }
}
