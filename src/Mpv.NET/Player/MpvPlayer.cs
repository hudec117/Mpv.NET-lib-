using System;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using Mpv.NET.API;

#if DEBUG
using System.Diagnostics;
#endif

namespace Mpv.NET.Player
{
	/// <summary>
	/// User control containing an mpv player.
	/// </summary>
	public partial class MpvPlayer : IDisposable
	{
		/// <summary>
		/// An instance of the underlying mpv API. Do not touch unless you know what you're doing.
		/// </summary>
		public API.Mpv API => mpv;

		/// <summary>
		/// Absolute or relative (to your executable) path to the libmpv DLL.
		/// </summary>
		public string LibMpvPath { get; private set; }

		/// <summary>
		/// Title of the loaded media. If the title is not available, this falls back to the path.
		/// </summary>
		public string MediaTitle { get; private set; }

		/// <summary>
		/// The desired video quality to retrieve when loading streams from video sites.
		/// </summary>
		public YouTubeDlVideoQuality YouTubeDlVideoQuality
		{
			get => ytdlVideoQuality;
			set
			{
				var formatString = YouTubeDlHelperQuality.GetFormatStringForVideoQuality(value);

				lock (mpvLock)
				{
					mpv.SetPropertyString("ytdl-format", formatString);
				}

				ytdlVideoQuality = value;
			}
		}

		/// <summary>
		/// Number of entries in the playlist.
		/// </summary>
		public int PlaylistEntryCount
		{
			get
			{
				lock (mpvLock)
				{
					return (int)mpv.GetPropertyLong("playlist-count");
				}
			}
		}

		/// <summary>
		/// Index of the current entry in the playlist. (zero based)
		/// </summary>
		public int PlaylistIndex
		{
			get
			{
				lock (mpvLock)
				{
					return (int)mpv.GetPropertyLong("playlist-pos");
				}
			}
		}

		/// <summary>
		/// If true, media will not be unloaded when playback finishes. Warning: The MediaUnloaded event will not be raised!
		/// </summary>
		public KeepOpen KeepOpen
		{
			get
			{
				string stringValue;
				lock (mpvLock)
				{
					stringValue = mpv.GetPropertyString("keep-open");
				}

				return KeepOpenHelper.FromString(stringValue);
			}
			set
			{
				var stringValue = KeepOpenHelper.ToString(value);

				lock (mpvLock)
				{
					mpv.SetPropertyString("keep-open", stringValue);
				}
			}
		}

		/// <summary>
		/// If true, media will loop.
		/// </summary>
		public bool Loop
		{
			get
			{
				string stringValue;
				lock (mpvLock)
				{
					stringValue = mpv.GetPropertyString("loop");
				}

				return LoopHelper.FromString(stringValue);
			}
			set
			{
				var stringValue = LoopHelper.ToString(value);

				lock (mpvLock)
				{
					mpv.SetPropertyString("loop", stringValue);
				}
			}
		}

		/// <summary>
		/// If true, when media is loaded it will automatically play.
		/// </summary>
		public bool AutoPlay { get; set; }

		/// <summary>
		/// True when media is loaded and ready for playback.
		/// </summary>
		public bool IsMediaLoaded { get; private set; }

		/// <summary>
		/// True if media is playing.
		/// </summary>
		public bool IsPlaying { get; private set; }

		/// <summary>
		/// Duration of the media file. (As indicated by metadata)
		/// </summary>
		public TimeSpan Duration
		{
			get
			{
				if (!IsMediaLoaded)
					return TimeSpan.Zero;

				double duration;
				lock (mpvLock)
				{
					duration = mpv.GetPropertyDouble("duration");
				}

				return TimeSpan.FromSeconds(duration);
			}
		}

		/// <summary>
		/// Time since the beginning of the media file.
		/// </summary>
		public TimeSpan Position
		{
			get
			{
				if (!IsMediaLoaded)
					return TimeSpan.Zero;

				double position;
				lock (mpvLock)
				{
					position = mpv.GetPropertyDouble("time-pos");
				}

				return TimeSpan.FromSeconds(position);
			}
			set
			{
				GuardAgainstNotLoaded();

				if (value < TimeSpan.Zero || value > Duration)
					throw new ArgumentOutOfRangeException("Desired position is out of range of the duration or less than zero.");

				var totalMilliseconds = value.TotalMilliseconds;

				var totalMillisecondsString = totalMilliseconds.ToString(CultureInfo.InvariantCulture);

				lock (mpvLock)
				{
					mpv.Command("seek", totalMillisecondsString, "absolute");
				}
			}
		}

