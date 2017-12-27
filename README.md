## GifViewer
A single page Web application that show Gif animation images one by one.

## Development Environment
- Windows 10 + Visual Studio 2017 community
- Or MacOS + Visual Studio 2017 community for Mac (If it suport Bower then it will be verry will )
- [ASP.Net core 1.1.2 for Windows](https://www.microsoft.com/net/download/windows),or [ASP.Net core 1.1.2 for Mac](https://www.microsoft.com/net/download/macos)
- [W3.css](https://www.w3schools.com/w3css/default.asp) for responsiveness
- Use [CoreCompact](https://github.com/CoreCompat/CoreCompat) for imaging.
- C#,Javascript

## Setup a Web Server
- Currentle I use Azure App Service windows server to test this project.
- I believe Linux + ASP.NET core can be used, but need make a testing. 

## Use of browsers
- IE,Edge,Chrome,FireFox on Windows.
- Safari,Chrome on MacOS.
- Safari,Chrome,FireFox on iOS.
- Chrome,FireFox on Android.

## Features
- Show GIF animation one by one on Web browser.
- Set interval time.
- Turn on/off showing animation. If it set to off, show a frame of the GIF image..
- This program work for multi languages, and currently has English, Chinese, Japanese UI. If you want to display other langguage, the only you need to do is translating a set of strings(a JSON file)
- You can upload your own GIF file to server, and show it to all 

## Screen shoots
![Screenshot](/GifViewer/wwwroot/images/GifViewer.gif)
![ScreenshotiPhone](/GifViewer/wwwroot/images/GifVieweriPhone.gif)

## Notice
If use this program on MacOS or Linux, make sure you reference the native packages of CoreCompact:
* [Linux: `runtime.linux-x64.CoreCompat.System.Drawing`](https://www.nuget.org/packages/runtime.linux-x64.CoreCompat.System.Drawing)
* [OS X: `runtime.osx.10.10-x64.CoreCompat.System.Drawing`](https://www.nuget.org/packages/runtime.osx.10.10-x64.CoreCompat.System.Drawing)
