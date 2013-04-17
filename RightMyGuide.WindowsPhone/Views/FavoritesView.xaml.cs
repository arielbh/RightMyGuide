using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace RightMyGuide.WindowsPhone.Views
{
    public partial class FavoritesView : UserControl
    {
        public FavoritesView()
        {
            InitializeComponent();
        }

        private void LongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (LongListSelector)sender;
            list.SelectedItem = null;
        }
    }
}
