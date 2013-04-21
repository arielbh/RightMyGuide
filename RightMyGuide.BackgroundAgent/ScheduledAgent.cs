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
            FavoritesService = new FavoritesService();
        }

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
            await FlipTileWIthEpisodes();
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