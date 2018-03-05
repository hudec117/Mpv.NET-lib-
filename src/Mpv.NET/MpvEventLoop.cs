using Mpv.NET.Interop;
using System;
using System.Threading;

namespace Mpv.NET
{
	public class MpvEventLoop
	{
		public bool IsRunning { get; private set; }

		public Action<MpvEvent> Callback
		{
			get => callback;
			set
			{
				Guard.AgainstNull(value, nameof(Callback));

				callback = value;
			}
		}

		public IntPtr MpvHandle
		{
			get => mpvHandle;
			private set
			{
				if (value == IntPtr.Zero)
					throw new ArgumentException("Mpv handle is invalid.");

				mpvHandle = value;
			}
		}

		public IMpvFunctions Functions
		{
			get => functions;
			private set
			{
				Guard.AgainstNull(value, nameof(functions));

				functions = value;
			}
		}

		private Action<MpvEvent> callback;
		private IntPtr mpvHandle;
		private IMpvFunctions functions;

		private Thread eventLoopThread;

		private bool isStopping = false;

		private bool disposed = false;

		public MpvEventLoop(Action<MpvEvent> callback, IntPtr mpvHandle, IMpvFunctions functions)
		{
			Callback = callback;
			MpvHandle = mpvHandle;
			Functions = functions;

			eventLoopThread = new Thread(EventLoopThreadHandler);
		}

		public void Start()
		{
			Guard.AgainstDisposed(disposed, nameof(MpvEventLoop));

			eventLoopThread.Start();

			IsRunning = true;
		}

		public void Stop()
		{
			Guard.AgainstDisposed(disposed, nameof(MpvEventLoop));

			isStopping = true;

			// Wake up WaitEvent in the event loop thread
			// so we can stop it.
			Functions.Wakeup(mpvHandle);

			IsRunning = false;
		}

		private void EventLoopThreadHandler(object state)
		{
			while (IsRunning)
			{
				var eventPtr = Functions.WaitEvent(mpvHandle, Timeout.Infinite);
				if (eventPtr == IntPtr.Zero)
					continue;

				var @event = MpvMarshal.PtrToStructure<MpvEvent>(eventPtr);

				if (@event.ID == MpvEventID.None && isStopping)
				{
					isStopping = false;
					continue;
				}

				callback(@event);
			}
		}
	}
}