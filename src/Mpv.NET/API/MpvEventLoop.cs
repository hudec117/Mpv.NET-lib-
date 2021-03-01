using Mpv.NET.API.Interop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mpv.NET.API
{
	public class MpvEventLoop : IMpvEventLoop, IDisposable
	{
		public bool IsRunning { get => isRunning; private set => isRunning = value; }

		public Action<MpvEvent> Callback { get; set; }

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
			set
			{
				Guard.AgainstNull(value);

				functions = value;
			}
		}

		private IntPtr mpvHandle;
		private IMpvFunctions functions;

		private Task eventLoopTask;

		private bool disposed = false;
		private volatile bool isRunning;

		public MpvEventLoop(Action<MpvEvent> callback, IntPtr mpvHandle, IMpvFunctions functions)
		{
			Callback = callback;
			MpvHandle = mpvHandle;
			Functions = functions;
		}

		public void Start()
		{
			Guard.AgainstDisposed(disposed, nameof(MpvEventLoop));

			DisposeEventLoopTask();

			IsRunning = true;

			eventLoopTask = new Task(EventLoopTaskHandler);
			eventLoopTask.Start();
		}

		public void Stop()
		{
			Guard.AgainstDisposed(disposed, nameof(MpvEventLoop));

			IsRunning = false;

			if (Task.CurrentId == eventLoopTask.Id)
			{
				return;
			}

			// Wake up WaitEvent in the event loop thread
			// so we can stop it.
			Functions.Wakeup(mpvHandle);

			eventLoopTask.Wait();
		}

		private void EventLoopTaskHandler()
		{
			while (IsRunning)
			{
				var eventPtr = Functions.WaitEvent(mpvHandle, Timeout.Infinite);
				if (eventPtr != IntPtr.Zero)
				{
					var @event = MpvMarshal.PtrToStructure<MpvEvent>(eventPtr);
					if (@event.ID != MpvEventID.None)
						Callback?.Invoke(@event);
				}
			}
		}

		private void DisposeEventLoopTask()
		{
			eventLoopTask?.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!disposed)
				{
					Stop();

					DisposeEventLoopTask();
				}

				disposed = true;
			}
		}
	}
}