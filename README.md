## GifViewer
A single page Web application that show Gif animation images one by one.

## Development Enviroment
- Windows 10 + Visual Studio 2019 community
- Or MacOS + Visual Studio 2019 community for Mac (If it support Bower then it will be very will ). You have to seteup [libgdiplus] (https://github.com/mono/libgdiplus) for [CoreCompact](https://github.com/CoreCompat/CoreCompat).
- ASP.Net Core 2.1. For ASP.Net Core 1.X version ([ASP.Net core 1.1.2 for Windows](https://www.microsoft.com/net/download/windows),or [ASP.Net core 1.1.2 for Mac](https://www.microsoft.com/net/download/macos)) check [here](https://github.com/XMTei/GifViewer/tree/495646ab69d3a66a2229fb040491c10146afb0f7)
- [W3.css](https://www.w3schools.com/w3css/default.asp) for responsiveness
- Use [CoreCompact](https://github.com/CoreCompat/CoreCompat) for imaging.
- C#,Javascript

## Setup a Web Server
- Azure App Service windows server is used to test this project.[here](http://gifviewer.azurewebsites.net/) is the web page.
- Linux + ASP.NET core. Tested on Ubuntu Server.

## Use of browsers
- IE,Edge,Chrome,FireFox on Windows.
- Safari,Chrome on MacOS.
- Safari,Chrome,FireFox on iOS.
- Chrome,FireFox on Android.

## Features
- Show GIF animation one by one on Web browser.
- Set interval time to show next GIF image.
- Turn on/off for showing animation. If it set to off, show a frame of the GIF image.
- This program work for multi languages, and currently has English, Chinese, Japanese UI. If you want to display with other languages, the only thing you need to do is translating a set of strings(part of JSON file).
- You can upload your own GIF file to server, and show it to all. 

## Screen shoots
![Screenshot](/GifViewer/wwwroot/images/GifViewer.gif)

![ScreenshotiPhone](/GifViewer/wwwroot/images/GifVieweriPhone.gif)

## Notice
If use this program on MacOS or Linux, make sure you have to install [libgdiplus] (https://github.com/mono/libgdiplus) for [CoreCompact](https://github.com/CoreCompat/CoreCompat).