		/// <summary>
		/// Time left of playback in the media file.
		/// </summary>
		public TimeSpan Remaining
		{
			get
			{
				if (!IsMediaLoaded)
					return TimeSpan.Zero;

				double remaining;
				lock (mpvLock)
				{
					remaining = mpv.GetPropertyDouble("time-remaining");
				}

				return TimeSpan.FromSeconds(remaining);
			}
		}

		/// <summary>
		/// Volume of the current media. Ranging from 0 to 100 inclusive.
		/// </summary>
		public int Volume
		{
			get
			{
				lock (mpvLock)
				{
					return (int)mpv.GetPropertyDouble("volume");
				}
			}
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentOutOfRangeException("Volume should be between 0 and 100 (inclusive).");

				lock (mpvLock)
				{
					mpv.SetPropertyDouble("volume", value);
				}
			}
		}

		/// <summary>
		/// Playback speed of the current media. Ranging from 0.01 to 100 inclusive.
		/// For example, 0.5 is half the speed, 1 is normal speed and 2 is double the speed.
		/// </summary>
		public double Speed
		{
			get
			{
				lock (mpvLock)
				{
					return mpv.GetPropertyDouble("speed");
				}
			}
			set
			{
				if (value < 0.01 || value > 100)
					throw new ArgumentOutOfRangeException("Speed should be between 0.01 and 100 (inclusive).");

				lock (mpvLock)
				{
					mpv.SetPropertyDouble("speed", value);
				}
			}
		}

		/// <summary>
		/// Invoked when media is loaded.
		/// </summary>
		public event EventHandler MediaLoaded;

		/// <summary>
		/// Invoked when media is unloaded.
		/// </summary>
		public event EventHandler MediaUnloaded;

		/// <summary>
		/// Invoked when an error occurs with the media. (E.g. failed to load)
		/// </summary>
		public event EventHandler MediaError;

		/// <summary>
		/// Invoked when started seeking.
		/// </summary>
		public event EventHandler MediaStartedSeeking;

		/// <summary>
		/// Invoked when seeking has ended and media is ready to be played.
		/// </summary>
		public event EventHandler MediaEndedSeeking;

		/// <summary>
		/// Invoked when the Position ("time-pos" in mpv) property is changed. Event arguments contain the new position.
		/// </summary>
		public event EventHandler<MpvPlayerPositionChangedEventArgs> PositionChanged;

		private API.Mpv mpv;

		private IntPtr hwnd;

		private YouTubeDlVideoQuality ytdlVideoQuality;

		private bool isYouTubeDlEnabled = false;
		private bool isSeeking = false;

		private TaskCompletionSource<object> seekCompletionSource;

		private const int timePosUserData = 10;

		private readonly object mpvLock = new object();

		/// <summary>
		/// Creates an instance of MpvPlayer using a specific libmpv DLL.
		/// </summary>
		/// <param name="hwnd">The windows handle that will host Mpv, such as one created with a WindowsFormsHost in WPF.</param>
		/// <param name="libMpvPath">Relative or absolute path to the libmpv DLL.</param>
		public MpvPlayer(IntPtr hwnd, string libMpvPath)
		{
			this.LibMpvPath = libMpvPath;
			this.hwnd = hwnd;

			Initialise();
		}

		/// <summary>
		/// Creates an instance of MpvPlayer.
		/// </summary>
		/// <param name="hwnd">The windows handle that will host Mpv, such as one created with a WindowsFormsHost in WPF.</param>
		public MpvPlayer(IntPtr hwnd)
		{
			this.hwnd = hwnd;

			Initialise();
		}

		private void Initialise()
		{
			// Initialise the API.
			if (!string.IsNullOrEmpty(LibMpvPath))
				InitialiseMpv(LibMpvPath);
			else if (File.Exists("mpv-1.dll"))
				InitialiseMpv("mpv-1.dll");
			else if (File.Exists("lib\\mpv-1.dll"))
				InitialiseMpv("lib\\mpv-1.dll");
			else
				throw new MpvPlayerException("Failed to find libmpv. Check your path.");

			// Set defaults.
			Volume = 50;
			YouTubeDlVideoQuality = YouTubeDlVideoQuality.Highest;

			// Set the host of the mpv player.
			SetMpvHost(hwnd);
		}

