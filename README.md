# Mpv<span />.NET

[![Version](https://img.shields.io/nuget/v/Mpv.NET.svg?style=flat-square)](https://www.nuget.org/packages/Mpv.NET/)
[![Downloads](https://img.shields.io/nuget/dt/Mpv.NET.svg?style=flat-square)](https://www.nuget.org/packages/Mpv.NET/)

.NET video/media player based on [mpv](https://github.com/mpv-player/mpv) along with a wrapper for the C API.

#### Player Features

* Looping
* Auto play
* Asynchronous seeking
* Simple setup and usage
* Playlist - Load, Next, Previous, Move, Remove, Shuffle or Clear
* Optional youtube-dl support to play videos from hundreds of video sites.
	* Change the desired video quality.

#### Notes:

* No documentation has yet been written for the C API. Consult [client.h](https://github.com/mpv-player/mpv/blob/master/libmpv/client.h).
* The entirety of the mpv C API has not yet been implemented.

If you encounter any bugs or would like to see a feature added then please open an issue. Contributions are very welcome!

## Download

This package is available via [NuGet](https://www.nuget.org/packages/Mpv.NET).

## Prerequisites

To use the wrapper (and user control) you will need libmpv.

1. Download libmpv from https://mpv.srsfckn.biz/ ("dev" version)
2. Extract "mpv-1.dll" from either the "32" or "64" directories into your project.
    (A "lib" folder in your project is common practice)
3. Include the file in your IDE and instruct your build system to copy the DLL to output.
    * In Visual Studio this can be achieved so:
        1. In your Solution Explorer click the "Show All Files" button at the top.
        2. You should see the DLL show up, right click on it and select "Include In Project".
        3. Right click on the DLL and select "Properties", then change the value for "Copy to Output Directory" to "Copy Always".
4. Done!

If you wish to compile libmpv yourself, there is a [guide](https://github.com/mpv-player/mpv/blob/master/DOCS/compile-windows.md) available in the mpv repository.

## Player

To embed an instance of mpv into your application, you will need to pass in a handle (HWND) into the constructor of the `MpvPlayer` class.

This player was designed to work on Windows and tested in WPF and Windows Forms. Not tested on other platforms.

To overlay controls over the top of the player please see this [issue](https://github.com/hudec117/Mpv.WPF/issues/3#issuecomment-396020211).

### Initialisation

`MpvPlayer` provides 2 constructors:
1. `MpvPlayer(IntPtr hwnd)`
2. `MpvPlayer(IntPtr hwnd, string libMpvPath)`

When constructor #1 is used, the player attempts to load libmpv from: (in order)
* LibMpvPath property
* "mpv-1.dll"
* "lib\mpv-1.dll"

### WPF

Since WPF doesn't keep traditional HWND handles to controls we will need use a `System.Windows.Forms.Control` object (E.g. `Panel` or nearly any other WinForms control) to host the mpv instance.

You will need to add a reference to `System.Windows.Forms`:
* In Visual Studio this can be achieved so:
    * Right click "References" in your project and click "Add Reference".
    * Navigate to Assemblies, find `System.Windows.Forms`, make sure it's ticked and click "OK".

See [Mpv.NET.WPFExample](https://github.com/hudec117/Mpv.NET/tree/master/src/Mpv.NET.WPFExample) project for a basic example.

### Windows Forms

See [Mpv.NET.WinFormsExample](https://github.com/hudec117/Mpv.NET/tree/master/src/Mpv.NET.WinFormsExample) project for a basic example.

## API

This "API" is a wrapper around the mpv C API defined in [client.h](https://github.com/mpv-player/mpv/blob/master/libmpv/client.h) and it utilised by the player.

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
var functions = new MpvFunctions(dllPath)

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
```

The code above does not contain any error checking, most of the mpv functions return an MpvError which indicates whether an error has occured.

## Licensing

The libmpv C API *specifically* is licensed under [ICS](https://choosealicense.com/licenses/isc/), this means that a wrapper such as this can be licensed under [MIT](https://choosealicense.com/licenses/mit/).

The rest of libmpv is licensed under [GPLv2](https://choosealicense.com/licenses/gpl-2.0/) by default, which means that any work utilising this wrapper in conjunction with libmpv is subject to GPLv2, unless libmpv is compiled using [LGPL](https://choosealicense.com/licenses/lgpl-2.1/).

See [here](https://github.com/mpv-player/mpv#license) for more information.
