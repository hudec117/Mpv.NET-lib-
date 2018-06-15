using System.Windows.Forms;
using Mpv.NET.Player;

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
			player.Load("http://techslides.com/demos/sample-videos/small.mp4");
			player.Resume();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			player.Dispose();
		}
	}
}