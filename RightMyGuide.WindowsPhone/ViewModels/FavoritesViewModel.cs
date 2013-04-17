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
            App.IMdbServiceClient.GetShowByIdCompleted += IMdbServiceClient_GetShowByIdCompleted;
        }

        void IMdbServiceClient_GetShowByIdCompleted(object sender, DataAccess.ServiceReference.GetShowByIdCompletedEventArgs e)
        {
            if (e.UserState == this)
            {
                if (e.Cancelled || e.Error != null) return;
                if (e.Result != null)
                {
                    Results.Add(e.Result);
                }
                if (Results.Count == _expectedToLoad)
                {
                    OnPropertyChanged(() => NoItemsVisibility);
                    _asyncViewModel.IsInAsync = false;
                }
            }
        }
        public async override void Activate()
        {
            base.Activate();
            Results.Clear();

            _asyncViewModel.IsInAsync = true;
            _asyncViewModel.AsyncMessage = "Loading favorites...";
            var ids = await App.FavoritesService.GetFavorites();
            if (ids == null)
            {
                _asyncViewModel.IsInAsync = false;
                OnPropertyChanged(() => NoItemsVisibility);
                return;
            }
            _expectedToLoad = ids.Length;

            foreach (var id in ids)
            {
                
                App.IMdbServiceClient.GetShowByIdAsync(id, false, this);
            }

        }

        private ObservableCollection<TVShow> _results;
        private int _expectedToLoad;

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
        public Visibility NoItemsVisibility { get { return Results.Count == 0 ? Visibility.Visible : Visibility.Collapsed; } }

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