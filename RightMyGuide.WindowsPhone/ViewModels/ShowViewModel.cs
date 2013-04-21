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
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationMode mode,
                                              System.Collections.Generic.IDictionary<string, string> parameter,
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
                              () => App.IMdbServiceClient.AddReviewAsync(Show.Id, (int)Math.Ceiling(NewReviewRating), UserName, NewReview)));
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
                                                                                 await
                                                                                     App.FavoritesService
                                                                                        .AddShowToFavorite(Show);
                                                                             }));
            }
        }

        private DelegateCommand _pinToStartCommand;

        public DelegateCommand PinToStartCommand
        {
            get
            {
                return _pinToStartCommand ?? (_pinToStartCommand = new DelegateCommand(CreateSecondaryTile));
            }
        }

        private DelegateCommand _lockScreenCommand;

        public DelegateCommand LockScreenCommand
        {
            get
            {
                return _lockScreenCommand ?? (_lockScreenCommand = new DelegateCommand(LockHelper));
            }
        }




        private DelegateCommand _shareCommand;

        public DelegateCommand ShareCommand
        {
            get
            {
                return _shareCommand ?? (_shareCommand = new DelegateCommand(
                                                             () =>
                                                             {
                                                                 var shareLinkTask = new ShareLinkTask
                                                                 {
                                                                     Title = Show.Title,
                                                                     LinkUri = new Uri(Show.ImdbUrl, UriKind.Absolute),
                                                                     Message = "Watch out for this TV Show"
                                                                 };
                                                                 shareLinkTask.Show();
                                                             }));
            }
        }


        private BitmapImage _currentImage;

        private async void LockHelper()
        {
            try
            {
                var isProvider = LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    // If you're not the provider, this call will prompt the user for permission.
                    // Calling RequestAccessAsync from a background agent is not allowed.
                    var op = await LockScreenManager.RequestAccessAsync();

                    // Only do further work if the access was granted.
                    isProvider = op == Windows.Phone.System.UserProfile.LockScreenRequestResult.Granted;
                }

                if (isProvider)
                {
                    if (_currentImage != null) return;
                    BitmapImage bmpImage = new BitmapImage() {CreateOptions = BitmapCreateOptions.None};
                    _currentImage = bmpImage;
                    bmpImage.ImageOpened += delegate
                    {
                        WriteableBitmap bmp = new WriteableBitmap(bmpImage);
                        var stg = IsolatedStorageFile.GetUserStoreForApplication();
                        using (var stm = stg.CreateFile(Show.Id + ".jpg"))
                        {
                            bmp.SaveJpeg(stm, bmp.PixelWidth, bmp.PixelHeight, 0, 80);
                        }

                        var path = "ms-appdata:///Local/" + Show.Id + ".jpg";

                        // At this stage, the app is the active lock screen background provider.

                        // The following code example shows the new URI schema.
                        // ms-appdata points to the root of the local app data folder.
                        // ms-appx points to the Local app install folder, to reference resources bundled in the XAP package.

                        //var schema = isAppResource ? "ms-appx:///" : "ms-appdata:///Local/";
                        var uri = new Uri(path, UriKind.Absolute);

                        // Set the lock screen background image.
                        Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                        _currentImage = null;
                    };
                    bmpImage.UriSource = new Uri(Show.PosterUrl, UriKind.Absolute);

                    // Get the URI of the lock screen background image.
                    //var currentImage = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                    //System.Diagnostics.Debug.WriteLine("The new lock screen background image is set to {0}", currentImage.ToString());
                }
                else
                {
                    MessageBox.Show("You said no, so I can't update your background.");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void CreateSecondaryTile()
        {
            var uri = new Uri(@"/Views/ShowView.xaml?id=" + Show.Id,
                              UriKind.Relative);
            var tile = new StandardTileData
            {
                Title = Show.Title,
                BackContent = Show.Title,
                BackTitle = Show.Title,
                Count = (Show.Episodes != null) ? Show.Episodes.Count : 0,
            };
            var second = ShellTile.ActiveTiles.FirstOrDefault(
                t => t.NavigationUri == uri);
            if (second == null)
            {
                ShellTile.Create(uri, tile);
            }
            else
            {
                second.Update(tile);
            }
        }
    }
}