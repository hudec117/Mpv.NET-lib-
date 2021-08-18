using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mpv.NET.API;
using Mpv.NET.Avalonia;
using Mpv.NET.Player;

namespace Mpv.NET.AvaloniaExample
{
    public partial class MainWindow : Window
    {
        private VideoView VideoView;
        public MainWindow()
        {
            InitializeComponent();
            VideoView = this.Get<VideoView>("VideoView");
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnOpened(object? sender, EventArgs e)
        {
            VideoView.MediaPlayer.AutoPlay = true;
            VideoView.MediaPlayer.Load(@"http://techslides.com/demos/sample-videos/small.mp4");
        }
    }
}