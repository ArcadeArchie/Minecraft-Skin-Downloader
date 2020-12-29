using Avalonia.Controls;
using MCSkinDownloader.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCSkinDownloader.Services
{
    public interface ISearchService
    {
        Task<string> GetSkinHTMLAsync(string name);
        IEnumerable<ListBoxItem> GetSearchResults(string html);
    }

    public class SearchService : ISearchService
    {
        private readonly HttpClient _client;


        public SearchService(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36 OPR/73.0.3856.284");
        }


        public async Task<string> GetSkinHTMLAsync(string name)
        {
            string uri = string.Format(Const.API_URL, name);

            var res = await _client.GetAsync(uri);

            return await res.Content.ReadAsStringAsync();
        }

        public IEnumerable<ListBoxItem> GetSearchResults(string html)
        {
            var list = new List<ListBoxItem>();
            RegexOptions options = RegexOptions.Multiline;

            try
            {
                foreach (Match m in Regex.Matches(html, Const.Regex.URL_REGEX_PATTERN, options))
                {
                    if (m.Value.Contains("href=\"/profile/"))
                    {
                        string dspValue = m.Value.Substring(Const.Regex.SUB.Length, m.Value.Length - 2 - Const.Regex.SUB.Length);
                        list.Add(new ListBoxItem()
                        {
                            Content = new SearchResult
                            {
                                DisplayText = dspValue,
                                Value = m.Value
                            }
                        });
                    }
                }
                return list;
            }
            catch (Exception)
            {
                //TODO: Log error
                return null;
            }
        }
    }
}
