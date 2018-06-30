using System;

namespace Mpv.NET.Player
{
	internal static class MpvPlayerHelper
	{
		public static string BoolToYesNo(bool yesNoBool)
		{
			return yesNoBool ? "yes" : "no";
		}

		public static bool YesNoToBool(string yesNoString)
		{
			switch (yesNoString)
			{
				case "yes":
					return true;
				case "no":
					return false;
			}

			throw new ArgumentException("Invalid string value for yes/no property.");
		}
	}
}