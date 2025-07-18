﻿using Microsoft.Extensions.DependencyInjection;
using PyroSentryAI.ViewModels; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PyroSentryAI.Views
{

    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            var viewModel = App.AppHost.Services.GetRequiredService<MainViewModel>();
            this.DataContext = viewModel;
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   //Oluşturdugmuz pencerenin başlık çubuğuna tıklandığında pencerenin taşınmasını sağlar.
            this.DragMove();
            
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            // Bu pencereyi Simge Durumuna küçült.
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
