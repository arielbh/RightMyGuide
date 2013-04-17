using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RightMyGuide.DataAccess.ServiceReference;
using RightMyGuide.WindowsPhone.Model;
using SuiteValue.UI.MVVM;
using SuiteValue.UI.WP8;

namespace RightMyGuide.WindowsPhone.ViewModels
{
    public class ShowViewModel : NavigationViewModelBase
    {
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationMode mode, System.Collections.Generic.IDictionary<string, string> parameter, bool isNavigationInitiator)
        {
            base.OnNavigatedTo(mode, parameter, isNavigationInitiator);
            App.IMdbServiceClient.GetShowByIdCompleted += IMdbServiceClient_GetShowByIdCompleted;
            App.IMdbServiceClient.GetReviewsCompleted += IMdbServiceClient_GetReviewsCompleted;
            try
            {
                App.IMdbServiceClient.GetShowByIdAsync(Show.Id, true);
                App.IMdbServiceClient.GetReviewsAsync(Show.Id);
            }
            catch (Exception ex)
            {
                
            }
            
        }

        void IMdbServiceClient_GetReviewsCompleted(object sender, GetReviewsCompletedEventArgs e)
        {
            App.IMdbServiceClient.GetReviewsCompleted -= IMdbServiceClient_GetReviewsCompleted;
            if (e.Cancelled || e.Error != null) return;
            Reviews = e.Result;

        }

        public TVShowReviews Reviews
        {
            get { return _reviews; }
            set { _reviews = value; 
            OnPropertyChanged(() => Reviews);}
        }

        void IMdbServiceClient_GetShowByIdCompleted(object sender, GetShowByIdCompletedEventArgs e)
        {
            App.IMdbServiceClient.GetShowByIdCompleted -= IMdbServiceClient_GetShowByIdCompleted;
            Show = e.Result;

            GroupedActors = AlphaKeyGroup<string>.CreateGroups(Show.Actors, CultureInfo.CurrentUICulture, (p) => { return p; }, true);


        }

        public List<AlphaKeyGroup<string>> GroupedActors
        {
            get { return _groupedActors; }
            set { _groupedActors = value;
            OnPropertyChanged(() => GroupedActors);}
        }

        private TVShow _show;

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

        private double _newReviewRating;

        public double NewReviewRating
        {
            get { return _newReviewRating; }
            set
            {
                if (value != _newReviewRating)
                {
                    _newReviewRating = value;
                    OnPropertyChanged(() => NewReviewRating);
                }
            }
        }

        private string _newReview;

        public string NewReview
        {
            get { return _newReview; }
            set
            {
                if (value != _newReview)
                {
                    _newReview = value;
                    OnPropertyChanged(() => NewReview);
                }
            }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    OnPropertyChanged(() => UserName);
                }
            }
        }
        private DelegateCommand _addReviewCommand;

        public DelegateCommand AddReviewCommand
        {
            get
            {
                return _addReviewCommand ?? (_addReviewCommand = new DelegateCommand(
                                                     () =>
                                                     {
                                                         App.IMdbServiceClient.AddReviewAsync(Show.Id, (int)Math.Ceiling(NewReviewRating), UserName, NewReview);
                                                     }));
            }
        }




        private DelegateCommand _addToFavoriteCommand;
        private List<AlphaKeyGroup<string>> _groupedActors;
        private TVShowReviews _reviews;

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




        //private void UpdateBackground()
        //{
        //    var image = new BitmapImage(new Uri(Show.PosterUrl, UriKind.Absolute));
            
        //    var brush = new ImageBrush();
        //    brush.ImageSource = image;
        //    brush.Stretch = Stretch.None;
            
        //    PanoramaBackground = brush;
        //}

        //private Brush _panoramaBackground;

        //public Brush PanoramaBackground
        //{
        //    get { return _panoramaBackground; }
        //    set
        //    {
        //        if (value != _panoramaBackground)
        //        {
        //            _panoramaBackground = value;
        //            OnPropertyChanged(() => PanoramaBackground);
        //        }
        //    }
        //}
         
    }
}