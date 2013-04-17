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
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace RightMyGuide.WindowsPhone.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void TapOnTile(object sender, GestureEventArgs e)
        {
            (DataContext as MainViewModel).Goto((sender as HubTile).Title);
        }
    }
}