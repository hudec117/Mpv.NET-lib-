using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mpv.NET.Avalonia;

namespace Mpv.NET.AvaloniaExample
{
    public partial class MainWindow : Window
    {
        private VideoView videoView;

        public MainWindow()
        {
            InitializeComponent();
            videoView = this.Get<VideoView>("VideoView");
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
            videoView.MediaPlayer.AutoPlay = true;
            videoView.MediaPlayer.Load(@"http://techslides.com/demos/sample-videos/small.mp4");
        }
    }
}