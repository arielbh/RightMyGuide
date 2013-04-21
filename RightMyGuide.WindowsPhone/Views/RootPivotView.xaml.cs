using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
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
    public partial class RootPivotView : NavigationPage
    {
        public RootPivotView()
        {
            InitializeComponent();
            ViewModel = new RootPivotViewModel();
            PhoneApplicationService.Current.Deactivated += AppDeactivated;
            PhoneApplicationService.Current.Activated += AppActivated;

            Loaded += delegate
            {
                int index;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("mainPivotIndex", out index))
                    pivot.SelectedIndex = index;
            };
        }

        private void AppActivated(object sender, ActivatedEventArgs e)
        {
            RestorePivotIndex();
        }

        void AppDeactivated(object sender, DeactivatedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["mainPivotIndex"] = pivot.SelectedIndex;
        }

        private void RestorePivotIndex()
        {
            if (PhoneApplicationService.Current.State.ContainsKey("mainPivotIndex"))
            {
                pivot.SelectedIndex = (int)PhoneApplicationService.Current.State["mainPivotIndex"];
            }
        }
  

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PhoneApplicationService.Current.State["mainPivotIndex"] = pivot.SelectedIndex;
        }

        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            (ViewModel as RootPivotViewModel).ActivatePivotItem((sender as Pivot).SelectedIndex);
        }
    }
}