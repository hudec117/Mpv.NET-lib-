using Mpv.NET.Player;
using System.Windows.Forms;

namespace Mpv.NET.WinFormsExample
{
	public partial class MainForm : Form
	{
		private MpvPlayer player;

		public MainForm()
		{
			InitializeComponent();

			player = new MpvPlayer(this.Handle)
			{
				Loop = true,
				Volume = 50
			};
			player.Load(@"http://techslides.com/demos/sample-videos/small.mp4");
			player.Resume();
		}

		private void MainFormOnFormClosed(object sender, FormClosedEventArgs e)
		{
			player.Dispose();
		}
	}
}