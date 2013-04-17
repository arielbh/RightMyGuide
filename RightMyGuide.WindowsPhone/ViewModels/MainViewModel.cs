using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiteValue.UI.MVVM;
using SuiteValue.UI.WP8;

namespace RightMyGuide.WindowsPhone.ViewModels
{
    public class MainViewModel : CommandableViewModelBase
    {
        private readonly INavigator _navigator;

        public MainViewModel(INavigator navigator)
        {
            _navigator = navigator;
        }


        public void Goto(string title)
        {
            if (title == "Search")
            {
                _navigator.Navigate(new SearchViewModel());
            }
            
        }
    }
}
