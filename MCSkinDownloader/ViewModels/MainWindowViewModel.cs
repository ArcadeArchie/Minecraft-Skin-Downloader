using Avalonia.Controls;
using Avalonia.Media.Imaging;
using DynamicData;
using MCSkinDownloader.Models;
using MCSkinDownloader.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace MCSkinDownloader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ISearchService _searchService;
        private readonly IImageDownloaderService _imageDownloaderService;

        [Reactive]
        public string SearchText { get; set; }
        [Reactive]
        public IBitmap CurrentImage { get; set; }
        
        private ListBoxItem _currentItem;
        public ListBoxItem CurrentItem 
        {
            get => _currentItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentItem, value);
                UpdateImageCmd.Execute(_currentItem);
            } 
        }
        public ObservableCollection<ListBoxItem> Items { get; set; } = new ObservableCollection<ListBoxItem>() { new ListBoxItem { Content = "No Results" } };

        public ReactiveCommand<string, bool> SearchCmd { get; }
        private ReactiveCommand<ListBoxItem, Unit> UpdateImageCmd { get; }


        public MainWindowViewModel()
        {
            var canSearch = this.WhenAnyValue(vm => vm.SearchText,
                (searchText) =>
                    !string.IsNullOrEmpty(searchText));
            SearchCmd = ReactiveCommand.CreateFromTask<string, bool>(SearchName, canSearch);
            SearchCmd.IsExecuting.ToProperty(this, x => x.IsBusy, out isBusy);

            var canUpdate = this.WhenAnyValue(vm => vm.CurrentItem,
                (currentItem) =>
                    currentItem!=null && currentItem.Content != null);
            UpdateImageCmd = ReactiveCommand.CreateFromTask<ListBoxItem, Unit>(UpdateImage, canUpdate);
            UpdateImageCmd.IsExecuting.ToProperty(this, x => x.IsBusy, out isBusy);
        }

        public MainWindowViewModel(ISearchService searchService, IImageDownloaderService imageDownloaderService) : this()
        {
            _searchService = searchService;
            _imageDownloaderService = imageDownloaderService;
        }

        private async Task<bool> SearchName(string arg)
        {
            string html = await _searchService.GetSkinHTMLAsync(SearchText);
            var newItems = _searchService.GetSearchResults(html);
            if (newItems != null & newItems.Count() > 0)
            {
                Items.Clear();
                Items.AddRange(newItems);
                return true;
            }
            return false;
        }
    
    
        private async Task<Unit> UpdateImage(ListBoxItem item)
        {
            if (item != null && item.Content != null)
            {
                string url = await _imageDownloaderService.GetImageURL(item.Content as SearchResult);
                CurrentImage = await _imageDownloaderService.DownloadImage(url, true, (item.Content as SearchResult).DisplayText);
            }
            return Unit.Default;
        }
    }
}
