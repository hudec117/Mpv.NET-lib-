using System.Windows;
using Mpv.NET.Player;

namespace Mpv.NET.WPFExample
{
	public partial class MainWindow : Window
	{
		private MpvPlayer player;

		public MainWindow()
		{
			InitializeComponent();

			player = new MpvPlayer(PlayerHost.Handle)
			{
				Loop = true,
				Volume = 50
			};
			player.Load("http://techslides.com/demos/sample-videos/small.mp4");
			player.Resume();
		}

		private void WindowOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			player.Dispose();
		}
	}
}