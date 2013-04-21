using System;
using Microsoft.Phone.Scheduler;
using RightMyGuide.BackgroundAgent;
using SuiteValue.UI.MVVM;
using SuiteValue.UI.WP8;
using Windows.Phone.Speech.Recognition;

namespace RightMyGuide.WindowsPhone.ViewModels
{
    public class RootPivotViewModel : NavigationViewModelBase
    {
        private CommandableViewModelBase[] _children;
        public RootPivotViewModel()
        {
            MainViewModel = new MainViewModel(this);
            FavoritesViewModel = new FavoritesViewModel(this, this);
            _children = new CommandableViewModelBase[] { MainViewModel, FavoritesViewModel };
        }

   

        private MainViewModel _mainViewModel;

        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set
            {
                if (value != _mainViewModel)
                {
                    _mainViewModel = value;
                    OnPropertyChanged(() => MainViewModel);
                }
            }
        }

        private FavoritesViewModel _favoritesViewModel;

        public FavoritesViewModel FavoritesViewModel
        {
            get { return _favoritesViewModel; }
            set
            {
                if (value != _favoritesViewModel)
                {
                    _favoritesViewModel = value;
                    OnPropertyChanged(() => FavoritesViewModel);
                }
            }
        }

        private object _selectedPivotIndex;

        public object SelectedPivotIndex
        {
            get { return _selectedPivotIndex; }
            set
            {
                if (value != _selectedPivotIndex)
                {
                    _selectedPivotIndex = value;
                    OnPropertyChanged(() => SelectedPivotIndex);
                }
            }
        }

        public void ActivatePivotItem(int selectedIndex)
        {
            _children[selectedIndex].Activate();

        }

     


    }
}