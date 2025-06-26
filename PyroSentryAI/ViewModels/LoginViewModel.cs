using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PyroSentryAI.ViewModels
{
    public partial class LoginViewModel: ObservableObject
    {
        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        /// <summary>
        /// Girdileri kontrol eder ve geçerliyse giriş denemesi yapar.
        /// </summary>
        [RelayCommand]
        private void Login(PasswordBox passwordBox)
        {
            HasError = false;
            ErrorMessage = "";

            string password = passwordBox.Password;

            if (!ValidateInput(password))
            {
                return; 
            }

            // Gerçek giriş denemesi.
            try
            {
                if (Username.ToLower() == "admin" && password == "123456")
                {
                    
                }
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
        }

        /// <summary>
        /// Kullanıcı adı ve şifrenin temel kurallara uygunluğunu kontrol eder.
        /// Geçerliyse true, değilse false döndürür ve hata mesajını ayarlar.
        /// </summary>
        private bool ValidateInput(string password)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                HasError = true;
                ErrorMessage = "Lütfen kullanıcı adınızı girin.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                HasError = true;
                ErrorMessage = "Lütfen şifrenizi girin.";
                return false;
            }

            if (password.Length < 6)
            {
                HasError = true;
                ErrorMessage = "Şifre en az 6 karakter olmalıdır.";
                return false;
            }

            return true;
        }
    }
}
