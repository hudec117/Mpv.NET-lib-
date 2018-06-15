using System;

namespace Mpv.NET.Player
{
	internal static class KeepOpenHelper
	{
		public static string ToString(KeepOpen keepOpen)
		{
			switch (keepOpen)
			{
				case KeepOpen.Yes:
					return "yes";
				case KeepOpen.No:
					return "no";
				case KeepOpen.Always:
					return "always";
			}

			throw new ArgumentException("Invalid enumeration value.");
		}

		public static KeepOpen FromString(string keepOpenString)
		{
			switch (keepOpenString)
			{
				case "yes":
					return KeepOpen.Yes;
				case "no":
					return KeepOpen.No;
				case "always":
					return KeepOpen.Always;
			}

			throw new ArgumentException("Invalid value for \"keep-open\" property.");
		}
	}
}