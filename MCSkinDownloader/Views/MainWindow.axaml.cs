using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MCSkinDownloader.ViewModels;

namespace MCSkinDownloader.Views
{
    public class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Instance = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void SearchBox_HandleEnter(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                (this.DataContext as MainWindowViewModel).SearchCmd.Execute();
            }
        }
    }
}
