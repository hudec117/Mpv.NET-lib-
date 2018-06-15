using System;

namespace Mpv.NET.API
{
	public class MpvException : Exception
	{
		public MpvError Error { get; private set; }

		public static MpvException FromError(MpvError error, IMpvFunctions functions)
		{
			var errorString = functions.ErrorString(error);

			var message = $"Error occured: \"{errorString}\".";

			return new MpvException(message, error);
		}

		public MpvException(string message, MpvError error) : base(message)
		{
			Error = error;
		}

		public MpvException(string message) : base(message)
		{
		}
	}
}