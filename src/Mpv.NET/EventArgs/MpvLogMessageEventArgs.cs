using System;

namespace Mpv.NET
{
	public class MpvLogMessageEventArgs : EventArgs
	{
		public MpvLogMessage Message { get; private set; }

		public MpvLogMessageEventArgs(MpvLogMessage message)
		{
			Message = message;
		}
	}
}