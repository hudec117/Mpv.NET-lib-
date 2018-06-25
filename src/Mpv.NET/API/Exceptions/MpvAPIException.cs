using System;

namespace Mpv.NET.API
{
	public class MpvAPIException : Exception
	{
		public MpvError Error { get; private set; }

		public static MpvAPIException FromError(MpvError error, IMpvFunctions functions)
		{
			var errorString = functions.ErrorString(error);

			var message = $"Error occured: \"{errorString}\".";

			return new MpvAPIException(message, error);
		}

		public MpvAPIException(string message, MpvError error) : base(message)
		{
			Error = error;
		}

		public MpvAPIException(string message) : base(message)
		{
		}
	}
}