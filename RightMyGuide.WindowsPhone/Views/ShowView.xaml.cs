using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RightMyGuide.WindowsPhone.ViewModels;
using SuiteValue.UI.WP8;

namespace RightMyGuide.WindowsPhone.Views
{
    public partial class ShowView : NavigationPage
    {
        public ShowView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!e.IsNavigationInitiator && e.NavigationMode == NavigationMode.New)
            {
                ViewModel = new ShowViewModel();
            }
            base.OnNavigatedTo(e);
            
        }

        private void ImageBrush_OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            
        }

        private void ImageBrush_OnImageOpened(object sender, RoutedEventArgs e)
        {
        }
    }
}