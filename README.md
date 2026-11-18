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

## Target frameworks
Version 2.0.0 ships `net10.0`, `net11.0` and `netstandard2.0` assets. The `net6.0`, `net7.0` and
`net8.0` assets from 1.x are no longer produced. Applications on those frameworks resolve the
`netstandard2.0` asset, which does not expose the `DateOnly` overloads (`GetEra(DateOnly)`,
`GetEraIndex(DateOnly)`, `GetRokuyo(DateOnly)`, `IsNationalHoliday(DateOnly)` and
`GetNationalHolidayName(DateOnly)`). Use the `DateTime` overloads there, or target `net10.0` or later.

## Usage
### Era
Get the Japanese calendar name corresponding to the date passed to `GetEra(date)`.

```csharp
private string ToJapaneseString(DateTime date, string format)
{
    return $"{Calendar.GetEra(date).Name}{date.ToString(format, Calendar.JapaneseCultureInfo)}";
}
```

```
# date <- 2023-08-07
令和5年08月07日
```

### National holiday
Get the name of the holiday corresponding to the date passed to `GetNationalHolidayName(date)`. Returns `null` if the date is not a national holiday.

```csharp
private string GetJapaneseNationalHoliday(DateTime date)
{
    return Calendar.GetNationalHolidayName(date);
}
```

```
# date <- 2023-08-11
山の日
```

### Rokuyo
Get the "Rokuyo" corresponding to the date passed to `GetRokuyo(date)`.

```csharp
private string GetRokuyo(DateTime date)
{
    return Calendar.GetRokuyo(date);
}
```

```
# date <- 2023-08-07
友引
```

## Examples
In addition to the library, the ASP.NET Core Web API project exists in this repository.
By running the Web API project, it is possible to see the library in action on the Swagger UI.

### Era
![image](https://user-images.githubusercontent.com/46480712/257435783-e50001e0-0af1-4bbd-a185-b9086f4d3de5.png)

### National holiday
![image](https://user-images.githubusercontent.com/46480712/257436523-19eb63cd-b5cb-4b66-a727-dab76366e253.png)

![image](https://user-images.githubusercontent.com/46480712/257436895-3771597a-93cb-45b5-b9ed-1f1a51037071.png)

### Rokuyo
![image](https://user-images.githubusercontent.com/46480712/257437281-c27c43fe-86e3-4a06-a89b-11e1809a6e5c.png)

![image](https://user-images.githubusercontent.com/46480712/257437585-a52bb544-468e-4dcf-97b3-1b5f7ee4d16a.png)

## Data
The test fixture `docs/syukujitsu.csv` is the unmodified CSV published by the Cabinet Office:
https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv

It is provided under the Public Data License (PDL) 1.0. See [docs/syukujitsu.md](docs/syukujitsu.md)
for the source, checksum and provenance notice. The source code of this repository remains under
the MIT License (see LICENSE).

# Author
[@zwei_222](https://twitter.com/zwei_222)

# License
This software is released under the MIT License, see LICENSE.
