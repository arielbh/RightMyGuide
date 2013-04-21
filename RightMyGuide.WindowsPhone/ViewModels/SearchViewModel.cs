using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using RightMyGuide.DataAccess.ServiceReference;
using ShakeGestures;
using SuiteValue.UI.WP8;
using Windows.Phone.Speech.Synthesis;

namespace RightMyGuide.WindowsPhone.ViewModels
{
    public class SearchViewModel : NavigationViewModelBase
    {
        private ObservableCollection<TVShow> _results;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationMode mode, System.Collections.Generic.IDictionary<string, string> parameter, bool isNavigationInitiator)
        {
            base.OnNavigatedTo(mode, parameter, isNavigationInitiator);
            App.IMdbServiceClient.SearchShowByTitleCompleted += IMdbServiceClient_SearchShowByTitleCompleted;
            // register shake event
            ShakeGesturesHelper.Instance.ShakeGesture += Instance_ShakeGesture;

            // optional, set parameters
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 3;
            ShakeGesturesHelper.Instance.Active = true;
        }

        private void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(Results.Clear);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationMode mode)
        {
            base.OnNavigatedFrom(mode);
            App.IMdbServiceClient.SearchShowByTitleCompleted -= IMdbServiceClient_SearchShowByTitleCompleted;
            ShakeGesturesHelper.Instance.Active = false;
            ShakeGesturesHelper.Instance.ShakeGesture -= Instance_ShakeGesture;


        }

        void IMdbServiceClient_SearchShowByTitleCompleted(object sender, DataAccess.ServiceReference.SearchShowByTitleCompletedEventArgs e)
        {
            if (e.UserState == this)
            {
                IsInAsync = false;
                if (e.Cancelled || e.Error != null) return;
                Results = e.Result;
                CallOutFirstShow(Results.FirstOrDefault());
            }
        }

        private void CallOutFirstShow(TVShow show)
        {
            if (show != null)
            {
                var synth = new SpeechSynthesizer();
                synth.SpeakTextAsync(show.Title);
            }
        }

        public ObservableCollection<TVShow> Results
        {
            get { return _results; }
            set { _results = value;
            OnPropertyChanged(() => Results);}
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
                        Navigate(new ShowViewModel { Show = SelectedShow});
                    }
                    SelectedShow = null;
                }
            }
        }

        public void Search(string text)
        {
            IsInAsync = true;
            AsyncMessage = "Searching...";
            App.IMdbServiceClient.SearchShowByTitleAsync(text, 3, 0,false, this);
            
        }
    }
}