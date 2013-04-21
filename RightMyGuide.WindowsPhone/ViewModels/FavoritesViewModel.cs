using System.Collections.ObjectModel;
using System.Windows;
using RightMyGuide.DataAccess.ServiceReference;
using SuiteValue.UI.MVVM;
using SuiteValue.UI.WP8;

namespace RightMyGuide.WindowsPhone.ViewModels
{
    public class FavoritesViewModel : CommandableViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IAsyncViewModel _asyncViewModel;

        public FavoritesViewModel(INavigator navigator, IAsyncViewModel asyncViewModel)
        {
            _navigator = navigator;
            _asyncViewModel = asyncViewModel;
            Results = new ObservableCollection<TVShow>();
            App.IMdbServiceClient.GetShowsByIdsCompleted += IMdbServiceClient_GetShowsByIdsCompleted;
        }

        void IMdbServiceClient_GetShowsByIdsCompleted(object sender, GetShowsByIdsCompletedEventArgs e)
        {
            if (e.UserState == this)
            {
                if (e.Cancelled || e.Error != null) return;
                if (e.Result != null)
                {
                    foreach (var show in e.Result)
                        Results.Add(show);
                }
                _asyncViewModel.IsInAsync = false;
            }
        }
        public async override void Activate()
        {
            base.Activate();
            NoItemsVisibility = Visibility.Collapsed;
            Results.Clear();

            _asyncViewModel.IsInAsync = true;
            _asyncViewModel.AsyncMessage = "Loading favorites...";
            var ids = await App.FavoritesService.GetFavorites();
            if (ids == null || (ids.Length == 0))
            {
                _asyncViewModel.IsInAsync = false;
                NoItemsVisibility = Visibility.Visible;
                
                return;
            }
            App.IMdbServiceClient.GetShowsByIdsAsync(new ObservableCollection<string>(ids), false, false, this);
        }

        private ObservableCollection<TVShow> _results;

        public ObservableCollection<TVShow> Results
        {
            get { return _results; }
            set
            {
                if (value != _results)
                {
                    _results = value;
                    OnPropertyChanged(() => Results);
                }
            }
        }

        private Visibility _noItemsVisibility;

        public Visibility NoItemsVisibility
        {
            get { return _noItemsVisibility; }
            set
            {
                if (value != _noItemsVisibility)
                {
                    _noItemsVisibility = value;
                    OnPropertyChanged(() => NoItemsVisibility);
                }
            }
        }

        private TVShow _selectedShow;

        public TVShow SelectedShow
        {
            get { return _selectedShow; }
            set
            {
                if (value != _selectedShow)
                {
                    _selectedShow = value;
                    OnPropertyChanged(() => SelectedShow);
                    if (SelectedShow != null)
                    {
                        _navigator.Navigate(new ShowViewModel { Show = SelectedShow });
                    }
                    SelectedShow = null;
                }
            }
        }
         
    }
}