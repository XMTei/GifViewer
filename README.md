## GifViewer
A single page Web application that show Gif animation images one by one.

## Development Environment
- Windows 10 + Visual Studio 2017 community
- Or MacOS + Visual Studio 2017 community for Mac (If it support Bower then it will be very will )
- ASP.Net Core 2.0. For ASP.Net Core 1.X version ([ASP.Net core 1.1.2 for Windows](https://www.microsoft.com/net/download/windows),or [ASP.Net core 1.1.2 for Mac](https://www.microsoft.com/net/download/macos)) check [here](https://github.com/XMTei/GifViewer/tree/495646ab69d3a66a2229fb040491c10146afb0f7)
- [W3.css](https://www.w3schools.com/w3css/default.asp) for responsiveness
- Use [CoreCompact](https://github.com/CoreCompat/CoreCompat) for imaging.
- C#,Javascript

## Setup a Web Server
- Azure App Service windows server is used to test this project.[here](http://gifviewer.azurewebsites.net/) is the web page.
- I believe Linux + ASP.NET core can be used, but need to test. 

## Use of browsers
- IE,Edge,Chrome,FireFox on Windows.
- Safari,Chrome on MacOS.
- Safari,Chrome,FireFox on iOS.
- Chrome,FireFox on Android.

## Features
- Show GIF animation one by one on Web browser.
- Set interval time to show next GIF image.
- Turn on/off for showing animation. If it set to off, show a frame of the GIF image.
- This program work for multi languages, and currently has English, Chinese, Japanese UI. If you want to display with other langguages, the only thing you need to do is translating a set of strings(part of JSON file).
- You can upload your own GIF file to server, and show it to all. 

## Screen shoots
![Screenshot](/GifViewer/wwwroot/images/GifViewer.gif)

![ScreenshotiPhone](/GifViewer/wwwroot/images/GifVieweriPhone.gif)

## Notice
If use this program on MacOS or Linux, make sure you reference the native packages of CoreCompact:
* [Linux: `runtime.linux-x64.CoreCompat.System.Drawing`](https://www.nuget.org/packages/runtime.linux-x64.CoreCompat.System.Drawing)
* [OS X: `runtime.osx.10.10-x64.CoreCompat.System.Drawing`](https://www.nuget.org/packages/runtime.osx.10.10-x64.CoreCompat.System.Drawing)
