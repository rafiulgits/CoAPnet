<p align="center">
<img src="https://github.com/chkr1011/CoAPnet/blob/master/Images/icon_det_256.png?raw=true" width="196">
<br/>
<br/>
</p>

[![NuGet Badge](https://buildstats.info/nuget/CoAPnet)](https://www.nuget.org/packages/CoAPnet)
[![Build status](https://ci.appveyor.com/api/projects/status/j252icg2pgkfb7s0/branch/master?svg=true)](https://ci.appveyor.com/project/chkr1011/coapnet)
[![BCH compliance](https://bettercodehub.com/edge/badge/chkr1011/CoAPnet?branch=master)](https://bettercodehub.com/)
[![Join the chat at https://gitter.im/CoAPnet/community](https://badges.gitter.im/CoAPnet/community.svg)](https://gitter.im/CoAPnet/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://raw.githubusercontent.com/chkr1011/CoAPnet/master/LICENSE)

# CoAPnet

CoAPnet is a high performance .NET library for CoAPnet based communication. It provides a CoAPnet client and a CoAPnet server. The library runs on macOS, Linux and Windows. It also supports UDP, UDP+DTLS, TCP and TCP+TLS connection.

## Features

### General

* Async support
* TLS 1.2 support for client and server (but not UWP servers)
* DTLS (up to 1.2) support
* Extensible communication channels (e.g. In-Memory, TCP, TCP+TLS, UDP, UDP+TLS)
* Lightweight (only the low level implementation of CoAPnet, no overhead)
* Performance optimized (processing x / second)*
* Interfaces included for mocking and testing
* Access to internal trace messages
* Unit tested (~x tests)
* No external dependencies

\* Tested on local machine (Intel i7 8700K) with CoAP client and server running in the same process using the TCP channel. The app for verification is part of this repository and stored in _XX_.

### Client

* Communication via TCP (+TLS) or UDP (+DTLS) supported
* Included core _LowLevelCoAPClient_ with low level functionality

### Server

xxx

## CoAPnet Server

_CoAPnet Server_ is a standalone cross platform CoAP server basing on this library. It has the following features.
* Running portable (no installation required)
* Runs und Windows, Linux, macOS, Raspberry Pi
* Python scripting support for manipulating messages, validation of clients, building business logic etc.
* Supports UDP and TCP (with and without TLS/DTLS) connections
* Provides a HTTP based API (including Swagger endpoint)
* Extensive configuration parameters and customization supported

## Supported frameworks

* .NET Standard 1.3+
* .NET Core 1.1+
* .NET Core App 1.1+
* .NET Framework 4.5.2+ (x86, x64, AnyCPU)
* Mono 5.2+
* Universal Windows Platform (UWP) 10.0.10240+ (x86, x64, ARM, AnyCPU, Windows 10 IoT Core)
* Xamarin.Android 7.5+
* Xamarin.iOS 10.14+

## Nuget

This library is available as a nuget package: <https://www.nuget.org/packages/CoAPnet/>

## Examples

Please find examples and the documentation at the Wiki of this repository (<https://github.com/chkr1011/CoAPnet/wiki>).

## Contributions

If you want to contribute to this project just create a pull request. But only pull requests which are matching the code style of this library will be accepted. Before creating a pull request please have a look at the library to get an overview of the required style.
Also additions and updates in the Wiki are welcome.

## References

This library is used in the following projects:

* This library can be used to communicate with the IKEA Tradfri gateway.
* Wirehome.Core (Open Source Home Automation system for .NET Core, <https://github.com/chkr1011/Wirehome.Core>)

If you use this library and want to see your project here please create a pull request.

## License

MIT License

CoAPnet Copyright (c) 2016-2020 Christian Kratky

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
