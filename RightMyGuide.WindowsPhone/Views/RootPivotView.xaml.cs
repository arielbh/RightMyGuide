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
    public partial class RootPivotView : NavigationPage
    {
        public RootPivotView()
        {
            InitializeComponent();
            ViewModel = new RootPivotViewModel();

        }

        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            (ViewModel as RootPivotViewModel).ActivatePivotItem((sender as Pivot).SelectedIndex);
        }
    }
}