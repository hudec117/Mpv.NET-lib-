using System;

namespace Mpv.NET.Player
{
	internal static class LoopHelper
	{
		public static string ToString(bool loop)
		{
			return loop ? "yes" : "no";
		}

		public static bool FromString(string loopString)
		{
			if (loopString.Equals("yes", StringComparison.OrdinalIgnoreCase))
				return true;
			else if (loopString.Equals("no", StringComparison.OrdinalIgnoreCase))
				return false;
			else
				throw new ArgumentException("Invalid value for \"loop\" property.");
		}
	}
}