using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RightMyGuide.WindowsPhone.ViewModels;
using SuiteValue.UI.WP8;

namespace RightMyGuide.WindowsPhone.Views
{
    public partial class SearchView : NavigationPage
    {
        public SearchView()
        {
            InitializeComponent();
        }

        private void PhoneTextBox_OnActionIconTapped(object sender, EventArgs e)
        {
            (ViewModel as SearchViewModel).Search((sender as PhoneTextBox).Text);
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (ViewModel as SearchViewModel).Search((sender as PhoneTextBox).Text);
            }
        }

        private void LongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (LongListSelector)sender;
            list.SelectedItem = null;
        }
    }
}