using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using MCSkinDownloader.Models;
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
        Task<IBitmap> DownloadImage(string url, bool saveToFile, string fileName = "");
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
        public async Task<IBitmap> DownloadImage(string url, bool saveToFile, string fileName = "")
        {
            using (var result = await _client.GetAsync(url))
            {
                if (result.IsSuccessStatusCode)
                {
                    var pic = new Bitmap(await result.Content.ReadAsStreamAsync());
                    if (saveToFile)
                    {
                        SavePicture(pic, fileName);
                    }
                    return pic;
                }
            }
            return null;
        }

        private void SavePicture(Bitmap pic, string fileName)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filePath = System.IO.Path.Combine(basePath, "Downloads", $"{fileName}.png");
            using (var file = System.IO.File.Create(filePath))
            {
                pic.Save(file);
            }
        }

        private string GetHash(string html)
        {
            string hash = "";
            RegexOptions options = RegexOptions.Multiline;
            foreach (Match m in Regex.Matches(html, Const.Regex.ATTR_VALUE, options))
            {
                if (m.Groups[1].Value.ToLowerInvariant() == "data-skin-hash")
                {
                    hash = m.Groups[2].Value;
                    break;
                }
            }
            return hash;
        }
    }
}
