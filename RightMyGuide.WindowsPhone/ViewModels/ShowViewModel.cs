using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using RightMyGuide.DataAccess.ServiceReference;
using RightMyGuide.WindowsPhone.Model;
using SuiteValue.UI.MVVM;
using SuiteValue.UI.WP8;
using Windows.Phone.System.UserProfile;

namespace RightMyGuide.WindowsPhone.ViewModels
{
    public class ShowViewModel : NavigationViewModelBase
    {
        private List<AlphaKeyGroup<string>> _groupedActors;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationMode mode,
                                              IDictionary<string, string> parameter,
                                              bool isNavigationInitiator)
        {
            base.OnNavigatedTo(mode, parameter, isNavigationInitiator);
            string id;
            if (parameter != null && parameter.ContainsKey("id") && !isNavigationInitiator)
            {
                id = parameter["id"];
            }
            else
            {
                id = Show.Id;
            }
            App.IMdbServiceClient.GetShowByIdCompleted += IMdbServiceClient_GetShowByIdCompleted;
            App.IMdbServiceClient.GetReviewsCompleted += IMdbServiceClient_GetReviewsCompleted;
            try
            {
                App.IMdbServiceClient.GetShowByIdAsync(id, true, false);
                App.IMdbServiceClient.GetReviewsAsync(id);
            }
            catch (Exception ex)
            {

            }

        }

        private void IMdbServiceClient_GetReviewsCompleted(object sender, GetReviewsCompletedEventArgs e)
        {
            App.IMdbServiceClient.GetReviewsCompleted -= IMdbServiceClient_GetReviewsCompleted;
            if (e.Cancelled || e.Error != null) return;
            Reviews = e.Result;

        }

        public TVShowReviews Reviews
        {
            get { return _reviews; }
            set
            {
                _reviews = value;
                OnPropertyChanged(() => Reviews);
            }
        }

        private void IMdbServiceClient_GetShowByIdCompleted(object sender, GetShowByIdCompletedEventArgs e)
        {
            App.IMdbServiceClient.GetShowByIdCompleted -= IMdbServiceClient_GetShowByIdCompleted;
            Show = e.Result;

            GroupedActors = AlphaKeyGroup<string>.CreateGroups(Show.Actors, CultureInfo.CurrentUICulture,
                                                               (p) => { return p; }, true);


        }

        public List<AlphaKeyGroup<string>> GroupedActors
        {
            get { return _groupedActors; }
            set
            {
                _groupedActors = value;
                OnPropertyChanged(() => GroupedActors);
            }
        }

        private TVShow _show;
        private TVShowReviews _reviews;

        public TVShow Show
        {
            get { return _show; }
            set
            {
                if (value != _show)
                {
                    _show = value;
                    OnPropertyChanged(() => Show);
                    //Doesn't look good
                    //  UpdateBackground();
                }
            }
        }

        private DelegateCommand _addToFavoriteCommand;

        public DelegateCommand AddToFavoriteCommand
        {
            get
            {
                return _addToFavoriteCommand ?? (_addToFavoriteCommand = new DelegateCommand(
                                 async () =>
                                 {
                                     await App.FavoritesService.AddShowToFavorite(Show);
                                 }));
            }
        }


    }
}