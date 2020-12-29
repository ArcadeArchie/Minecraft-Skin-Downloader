using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MCSkinDownloader.Services;
using MCSkinDownloader.ViewModels;
using MCSkinDownloader.Views;
using System.Net.Http;

namespace MCSkinDownloader
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var client = new HttpClient();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(new SearchService(client), new ImageDownloaderService(client)),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
