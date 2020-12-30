using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using MCSkinDownloader.Models;
using MCSkinDownloader.Views;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCSkinDownloader.Services
{
    public interface IImageDownloaderService
    {
        Task<string> GetImageURL(SearchResult res);
        Task<IBitmap> GetImage(string url);
        Task DownloadImage(string url, string folderPath = "", string fileName = "");
    }

    public class ImageDownloaderService : IImageDownloaderService
    {
        private readonly HttpClient _client;

        public ImageDownloaderService(HttpClient client = null)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36 OPR/73.0.3856.284");
        }


        public async Task<string> GetImageURL(SearchResult res)
        {

            RegexOptions options = RegexOptions.Multiline;
            Match m = new Regex(Const.Regex.ATTR_VALUE, options).Match(res.Value.ToString());
            string uri = Const.BASE_URL + m.Groups[2].Value;

            var result = await _client.GetAsync(uri);

            return string.Format(Const.IMAGE_URL ,GetHash(await result.Content.ReadAsStringAsync()));
        }
        
        /// <summary>
        /// Downloads an image from the given URL and returns it as an Bitmap
        /// </summary>
        /// <param name="url">The image URL</param>
        /// <returns>The image Bitmap</returns>
        public async Task<IBitmap> GetImage(string url)
        {
            using (var result = await _client.GetAsync(url))
            {
                if (result.IsSuccessStatusCode)
                {
                    var pic = new Bitmap(await result.Content.ReadAsStreamAsync());
                    return pic;
                }
            }
            return null;
        }

        /// <summary>
        /// Downloads an image from the given URL and saves it the specified location
        /// </summary>
        /// <param name="url">The image URL</param>
        /// <param name="folderPath">Save Folder</param>
        /// <param name="fileName">Save Filename</param>
        public async Task DownloadImage(string url, string folderPath = "", string fileName = "")
        {
            var pic = await GetImage(url);
            if (pic != null)
            {
                SavePicture(pic, folderPath, fileName);
            }
        }

        /// <summary>
        /// Saves an Bitmap image to disk
        /// </summary>
        /// <param name="pic">The Image</param>
        /// <param name="folderPath">Save Location</param>
        /// <param name="fileName">Save Filename</param>
        private async void SavePicture(IBitmap pic, string folderPath, string fileName)
        {
            //if null default the users Download folder
            string basePath = string.IsNullOrEmpty(folderPath) ? System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") : folderPath;
            string filePath = System.IO.Path.Combine(basePath, $"{fileName}.png");
            //save the image
            try
            {
                using (var file = System.IO.File.Create(filePath))
                {
                    pic.Save(file);
                }
            }
            catch (Exception e)
            {
                //TODO: Log error
                await MessageBox.Show(MainWindow.Instance,e.Message, "ERROR", MessageBox.MessageBoxButtons.Ok);
            }
        }

        /// <summary>
        /// Searches for the NameMC Skinhash and returns it
        /// </summary>
        /// <param name="html">The HTML text from NameMC</param>
        /// <returns>the NameMC Skinhash</returns>
        private string GetHash(string html)
        {
            string hash = "";
            //set the regex options and loop through the matches
            RegexOptions options = RegexOptions.Multiline;
            foreach (Match m in Regex.Matches(html, Const.Regex.ATTR_VALUE, options))
            {
                if (m.Groups[1].Value.ToLowerInvariant() == "data-skin-hash")// if the 1st group is the right attribute discriptor
                {
                    //take the value of the 2nd group and break the loop
                    hash = m.Groups[2].Value;
                    break;
                }
            }
            return hash;
        }
    }
}