		private void InitialiseMpv(string libMpvPath)
		{
			mpv = new API.Mpv(libMpvPath);

			mpv.PlaybackRestart += MpvOnPlaybackRestart;
			mpv.Seek += MpvOnSeek;

			mpv.FileLoaded += MpvOnFileLoaded;
			mpv.EndFile += MpvOnEndFile;

			mpv.PropertyChange += MpvOnPropertyChange;

			mpv.ObserveProperty("time-pos", MpvFormat.Double, timePosUserData);

#if DEBUG
			mpv.LogMessage += MpvOnLogMessage;

			mpv.RequestLogMessages(MpvLogLevel.Info);
#endif
		}

		private void SetMpvHost(IntPtr hwnd)
		{
			var playerHostPtrLong = hwnd.ToInt64();
			mpv.SetPropertyLong("wid", playerHostPtrLong);
		}

		/// <summary>
		/// Loads the file at the path into mpv. If called while media is playing, the specified media
		/// will be appended to the playlist.
		/// If youtube-dl is enabled, this method can be used to load videos from video sites.
		/// </summary>
		/// <param name="path">Path or URL to media source.</param>
		/// <param name="force">If true, will force load the media replacing any currently playing media.</param>
		public void Load(string path, bool force = false)
		{
			Guard.AgainstNullOrEmptyOrWhiteSpaceString(path, nameof(path));

			lock (mpvLock)
			{
				mpv.SetPropertyString("pause", AutoPlay ? "no" : "yes");

				var loadMethod = LoadMethod.Replace;

				// If there is media already playing, we append
				// the desired video onto the playlist.
				// (Unless force is true.)
				if (IsPlaying && !force)
					loadMethod = LoadMethod.Append;

				var loadMethodString = LoadMethodHelper.ToString(loadMethod);
				mpv.Command("loadfile", path, loadMethodString);
			}
		}

		/// <summary>
		/// Seek to the specified position.
		/// </summary>
		/// <param name="newPosition">The new position.</param>
		/// <returns>Task that will complete when seeking is finished.</returns>
		public Task SeekAsync(TimeSpan newPosition)
		{
			seekCompletionSource = new TaskCompletionSource<object>();

			Position = newPosition;

			return seekCompletionSource.Task;
		}

		/// <summary>
		/// Resume playback.
		/// </summary>
		public void Resume()
		{
			lock (mpvLock)
			{
				mpv.SetPropertyString("pause", "no");
			}

			IsPlaying = true;
		}

		/// <summary>
		/// Pause playback.
		/// </summary>
		public void Pause()
		{
			lock (mpvLock)
			{
				mpv.SetPropertyString("pause", "yes");
			}

			IsPlaying = false;
		}

		/// <summary>
		/// Stop playback and unload the media file.
		/// </summary>
		public void Stop()
		{
			lock (mpvLock)
			{
				mpv.Command("stop");
			}

			IsMediaLoaded = false;
			IsPlaying = false;
		}

		/// <summary>
		/// Goes to the start of the media file and resumes playback.
		/// </summary>
		public void Restart()
		{
			Position = TimeSpan.Zero;

			Resume();
		}

		/// <summary>
		/// Go to the next entry in the playlist.
		/// </summary>
		/// <returns>True if successful, false if not. False indicates that there are no entries after the current entry.</returns>
		public bool PlaylistNext()
		{
			try
			{
				lock (mpvLock)
				{
					mpv.Command("playlist-next");
				}

				return true;
			}
			catch (MpvAPIException exception)
			{
				return HandleCommandMpvException(exception);
			}
		}

		/// <summary>
		/// Go to the previous entry in the playlist.
		/// </summary>
		/// <returns>True if successful, false if not. False indicates that there are no entries before the current entry.</returns>
		public bool PlaylistPrevious()
		{
			try
			{
				lock (mpvLock)
				{
					mpv.Command("playlist-prev");
				}

				return true;
			}
			catch (MpvAPIException exception)
			{
				return HandleCommandMpvException(exception);
			}
		}

		/// <summary>
		/// Remove the current entry from the playlist.
		/// </summary>
		/// <returns>True if removed, false if not.</returns>
		public bool PlaylistRemove()
		{
			try
			{
				lock (mpvLock)
				{
					mpv.Command("playlist-remove", "current");
				}

				return true;
			}
			catch (MpvAPIException exception)
			{
				return HandleCommandMpvException(exception);
			}
		}

