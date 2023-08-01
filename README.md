# JpCalendar
[![NuGet](https://img.shields.io/nuget/v/JpCalendar)](https://www.nuget.org/packages/JpCalendar)
[![GitHub Actions](https://github.com/zwei222/JpCalendar/actions/workflows/build.yml/badge.svg)](https://github.com/zwei222/JpCalendar/actions/workflows/build.yml)

This library is for processing related to the Japanese calendar.

Processing related to Japanese national holidays and the era name based on the contents listed in the following URLs and the CSV files that can be downloaded.

- [国民の祝日について - 内閣府](https://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html)

## Features
The features of this library are described below.

- A function to obtain the era number (Japanese calendar) based on a given date.
- A function to obtain the "Rokuyo" based on a given date.
- A function to retrieve the name of a Japanese holiday based on a given date.

## Quick Start
Install the NuGet package as follows.

```
dotnet add package JpCalendar
```

`JpCalendar` can be DI by `Generic Host`. Register for DI as follows.

```csharp
using JpCalendar;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddJpCalendar();
```

# Author
[@zwei_222](https://twitter.com/zwei_222)

# License
This software is released under the MIT License, see LICENSE.
