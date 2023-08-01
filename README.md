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

## Examples
In addition to the library, the ASP.NET Core Web API project exists in this repository.
By running the Web API project, it is possible to see the library in action on the Swagger UI.

### Era
![image](https://github.com/zwei222/JpCalendar/assets/46480712/e50001e0-0af1-4bbd-a185-b9086f4d3de5)

### National holiday
![image](https://github.com/zwei222/JpCalendar/assets/46480712/19eb63cd-b5cb-4b66-a727-dab76366e253)

![image](https://github.com/zwei222/JpCalendar/assets/46480712/3771597a-93cb-45b5-b9ed-1f1a51037071)

### Rokuyo
![image](https://github.com/zwei222/JpCalendar/assets/46480712/c27c43fe-86e3-4a06-a89b-11e1809a6e5c)

![image](https://github.com/zwei222/JpCalendar/assets/46480712/a52bb544-468e-4dcf-97b3-1b5f7ee4d16a)

# Author
[@zwei_222](https://twitter.com/zwei_222)

# License
This software is released under the MIT License, see LICENSE.