		/// <summary>
		/// Removes a specific entry in the playlist, indicated by an index.
		/// </summary>
		/// <param name="index">Zero based index to an entry in the playlist.</param>
		/// <returns>True if removed, false if not.</returns>
		public bool PlaylistRemove(int index)
		{
			var indexString = index.ToString();

			try
			{
				lock (mpvLock)
				{
					mpv.Command("playlist-remove", indexString);
				}

				return true;
			}
			catch (MpvAPIException exception)
			{
				return HandleCommandMpvException(exception);
			}
		}

		/// <summary>
		/// Moves the playlist entry at oldIndex to newIndex. This does not swap the entries.
		/// </summary>
		/// <param name="oldIndex">Index of the entry you want to move.</param>
		/// <param name="newIndex">Index of where you want to move the entry.</param>
		/// <returns>True if moved, false if not.</returns>
		public bool PlaylistMove(int oldIndex, int newIndex)
		{
			var oldIndexString = oldIndex.ToString();
			var newIndexString = newIndex.ToString();

			try
			{
				lock (mpvLock)
				{
					mpv.Command("playlist-move", oldIndexString, newIndexString);
				}

				return true;
			}
			catch (MpvAPIException exception)
			{
				return HandleCommandMpvException(exception);
			}
		}

		/// <summary>
		/// Clear the playlist of all entries,
		/// </summary>
		public void PlaylistClear()
		{
			lock (mpvLock)
			{
				mpv.Command("playlist-clear");
			}
		}

		/// <summary>
		/// Enable youtube-dl functionality in mpv.
		/// </summary>
		/// <param name="ytdlHookScriptPath">Relative or absolute path to the "ytdl_hook.lua" script.</param>
		public void EnableYouTubeDl(string ytdlHookScriptPath)
		{
			if (isYouTubeDlEnabled)
				return;

			Guard.AgainstNullOrEmptyOrWhiteSpaceString(ytdlHookScriptPath, nameof(ytdlHookScriptPath));

			lock (mpvLock)
			{
				mpv.Command("load-script", ytdlHookScriptPath);
			}

			isYouTubeDlEnabled = true;
		}

		private void MpvOnPlaybackRestart(object sender, EventArgs e)
		{
			if (isSeeking)
			{
				seekCompletionSource?.SetResult(null);

				MediaEndedSeeking?.Invoke(this, EventArgs.Empty);
				isSeeking = false;
			}
		}

		private void MpvOnSeek(object sender, EventArgs e)
		{
			isSeeking = true;
			MediaStartedSeeking?.Invoke(this, EventArgs.Empty);
		}

		private void MpvOnFileLoaded(object sender, EventArgs e)
		{
			IsMediaLoaded = true;

			IsPlaying = AutoPlay;

			MediaTitle = mpv.GetPropertyString("media-title");

			MediaLoaded?.Invoke(this, EventArgs.Empty);
		}

		private void MpvOnEndFile(object sender, MpvEndFileEventArgs e)
		{
			IsMediaLoaded = false;
			isSeeking = false;

			var eventEndFile = e.EventEndFile;

			switch (eventEndFile.Reason)
			{
				case MpvEndFileReason.EndOfFile:
				case MpvEndFileReason.Stop:
				case MpvEndFileReason.Quit:
				case MpvEndFileReason.Redirect:
					MediaUnloaded?.Invoke(this, EventArgs.Empty);
					break;
				case MpvEndFileReason.Error:
					MediaError?.Invoke(this, EventArgs.Empty);
					break;
			}
		}

#if DEBUG
		private void MpvOnLogMessage(object sender, MpvLogMessageEventArgs e)
		{
			var message = e.Message;

			var prefix = message.Prefix;
			var text = message.Text;

			Debug.Write($"[{prefix}] {text}");
		}
#endif

		private void MpvOnPropertyChange(object sender, MpvPropertyChangeEventArgs e)
		{
			var eventProperty = e.EventProperty;

			switch (e.ReplyUserData)
			{
				case timePosUserData:
					var newPosition = eventProperty.DataDouble;

					InvokePositionChanged(newPosition);
					break;
			}
		}

		private void InvokePositionChanged(double newPosition)
		{
			var eventArgs = new MpvPlayerPositionChangedEventArgs(newPosition);
			PositionChanged?.Invoke(this, eventArgs);
		}

		private void GuardAgainstNotLoaded()
		{
			if (!IsMediaLoaded)
				throw new InvalidOperationException("Operation could not be completed because no media file has been loaded.");
		}

		private static bool HandleCommandMpvException(MpvAPIException exception)
		{
			if (exception.Error == MpvError.Command)
				return false;
			else
				throw exception;
		}

		public void Dispose()
		{
			mpv.Dispose();
		}
	}
}