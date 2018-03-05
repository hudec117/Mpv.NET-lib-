using System;

namespace Mpv.NET
{
	public class MpvEndFileEventArgs : EventArgs
	{
		public MpvEventEndFile EventEndFile { get; private set; }

		public MpvEndFileEventArgs(MpvEventEndFile eventEndFile)
		{
			EventEndFile = eventEndFile;
		}
	}
}