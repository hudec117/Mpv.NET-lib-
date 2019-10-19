namespace Mpv.NET.Player
{
	internal static class MpvPlayerHelper
	{
		public static string BoolToYesNo(bool yesNoBool) => yesNoBool ? "yes" : "no";

		public static bool YesNoToBool(string yesNoString) => yesNoString == "yes";
	}
}