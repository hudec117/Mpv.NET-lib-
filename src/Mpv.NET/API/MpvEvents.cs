using System;

namespace Mpv.NET.API
{
	public partial class Mpv
	{
		public event EventHandler Shutdown;
		public event EventHandler<MpvLogMessageEventArgs> LogMessage;
		public event EventHandler<MpvGetPropertyReplyEventArgs> GetPropertyReply;
		public event EventHandler<MpvSetPropertyReplyEventArgs> SetPropertyReply;
		public event EventHandler<MpvCommandReplyEventArgs> CommandReply;
		public event EventHandler StartFile;
		public event EventHandler<MpvEndFileEventArgs> EndFile;
		public event EventHandler FileLoaded;

		[Obsolete("Deprecated in favour of ObserveProperty on \"track-list\".")]
		public event EventHandler TracksChanged;

		[Obsolete("Deprecated in favour of ObserveProperty on \"vid\", \"aid\" and \"sid\".")]
		public event EventHandler TrackSwitched;

		public event EventHandler Idle;

		[Obsolete("Deprecated in favour of ObserveProperty on \"pause\".")]
		public event EventHandler Pause;

		[Obsolete("Deprecated in favour of ObserveProperty on \"pause\".")]
		public event EventHandler Unpause;

		public event EventHandler Tick;

		[Obsolete("This event does not occur anymore, included for compatibility.")]
		public event EventHandler ScriptInputDispatch;

		public event EventHandler<MpvClientMessageEventArgs> ClientMessage;
		public event EventHandler VideoReconfig;
		public event EventHandler AudioReconfig;

		[Obsolete("Deprecated in favour of ObserveProperty on \"metadata\".")]
		public event EventHandler MetadataUpdate;

		public event EventHandler Seek;
		public event EventHandler PlaybackRestart;
		public event EventHandler<MpvPropertyChangeEventArgs> PropertyChange;

		[Obsolete("Deprecated in favour of ObserveProperty on \"chapter\".")]
		public event EventHandler ChapterChange;

		public event EventHandler QueueOverflow;

		private void EventCallback(MpvEvent @event)
		{
			var eventId = @event.ID;
			switch (eventId)
			{
				// Events that can be "handled"
				case MpvEventID.Shutdown:
					HandleShutdown();
					break;
				case MpvEventID.LogMessage:
					HandleLogMessage(@event);
					break;
				case MpvEventID.GetPropertyReply:
					HandleGetPropertyReply(@event);
					break;
				case MpvEventID.SetPropertyReply:
					HandleSetPropertyReply(@event);
					break;
				case MpvEventID.CommandReply:
					HandleCommandReply(@event);
					break;
				case MpvEventID.EndFile:
					HandleEndFile(@event);
					break;
				case MpvEventID.ClientMessage:
					HandleClientMessage(@event);
					break;
				case MpvEventID.PropertyChange:
					HandlePropertyChange(@event);
					break;

				// Todo: Find a better/shorter way of doing this?
				// I tried to put the EventHandlers below into a dictionary
				// and invoke them based on the event ID but a reference
				// to the EventHandler didn't seem to update when a handler
				// was attached to that property and therefore we couldn't invoke
				// it.

				// All other simple notification events.
				case MpvEventID.StartFile:
					InvokeSimple(StartFile);
					break;
				case MpvEventID.FileLoaded:
					InvokeSimple(FileLoaded);
					break;
				case MpvEventID.TracksChanged:
					InvokeSimple(TracksChanged);
					break;
				case MpvEventID.TrackSwitched:
					InvokeSimple(TrackSwitched);
					break;
				case MpvEventID.Idle:
					InvokeSimple(Idle);
					break;
				case MpvEventID.Pause:
					InvokeSimple(Pause);
					break;
				case MpvEventID.Unpause:
					InvokeSimple(Unpause);
					break;
				case MpvEventID.Tick:
					InvokeSimple(Tick);
					break;
				case MpvEventID.ScriptInputDispatch:
					InvokeSimple(ScriptInputDispatch);
					break;
				case MpvEventID.VideoReconfig:
					InvokeSimple(VideoReconfig);
					break;
				case MpvEventID.AudioReconfig:
					InvokeSimple(AudioReconfig);
					break;
				case MpvEventID.MetadataUpdate:
					InvokeSimple(MetadataUpdate);
					break;
				case MpvEventID.Seek:
					InvokeSimple(Seek);
					break;
				case MpvEventID.PlaybackRestart:
					InvokeSimple(PlaybackRestart);
					break;
				case MpvEventID.ChapterChange:
					InvokeSimple(ChapterChange);
					break;
				case MpvEventID.QueueOverflow:
					InvokeSimple(QueueOverflow);
					break;
			}
		}

