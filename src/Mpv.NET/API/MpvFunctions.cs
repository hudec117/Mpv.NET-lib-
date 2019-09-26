using Mpv.NET.API.Interop;
using System;

namespace Mpv.NET.API
{
	public class MpvFunctions : IMpvFunctions, IDisposable
	{
		public MpvClientAPIVersion ClientAPIVersion			{ get; private set; }
		public MpvErrorString ErrorString					{ get; private set; }
		public MpvFree Free									{ get; private set; }
		public MpvClientName ClientName						{ get; private set; }
		public MpvCreate Create								{ get; private set; }
		public MpvInitialise Initialise						{ get; private set; }
		public MpvDetachDestroy DetachDestroy				{ get; private set; }
		public MpvTerminateDestroy TerminateDestroy			{ get; private set; }
		public MpvCreateClient CreateClient					{ get; private set; }
		public MpvLoadConfigFile LoadConfigFile				{ get; private set; }
		public MpvGetTimeUs GetTimeUs						{ get; private set; }
		public MpvSetOption SetOption						{ get; private set; }
		public MpvSetOptionString SetOptionString			{ get; private set; }
		public MpvCommand Command							{ get; private set; }
		public MpvCommandAsync CommandAsync					{ get; private set; }
		public MpvSetProperty SetProperty					{ get; private set; }
		public MpvSetPropertyString SetPropertyString		{ get; private set; }
		public MpvSetPropertyAsync SetPropertyAsync			{ get; private set; }
		public MpvGetProperty GetProperty					{ get; private set; }
		public MpvGetPropertyString GetPropertyString		{ get; private set; }
		public MpvGetPropertyOSDString GetPropertyOSDString	{ get; private set; }
		public MpvGetPropertyAsync GetPropertyAsync			{ get; private set; }
		public MpvObserveProperty ObserveProperty			{ get; private set; }
		public MpvUnobserveProperty UnobserveProperty		{ get; private set; }
		public MpvEventName EventName						{ get; private set; }
		public MpvRequestEvent RequestEvent					{ get; private set; }
		public MpvRequestLogMessages RequestLogMessages		{ get; private set; }
		public MpvWaitEvent WaitEvent						{ get; private set; }
		public MpvWakeup Wakeup								{ get; private set; }
		public MpvSetWakeupCallback SetWakeupCallback		{ get; private set; }
		public MpvGetWakeupPipe GetWakeupPipe				{ get; private set; }
		public MpvWaitAsyncRequests WaitAsyncRequests		{ get; private set; }

		// Not strictly part of the C API but are used to invoke mpv_get_property with value data type.
		public MpvGetPropertyDouble GetPropertyDouble		{ get; private set; }
		public MpvGetPropertyLong GetPropertyLong			{ get; private set; }

		private IntPtr dllHandle;

		private bool disposed = false;

		public MpvFunctions(string dllPath)
		{
			LoadDll(dllPath);

			LoadFunctions();
		}

		private void LoadDll(string dllPath)
		{
			Guard.AgainstNullOrEmptyOrWhiteSpaceString(dllPath, nameof(dllPath));

			dllHandle = WinFunctions.LoadLibrary(dllPath);
			if (dllHandle == IntPtr.Zero)
				throw new MpvAPIException("Failed to load Mpv DLL. .NET apps by default are 32-bit so make sure you're loading the 32-bit DLL.");
		}

		private void LoadFunctions()
		{
			ClientAPIVersion		= LoadFunction<MpvClientAPIVersion>("mpv_client_api_version");
			ErrorString				= LoadFunction<MpvErrorString>("mpv_error_string");
			Free					= LoadFunction<MpvFree>("mpv_free");
			ClientName				= LoadFunction<MpvClientName>("mpv_client_name");
			Create					= LoadFunction<MpvCreate>("mpv_create");
			Initialise				= LoadFunction<MpvInitialise>("mpv_initialize");
			DetachDestroy			= LoadFunction<MpvDetachDestroy>("mpv_detach_destroy");
			TerminateDestroy		= LoadFunction<MpvTerminateDestroy>("mpv_terminate_destroy");
			CreateClient			= LoadFunction<MpvCreateClient>("mpv_create_client");
			LoadConfigFile			= LoadFunction<MpvLoadConfigFile>("mpv_load_config_file");
			GetTimeUs				= LoadFunction<MpvGetTimeUs>("mpv_get_time_us");
			SetOption				= LoadFunction<MpvSetOption>("mpv_set_option");
			SetOptionString			= LoadFunction<MpvSetOptionString>("mpv_set_option_string");
			Command					= LoadFunction<MpvCommand>("mpv_command");
			CommandAsync			= LoadFunction<MpvCommandAsync>("mpv_command_async");
			SetProperty				= LoadFunction<MpvSetProperty>("mpv_set_property");
			SetPropertyString		= LoadFunction<MpvSetPropertyString>("mpv_set_property_string");
			SetPropertyAsync		= LoadFunction<MpvSetPropertyAsync>("mpv_set_property_async");
			GetProperty				= LoadFunction<MpvGetProperty>("mpv_get_property");
			GetPropertyString		= LoadFunction<MpvGetPropertyString>("mpv_get_property_string");
			GetPropertyOSDString	= LoadFunction<MpvGetPropertyOSDString>("mpv_get_property_osd_string");
			GetPropertyAsync		= LoadFunction<MpvGetPropertyAsync>("mpv_get_property_async");
			ObserveProperty			= LoadFunction<MpvObserveProperty>("mpv_observe_property");
			UnobserveProperty		= LoadFunction<MpvUnobserveProperty>("mpv_unobserve_property");
			EventName				= LoadFunction<MpvEventName>("mpv_event_name");
			RequestEvent			= LoadFunction<MpvRequestEvent>("mpv_request_event");
			RequestLogMessages		= LoadFunction<MpvRequestLogMessages>("mpv_request_log_messages");
			WaitEvent				= LoadFunction<MpvWaitEvent>("mpv_wait_event");
			Wakeup					= LoadFunction<MpvWakeup>("mpv_wakeup");
			SetWakeupCallback		= LoadFunction<MpvSetWakeupCallback>("mpv_set_wakeup_callback");
			GetWakeupPipe			= LoadFunction<MpvGetWakeupPipe>("mpv_get_wakeup_pipe");
			WaitAsyncRequests		= LoadFunction<MpvWaitAsyncRequests>("mpv_wait_async_requests");

			GetPropertyDouble		= LoadFunction<MpvGetPropertyDouble>("mpv_get_property");
			GetPropertyLong			= LoadFunction<MpvGetPropertyLong>("mpv_get_property");
		}

		private TDelegate LoadFunction<TDelegate>(string name) where TDelegate : class
		{
			var delegateValue = MpvMarshal.LoadUnmanagedFunction<TDelegate>(dllHandle, name);
			if (delegateValue == null)
				throw new MpvAPIException($"Failed to load Mpv \"{name}\" function.");

			return delegateValue;
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
					WinFunctions.FreeLibrary(dllHandle);
				}

				disposed = true;
			}
		}
	}
}