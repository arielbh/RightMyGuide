using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using RightMyGuide.DataAccess;
using RightMyGuide.DataAccess.ServiceReference;

namespace RightMyGuide.BackgroundAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {

        public static IMdbServiceClient IMdbServiceClient { get; private set; }
        public static FavoritesService FavoritesService { get; private set; }

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
            IMdbServiceClient = new IMdbServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.None),
                                                      new EndpointAddress(
                                                          "http://demoepisodeservice.cloudapp.net/IMdbService.svc"));
            IMdbServiceClient.GetFutureEpisodesCompleted += IMdbServiceClient_GetFutureEpisodesCompleted;
            IMdbServiceClient.GetShowsByIdsCompleted += IMdbServiceClient_GetShowsByIdsCompleted;
            FavoritesService = new FavoritesService();

        }

        public static Dispatcher Dispatcher;

        private static Task<IEnumerable<Uri>> LoadImagesAsync(IEnumerable<string> shows)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Uri>>();
            int total = shows.Count(), count = 0;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                List<Uri> uris = new List<Uri>(total);
                foreach (var show in shows)
                {
                    var bmp = new BitmapImage {CreateOptions = BitmapCreateOptions.BackgroundCreation};
                    bmp.ImageOpened += (s, e) =>
                    {
                        var wbmp = new WriteableBitmap((BitmapImage) s);
                        var stg = IsolatedStorageFile.GetUserStoreForApplication();
                        using (var stm = stg.CreateFile("/shared/shellcontent/image" + count.ToString() + ".jpg"))
                        {
                            wbmp.SaveJpeg(stm, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);
                        }
                        uris.Add(new Uri("isostore:/shared/shellcontent/image" + count.ToString() + ".jpg"));
                        if (++count == total)
                            tcs.SetResult(uris);
                    };
                    bmp.UriSource = new Uri(show);
                }
            });
            return tcs.Task;

        }

        private static async void IMdbServiceClient_GetShowsByIdsCompleted(object sender, GetShowsByIdsCompletedEventArgs e)
        {
            if (!e.Cancelled || e.Error != null)
            {
                if (e.Result != null)
                {
                    var images = await LoadImagesAsync(e.Result.Take(9).Select(show => show.PosterUrl));
                    var tileData = new CycleTileData()
                    {
                        Title = "Favorites",
                        CycleImages = images
                    };


                    var tile = ShellTile.ActiveTiles.FirstOrDefault();
                    if (tile != null)
                    {
                        tile.Update(tileData);
                    }
                }
            }
            (e.UserState as ScheduledAgent).NotifyComplete();
        }


        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }



        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override async void OnInvoke(ScheduledTask task)
        {
            //await FlipTileWIthEpisodes();
            await CycleTileWithFavorites();

        }

        private async Task CycleTileWithFavorites()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                try
                {
                    var favs = await FavoritesService.GetFavorites();
                    if (favs.Length > 0)
                    {
                        IMdbServiceClient.GetShowsByIdsAsync(new ObservableCollection<string>(favs), false, false, this);
                        return;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            NotifyComplete();

        }

        private async Task FlipTileWIthEpisodes()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                try
                {
                    var favs = await FavoritesService.GetFavorites();
                    if (favs.Length > 0)
                    {
                        var show = favs.FirstOrDefault();


                        IMdbServiceClient.GetFutureEpisodesAsync(show, this);
                        return;
                    }


#if DEBUG_AGENT
              ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif
                }
                catch (Exception)
                {
                }
            }

            NotifyComplete();
        }

        private void UpdateMainTile()
        {

        }


        private static void IMdbServiceClient_GetFutureEpisodesCompleted(object sender,
                                                                         GetFutureEpisodesCompletedEventArgs e)
        {
            if (!e.Cancelled || e.Error != null)
            {
                var item = e.Result.FirstOrDefault();
                if (item != null)
                {
                    var tileData = new FlipTileData
                    {
                        BackContent =
                            string.Format("S{0}E{1} - {2} {3}", item.Season, item.Number, item.Title, item.Date),
                        BackTitle = "Next episode",
                        WideBackContent =
                            string.Format("S{0}E{1} - {2} {3}", item.Season, item.Number, item.Title, item.Date),
                        Count = 3

                        //BackBackgroundImage = new Uri(item.Link, UriKind.Absolute),
                        //WideBackBackgroundImage = new Uri(item.Link, UriKind.Absolute)
                    };


                    var tile = ShellTile.ActiveTiles.FirstOrDefault();
                    if (tile != null)
                    {
                        tile.Update(tileData);
                    }
                }
            }
            (e.UserState as ScheduledAgent).NotifyComplete();
        }
    }
}