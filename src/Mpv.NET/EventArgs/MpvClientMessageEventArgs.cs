using System;

namespace Mpv.NET
{
	public class MpvClientMessageEventArgs : EventArgs
	{
		public MpvEventClientMessage EventClientMessage { get; private set; }

		public MpvClientMessageEventArgs(MpvEventClientMessage eventClientMessage)
		{
			EventClientMessage = eventClientMessage;
		}
	}
}