using System;

namespace Mpv.NET.Tester
{
	public class Program
	{
		private static Mpv mpv;

		private static void Main(string[] args)
		{
			using (mpv = new Mpv("lib\\mpv-1.dll"))
			{
				mpv.Command("loadfile", @"");

				mpv.FileLoaded += MpvOnFileLoaded;

				Console.ReadLine();
			}
		}

		private static void MpvOnFileLoaded(object sender, EventArgs e)
		{
			var title = mpv.GetPropertyString("media-title");

			Console.WriteLine("Playing: " + title);
		}
	}
}