		private void HandleShutdown()
		{
			eventLoop.Stop();
			Shutdown?.Invoke(this, EventArgs.Empty);
		}

		private void HandleLogMessage(MpvEvent @event)
		{
			if (LogMessage == null)
				return;

			var logMessage = @event.MarshalDataToStruct<MpvLogMessage>();
			if (logMessage.HasValue)
			{
				var eventArgs = new MpvLogMessageEventArgs(logMessage.Value);
				LogMessage.Invoke(this, eventArgs);
			}
		}

		private void HandleGetPropertyReply(MpvEvent @event)
		{
			if (GetPropertyReply == null)
				return;

			var eventProperty = @event.MarshalDataToStruct<MpvEventProperty>();
			if (eventProperty.HasValue)
			{
				var replyUserData = @event.ReplyUserData;
				var error = @event.Error;

				var eventArgs = new MpvGetPropertyReplyEventArgs(replyUserData, error, eventProperty.Value);
				GetPropertyReply.Invoke(this, eventArgs);
			}
		}

		private void HandleSetPropertyReply(MpvEvent @event)
		{
			if (SetPropertyReply == null)
				return;

			var replyUserData = @event.ReplyUserData;
			var error = @event.Error;

			var eventArgs = new MpvSetPropertyReplyEventArgs(replyUserData, error);
			SetPropertyReply.Invoke(this, eventArgs);
		}

		private void HandleCommandReply(MpvEvent @event)
		{
			if (CommandReply == null)
				return;

			var replyUserData = @event.ReplyUserData;
			var error = @event.Error;

			var eventArgs = new MpvCommandReplyEventArgs(replyUserData, error);
			CommandReply.Invoke(this, eventArgs);
		}

		private void HandleEndFile(MpvEvent @event)
		{
			if (EndFile == null)
				return;

			var eventEndFile = @event.MarshalDataToStruct<MpvEventEndFile>();
			if (eventEndFile.HasValue)
			{
				var eventArgs = new MpvEndFileEventArgs(eventEndFile.Value);
				EndFile.Invoke(this, eventArgs);
			}
		}

		private void HandleClientMessage(MpvEvent @event)
		{
			if (ClientMessage == null)
				return;

			var eventClientMessage = @event.MarshalDataToStruct<MpvEventClientMessage>();
			if (eventClientMessage.HasValue)
			{
				var eventArgs = new MpvClientMessageEventArgs(eventClientMessage.Value);
				ClientMessage.Invoke(this, eventArgs);
			}
		}

		private void HandlePropertyChange(MpvEvent @event)
		{
			if (PropertyChange == null)
				return;

			var eventProperty = @event.MarshalDataToStruct<MpvEventProperty>();
			if (eventProperty.HasValue)
			{
				var replyUserData = @event.ReplyUserData;

				var eventArgs = new MpvPropertyChangeEventArgs(replyUserData, eventProperty.Value);
				PropertyChange.Invoke(this, eventArgs);
			}
		}

		private void InvokeSimple(EventHandler eventHandler)
		{
			eventHandler?.Invoke(this, EventArgs.Empty);
		}
	}
}