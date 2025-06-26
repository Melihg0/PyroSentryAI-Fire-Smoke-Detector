using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PyroSentryAI.Services.Interfaces;

namespace PyroSentryAI.ViewModels
{
    public partial class LoginViewModel: ObservableObject
    {
        private readonly IAuthenticationService _authService;

        private string _password;

        public event Action LoginSuccess;                    // Giriş başarılı olduğunda tetiklenecek olay tanımlanır.evente abone olan herkes o tetiklendiğinde 
                                                             //Haberdar olur. Bu olay LoginView'de tetiklenecek.

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginasyncCommand))] //_isLoggingIn) değeri her değiştiğinde, git ve LoginasyncCommand isimli komuta özel bir emir gönder.
                                                                //Emir şu: CanExecute durumunu yeniden kontrol et

        private bool _isLoggingIn;                           // Girisde bekleme durumunu belirtir.Butona tekrar tekrar basmayı engeller.

        public bool IsNotLoggingIn => !_isLoggingIn;         //tersini IsNotLoggingIn olarak tanımladık.Bu bir expression-bodied property'dir.


        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;


        public LoginViewModel(IAuthenticationService authService)
        {
            _authService = authService;
        }
        public void SetPassword(string password)
        {
            _password = password;   // her harf girildiğinde güncellenir.
        }

        /// <summary>
        /// Girdileri kontrol eder ve geçerliyse giriş denemesi yapar.
        /// </summary>
        [RelayCommand(CanExecute = nameof(IsNotLoggingIn))]
        private async Task Loginasync()     
        {
            IsLoggingIn = true;
            try
            {
                HasError = false;
                ErrorMessage = "";

                if (!ValidateInput())
                {
                    return;    // Doğrulama başarısızsa işlemi sonlandır.
                }

                bool isValid = await _authService.AuthenticateAsync(Username, _password);
                if (isValid)
                {
                    LoginSuccess?.Invoke(); //Invoke() metodu ile LoginSuccess olayını tetikleriz.'?' ile de eğer LoginSuccess olayına abone olan varsa tetiklenmesini sağlarız. 
                }                           //Eğer hiç kimse abone değilse, hata almayız.Biz '+=' ile abone olmamızı sağladık.
                else
                {
                    HasError = true;
                    ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Beklenmedik bir hata oluştu: {ex.Message}";
            }
            finally
            {
                IsLoggingIn = false; 
            }
        }

        /// <summary>
        /// Kullanıcı adı ve şifrenin temel kurallara uygunluğunu kontrol eder.
        /// Geçerliyse true, değilse false döndürür ve hata mesajını ayarlar.
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                HasError = true;
                ErrorMessage = "Lütfen kullanıcı adınızı girin.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(_password))
            {
                HasError = true;
                ErrorMessage = "Lütfen şifrenizi girin.";
                return false;
            }

            if (_password.Length < 6)
            {
                HasError = true;
                ErrorMessage = "Şifre en az 6 karakter olmalıdır.";
                return false;
            }

            return true;
        }
    }
}
