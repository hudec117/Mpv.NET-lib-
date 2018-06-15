using System;

namespace Mpv.NET.API
{
	public interface IMpvFunctions
	{
		MpvClientAPIVersion ClientAPIVersion			{ get; }
		MpvErrorString ErrorString						{ get; }
		MpvFree Free									{ get; }
		MpvClientName ClientName						{ get; }
		MpvCreate Create								{ get; }
		MpvInitialise Initialise						{ get; }
		MpvDetachDestroy DetachDestroy					{ get; }
		MpvTerminateDestroy TerminateDestroy			{ get; }
		MpvCreateClient CreateClient					{ get; }
		MpvLoadConfigFile LoadConfigFile				{ get; }
		MpvGetTimeUs GetTimeUs							{ get; }
		MpvSetOption SetOption							{ get; }
		MpvSetOptionString SetOptionString				{ get; }
		MpvCommand Command								{ get; }
		MpvCommandAsync CommandAsync					{ get; }
		MpvSetProperty SetProperty						{ get; }
		MpvSetPropertyString SetPropertyString			{ get; }
		MpvSetPropertyAsync SetPropertyAsync			{ get; }
		MpvGetProperty GetProperty						{ get; }
		MpvGetPropertyString GetPropertyString			{ get; }
		MpvGetPropertyOSDString GetPropertyOSDString	{ get; }
		MpvGetPropertyAsync GetPropertyAsync			{ get; }
		MpvObserveProperty ObserveProperty				{ get; }
		MpvUnobserveProperty UnobserveProperty			{ get; }
		MpvEventName EventName							{ get; }
		MpvRequestEvent RequestEvent					{ get; }
		MpvRequestLogMessages RequestLogMessages		{ get; }
		MpvWaitEvent WaitEvent							{ get; }
		MpvWakeup Wakeup								{ get; }
		MpvSetWakeupCallback SetWakeupCallback			{ get; }
		MpvGetWakeupPipe GetWakeupPipe					{ get; }
		MpvWaitAsyncRequests WaitAsyncRequests			{ get; }

		MpvGetPropertyDouble GetPropertyDouble			{ get; }
		MpvGetPropertyLong GetPropertyLong				{ get; }
	}
}