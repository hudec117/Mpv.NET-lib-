# Mpv<span />.NET

[![Version](https://img.shields.io/nuget/v/Mpv.NET.svg?style=flat-square)](https://www.nuget.org/packages/Mpv.NET/)
[![Downloads](https://img.shields.io/nuget/dt/Mpv.NET.svg?style=flat-square)](https://www.nuget.org/packages/Mpv.NET/)

A .NET wrapper for the [mpv](https://github.com/mpv-player/mpv) C API.

#### Notes:

* This software is not yet ready to be used in a production environment.
* No documentation has yet been written. Consult [client.h](https://github.com/mpv-player/mpv/blob/master/libmpv/client.h).
* The entirety of the C API has not yet been implemented.

If you encounter any bugs or would like to see a feature added then please open an issue. Contributions are very welcome!

## Download

This package is available via [NuGet](https://www.nuget.org/packages/Mpv.NET).

## Usage

### Prerequisites

To use the wrapper (or user control) you will need libmpv.

1. Download libmpv from https://mpv.srsfckn.biz/ ("dev" version)
2. Extract "mpv-1.dll" from either the "32" or "64" directories into your project.
    (A "lib" folder in your project is common practice)
3. Include the file in your IDE and instruct your build system to copy the DLL to output.
    * In Visual Studio this can be achieved so:
        1. In your Solution Explorer click the "Show All Files" button at the top. Make sure that you have your project selected.
        2. You should see the DLL show up, right click on it and select "Include In Project".
        3. Right click on the DLL and select "Properties", then change the value for "Copy to Output Directory" to "Copy Always".
4. Done!

If you wish to compile libmpv yourself, there is a [guide](https://github.com/mpv-player/mpv/blob/master/DOCS/compile-windows.md) available in the mpv repository.

### Mpv

Simplest way to create an instance of the mpv .NET API and load libmpv:

```csharp
// Relative path to the DLL.
var dllPath = @"lib\mpv-1.dll";

using (var mpv = new Mpv(dllPath))
{
	// code
}
```

This will provide a friendly interface to the mpv C API and start an event loop which will raise events accordingly.

### MpvFunctions

If you are looking for a more direct approach to the C API you can create an instance of MpvFunctions which allows you to execute the loaded delegates directly:

```csharp
var dllPath = @"lib\mpv-1.dll";

using (var mpvFunctions = new MpvFunctions(dllPath))
{
	// code
}
```

Be aware, this method does not raise events and an event loop would have to  be manually implemented.

### MpvEventLoop

If you are looking that start an event loop you will need to create an instance of MpvFunctions, create and initialise mpv, create an instance of MpvEventLoop and start it:

```csharp
var dllPath = @"lib\mpv-1.dll";

// Create an instance of MpvFunctions.
using (var functions = new MpvFunctions(dllPath))
{
	// Create mpv
	var handle = functions.Create();

	// Initialise mpv
	functions.Initialise(handle);
	
	// Create an instance of MpvEventLoop, passing in a callback argument
	// which will be invoked when an event comes in.
	using (var eventLoop = new MpvEventLoop(callback, handle, functions))
	{
		// Start the event loop.
		eventLoop.Start();
	}
}
```

The code above does not contain any error checking, most of the mpv functions return an MpvError which indicates whether an error has occured.

## Related Projects

* [Mpv.WPF](https://github.com/hudec117/Mpv.WPF) - User control library containing the mpv player, powered by this wrapper and mpv.
* Mpv.WinForms - Upcoming user control library for Windows Forms.

## Licensing

The libmpv C API *specifically* is licensed under [ICS](https://choosealicense.com/licenses/isc/), this means that a wrapper such as this can be licensed under [MIT](https://choosealicense.com/licenses/mit/).

The rest of libmpv is licensed under [GPLv2](https://choosealicense.com/licenses/gpl-2.0/) by default, which means that any work utilising this wrapper in conjunction with libmpv is subject to GPLv2, unless libmpv is compiled using [LGPL](https://choosealicense.com/licenses/lgpl-2.1/).

See [here](https://github.com/mpv-player/mpv#license) for more information.
