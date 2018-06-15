using System;

namespace Mpv.NET.Player
{
	public class PositionChangedEventArgs : EventArgs
	{
		public int Position { get; private set; }

		public PositionChangedEventArgs(int position)
		{
			Position = position;
		}
	}
}