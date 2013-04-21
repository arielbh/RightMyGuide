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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationMode mode, System.Collections.Generic.IDictionary<string, string> parameter, bool isNavigationInitiator)
        {
            base.OnNavigatedTo(mode, parameter, isNavigationInitiator);
            StartTileLiveAgent();

        }

        private void StartTileLiveAgent()
        {
            try
            {
                var taskName = "PeriodicAgent";
                var periodicTask = ScheduledActionService.Find(taskName) as PeriodicTask;
                if (periodicTask != null)
                {
                    ScheduledActionService.Remove(taskName);
                }
                periodicTask = new PeriodicTask(taskName) { Description = "Right My Guide tile notifications agent." };

                ScheduledActionService.Add(periodicTask);
#if(!DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(taskName, TimeSpan.FromSeconds(20));
#endif

            }
            catch (Exception ex)
            {

            }
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