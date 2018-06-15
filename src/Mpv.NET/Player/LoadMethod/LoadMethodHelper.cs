using System;

namespace Mpv.NET.Player
{
	internal static class LoadMethodHelper
	{
		public static string ToString(LoadMethod loadMethod)
		{
			switch (loadMethod)
			{
				case LoadMethod.Replace:
					return "replace";
				case LoadMethod.Append:
					return "append";
				case LoadMethod.AppendPlay:
					return "append-play";
			}

			throw new ArgumentException("Invalid enumeration value.");
		}
	}
}