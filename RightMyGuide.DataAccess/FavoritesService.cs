using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RightMyGuide.DataAccess.ServiceReference;
using Windows.Storage;

namespace RightMyGuide.DataAccess
{
    public class FavoritesService
    {
        public async Task AddShowToFavorite(TVShow show)
        {
            var local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync("favs.txt", CreationCollisionOption.OpenIfExists);


            using (var s = await file.OpenStreamForWriteAsync())
            {
                // Read the data.
                using (StreamReader streamReader = new StreamReader(s))
                {
                    //string current = streamReader.ReadToEnd();
                    var current = show.Id + ",";
                    byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(current.ToCharArray());
                    s.Seek(s.Length, SeekOrigin.Begin);
                    s.Write(fileBytes, 0, fileBytes.Length);
                }
            }
        }

        public Task RemoveShowToFavorite(TVShow show)
        {
            return null;

        }

        public async Task<string[]> GetFavorites()
        {
            var local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync("favs.txt", CreationCollisionOption.OpenIfExists);


            using (var s = await file.OpenStreamForReadAsync())
            {
                // Read the data.
                using (var streamReader = new StreamReader(s))
                {
                    string current = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(current)) return null;
                    var list = current.Split(',').ToList();
                    return list.Where(l => !string.IsNullOrEmpty(l)).ToArray();
                }
            }
        }
    }
}
