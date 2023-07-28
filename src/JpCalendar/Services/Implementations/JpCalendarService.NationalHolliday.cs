using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JpCalendar.Internals;

namespace JpCalendar.Services.Implementations;

internal sealed partial class JpCalendarService
{
    private const string SubstitutionDay = "振替休日";

    private const string NationalHolidayByLaw = "国民の休日";

    private const int MonthOfShunbun = 3;

    private const int MonthOfShubun = 9;

    private static readonly DateOnly StartSubstitutionDay = new(1973, 4, 30);

    private static readonly DateOnly StartNationalHolidayByLaw = new(1988, 1, 1);

    private readonly List<NationalHoliday> nationalHolidays = new();

    private readonly Dictionary<int, int> shunbunDayDictionary = new();

    private readonly Dictionary<int, int> shubunDayDictionary = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNationalHoliday(DateTime dateTime)
    {
        return this.IsNationalHoliday(DateOnly.FromDateTime(dateTime));
    }

    public bool IsNationalHoliday(DateOnly date)
    {
        var isNationalHoliday = this.IsNationalHolidayInner(date);

        if (isNationalHoliday)
        {
            return true;
        }

        return this.IsBetweenNationalHoliday(date) && date >= StartNationalHolidayByLaw;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetNationalHolidayName(DateTime dateTime)
    {
        return this.GetNationalHolidayName(DateOnly.FromDateTime(dateTime));
    }

    public string? GetNationalHolidayName(DateOnly date)
    {
        foreach (var nationalHoliday in CollectionsMarshal.AsSpan(this.nationalHolidays))
        {
            if (nationalHoliday.StartDate is not null &&
                date < nationalHoliday.StartDate)
            {
                continue;
            }

            if (nationalHoliday.EndDate is not null &&
                date > nationalHoliday.EndDate)
            {
                continue;
            }

            var exceptionDate = nationalHoliday.ExceptionDate?.FirstOrDefault(item => item.Year == date.Year);

            if (exceptionDate is not null && exceptionDate != default(DateOnly))
            {
                if (exceptionDate.Value.Month != date.Month)
                {
                    continue;
                }

                if (exceptionDate.Value.Day != date.Day)
                {
                    if ((exceptionDate.Value.Day + nationalHoliday.TransferPeriod) >= date.Day &&
                        date.AddDays(-1).DayOfWeek is DayOfWeek.Sunday &&
                        date >= StartSubstitutionDay)
                    {
                        return SubstitutionDay;
                    }

                    continue;
                }

                return nationalHoliday.Name;
            }
            else if (nationalHoliday.IsFixedDay)
            {
                if (nationalHoliday.Month != date.Month)
                {
                    continue;
                }

                if (nationalHoliday.Day != date.Day)
                {
                    if ((nationalHoliday.Day + nationalHoliday.TransferPeriod) >= date.Day &&
                        date.AddDays(-1).DayOfWeek is DayOfWeek.Sunday &&
                        date >= StartSubstitutionDay)
                    {
                        return SubstitutionDay;
                    }

                    continue;
                }

                return nationalHoliday.Name;
            }
            else if (nationalHoliday.IsFixedWeek)
            {
                if (nationalHoliday.Month != date.Month)
                {
                    continue;
                }

                var week = (date.Day - 1) / 7 + 1;

                if (nationalHoliday.Week != week)
                {
                    continue;
                }

                if (nationalHoliday.DayOfWeek != (int)date.DayOfWeek)
                {
                    continue;
                }

                return nationalHoliday.Name;
            }
        }

        if (this.IsDayOfShunbun(date.Year, date.Month, date.Day, out var isSubstitutionDay))
        {
            return "春分の日";
        }

        if (isSubstitutionDay)
        {
            return SubstitutionDay;
        }

        if (this.IsDayOfShubun(date.Year, date.Month, date.Day, out isSubstitutionDay))
        {
            return "秋分の日";
        }

        if (isSubstitutionDay)
        {
            return SubstitutionDay;
        }

        if (this.IsBetweenNationalHoliday(date) &&
            date >= StartNationalHolidayByLaw)
        {
            return NationalHolidayByLaw;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsBetweenNationalHoliday(DateOnly date)
    {
        return this.IsNationalHolidayInner(date.AddDays(-1)) ||
               this.IsNationalHolidayInner(date.AddDays(1));
    }

    private bool IsNationalHolidayInner(DateOnly date)
    {
        foreach (var nationalHoliday in CollectionsMarshal.AsSpan(this.nationalHolidays))
        {
            if (nationalHoliday.StartDate is not null &&
                date < nationalHoliday.StartDate)
            {
                continue;
            }

            if (nationalHoliday.EndDate is not null &&
                date > nationalHoliday.EndDate)
            {
                continue;
            }

            var exceptionDate = nationalHoliday.ExceptionDate?.FirstOrDefault(item => item.Year == date.Year);

            if (exceptionDate is not null && exceptionDate != default(DateOnly))
            {
                if (exceptionDate.Value.Month != date.Month)
                {
                    continue;
                }

                if (exceptionDate.Value.Day != date.Day)
                {
                    if ((exceptionDate.Value.Day + nationalHoliday.TransferPeriod) >= date.Day &&
                        date.AddDays(-1).DayOfWeek is DayOfWeek.Sunday &&
                        date >= StartSubstitutionDay)
                    {
                        return true;
                    }

                    continue;
                }

                return true;
            }
            else if (nationalHoliday.IsFixedDay)
            {
                if (nationalHoliday.Month != date.Month)
                {
                    continue;
                }

                if (nationalHoliday.Day != date.Day)
                {
                    if ((nationalHoliday.Day + nationalHoliday.TransferPeriod) >= date.Day &&
                        date.AddDays(-1).DayOfWeek is DayOfWeek.Sunday &&
                        date >= StartSubstitutionDay)
                    {
                        return true;
                    }

                    continue;
                }

                return true;
            }
            else if (nationalHoliday.IsFixedWeek)
            {
                if (nationalHoliday.Month != date.Month)
                {
                    continue;
                }

                var week = (date.Day - 1) / 7 + 1;

                if (nationalHoliday.Week != week)
                {
                    continue;
                }

                if (nationalHoliday.DayOfWeek != (int)date.DayOfWeek)
                {
                    continue;
                }

                return true;
            }
        }

        if (this.IsDayOfShunbun(date.Year, date.Month, date.Day, out var isSubstitutionDay))
        {
            return true;
        }

        if (isSubstitutionDay)
        {
            return true;
        }

        if (this.IsDayOfShubun(date.Year, date.Month, date.Day, out isSubstitutionDay))
        {
            return true;
        }

        if (isSubstitutionDay)
        {
            return true;
        }

        return false;
    }

    private bool IsDayOfShunbun(int year, int month, int day, out bool isSubstitutionDay)
    {
        isSubstitutionDay = false;

        if (month != MonthOfShunbun)
        {
            return false;
        }

        ref var dayOfShunbun = ref CollectionsMarshal.GetValueRefOrNullRef(
            this.shunbunDayDictionary,
            year);

        if (Unsafe.IsNullRef(ref dayOfShunbun))
        {
            return false;
        }

        if (day == dayOfShunbun)
        {
            return true;
        }

        if (day - 1 == dayOfShunbun)
        {
            var date  = new DateOnly(year, month, dayOfShunbun);

            if (date.DayOfWeek is DayOfWeek.Sunday &&
                date >= StartSubstitutionDay)
            {
                isSubstitutionDay = true;
            }
        }

        return false;
    }

    private bool IsDayOfShubun(int year, int month, int day, out bool isSubstitutionDay)
    {
        isSubstitutionDay = false;

        if (month != MonthOfShubun)
        {
            return false;
        }

        ref var dayOfShubun = ref CollectionsMarshal.GetValueRefOrNullRef(
            this.shubunDayDictionary,
            year);

        if (Unsafe.IsNullRef(ref dayOfShubun))
        {
            return false;
        }

        if (day == dayOfShubun)
        {
            return true;
        }

        if (day - 1 == dayOfShubun)
        {
            var date = new DateOnly(year, month, dayOfShubun);

            if (date.DayOfWeek is DayOfWeek.Sunday &&
                date >= StartSubstitutionDay)
            {
                isSubstitutionDay = true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InitializeNationalHolidays()
    {
        this.InitializeFixedDayOfNationalHolidays();
        this.InitializeFixedWeekOfNationalHolidays();
        this.InitializeDayOfShunbun();
        this.InitializeDayOfShubun();
    }

    private void InitializeFixedDayOfNationalHolidays()
    {
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "元日",
            Month = 1,
            Day = 1,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "休日",
            Month = 1,
            Day = 2,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "成人の日",
            Month = 1,
            Day = 15,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = new DateOnly(1999, 12, 31),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "建国記念日",
            Month = 2,
            Day = 11,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1967, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "天皇誕生日",
            Month = 2,
            Day = 23,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(2020, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "大喪の礼",
            Month = 2,
            Day = 24,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1989, 2, 24),
            EndDate = new DateOnly(1989, 2, 24),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "結婚の儀",
            Month = 4,
            Day = 10,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1959, 4, 10),
            EndDate = new DateOnly(1959, 4, 10),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "天皇誕生日",
            Month = 4,
            Day = 29,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = new DateOnly(1988, 12, 31),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "みどりの日",
            Month = 4,
            Day = 29,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1989, 1, 1),
            EndDate = new DateOnly(2006, 12, 31),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "昭和の日",
            Month = 4,
            Day = 29,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "休日",
            Month = 5,
            Day = 1,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(2019, 5, 1),
            EndDate = new DateOnly(2019, 5, 1),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "憲法記念日",
            Month = 5,
            Day = 3,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 2,
            Name = "みどりの日",
            Month = 5,
            Day = 4,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 3,
            Name = "こどもの日",
            Month = 5,
            Day = 5,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "結婚の儀",
            Month = 6,
            Day = 9,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1993, 6, 9),
            EndDate = new DateOnly(1993, 6, 9),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "海の日",
            Month = 7,
            Day = 20,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1996, 1, 1),
            EndDate = new DateOnly(2002, 12, 31),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "山の日",
            Month = 8,
            Day = 11,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(2016, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "敬老の日",
            Month = 9,
            Day = 15,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1966, 1, 1),
            EndDate = new DateOnly(2002, 12, 31),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "体育の日",
            Month = 10,
            Day = 10,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1966, 1, 1),
            EndDate = new DateOnly(1999, 12, 31),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "即位礼正殿の儀",
            Month = 10,
            Day = 22,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(2019, 10, 22),
            EndDate = new DateOnly(2019, 10, 22),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "文化の日",
            Month = 11,
            Day = 3,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "即位礼正殿の儀",
            Month = 11,
            Day = 12,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1990, 11, 12),
            EndDate = new DateOnly(1990, 11, 12),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "勤労感謝の日",
            Month = 11,
            Day = 23,
            Week = -1,
            DayOfWeek = -1,
            StartDate = null,
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = true,
            IsFixedWeek = false,
            TransferPeriod = 1,
            Name = "天皇誕生日",
            Month = 12,
            Day = 23,
            Week = -1,
            DayOfWeek = -1,
            StartDate = new DateOnly(1989, 1, 1),
            EndDate = new DateOnly(2018, 12, 31),
            ExceptionDate = null,
        });
    }

    private void InitializeFixedWeekOfNationalHolidays()
    {
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = false,
            IsFixedWeek = true,
            TransferPeriod = 1,
            Name = "成人の日",
            Month = 1,
            Day = -1,
            Week = 2,
            DayOfWeek = 1,
            StartDate = new DateOnly(2000, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = false,
            IsFixedWeek = true,
            TransferPeriod = 1,
            Name = "海の日",
            Month = 7,
            Day = -1,
            Week = 3,
            DayOfWeek = 1,
            StartDate = new DateOnly(2003, 1, 1),
            EndDate = null,
            ExceptionDate = new[]
            {
                new DateOnly(2020, 7, 23),
                new DateOnly(2021, 7, 22),
            },
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = false,
            IsFixedWeek = true,
            TransferPeriod = 1,
            Name = "敬老の日",
            Month = 9,
            Day = -1,
            Week = 3,
            DayOfWeek = 1,
            StartDate = new DateOnly(2003, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = false,
            IsFixedWeek = true,
            TransferPeriod = 1,
            Name = "スポーツの日",
            Month = 10,
            Day = -1,
            Week = 2,
            DayOfWeek = 1,
            StartDate = new DateOnly(2000, 1, 1),
            EndDate = new DateOnly(2019, 12, 31),
            ExceptionDate = null,
        });
        this.nationalHolidays.Add(new NationalHoliday
        {
            IsFixedDay = false,
            IsFixedWeek = true,
            TransferPeriod = 1,
            Name = "スポーツの日",
            Month = 10,
            Day = -1,
            Week = 2,
            DayOfWeek = 1,
            StartDate = new DateOnly(2020, 1, 1),
            EndDate = null,
            ExceptionDate = null,
        });
    }

    private void InitializeDayOfShunbun()
    {
        this.shunbunDayDictionary.Add(1955, 21);
        this.shunbunDayDictionary.Add(1956, 21);
        this.shunbunDayDictionary.Add(1957, 21);
        this.shunbunDayDictionary.Add(1958, 21);
        this.shunbunDayDictionary.Add(1959, 21);
        this.shunbunDayDictionary.Add(1960, 20);
        this.shunbunDayDictionary.Add(1961, 21);
        this.shunbunDayDictionary.Add(1962, 21);
        this.shunbunDayDictionary.Add(1963, 21);
        this.shunbunDayDictionary.Add(1964, 20);
        this.shunbunDayDictionary.Add(1965, 21);
        this.shunbunDayDictionary.Add(1966, 21);
        this.shunbunDayDictionary.Add(1967, 21);
        this.shunbunDayDictionary.Add(1968, 20);
        this.shunbunDayDictionary.Add(1969, 21);
        this.shunbunDayDictionary.Add(1970, 21);
        this.shunbunDayDictionary.Add(1971, 21);
        this.shunbunDayDictionary.Add(1972, 20);
        this.shunbunDayDictionary.Add(1973, 21);
        this.shunbunDayDictionary.Add(1974, 21);
        this.shunbunDayDictionary.Add(1975, 21);
        this.shunbunDayDictionary.Add(1976, 20);
        this.shunbunDayDictionary.Add(1977, 21);
        this.shunbunDayDictionary.Add(1978, 21);
        this.shunbunDayDictionary.Add(1979, 21);
        this.shunbunDayDictionary.Add(1980, 20);
        this.shunbunDayDictionary.Add(1981, 21);
        this.shunbunDayDictionary.Add(1982, 21);
        this.shunbunDayDictionary.Add(1983, 21);
        this.shunbunDayDictionary.Add(1984, 20);
        this.shunbunDayDictionary.Add(1985, 21);
        this.shunbunDayDictionary.Add(1986, 21);
        this.shunbunDayDictionary.Add(1987, 21);
        this.shunbunDayDictionary.Add(1988, 20);
        this.shunbunDayDictionary.Add(1989, 21);
        this.shunbunDayDictionary.Add(1990, 21);
        this.shunbunDayDictionary.Add(1991, 21);
        this.shunbunDayDictionary.Add(1992, 20);
        this.shunbunDayDictionary.Add(1993, 20);
        this.shunbunDayDictionary.Add(1994, 21);
        this.shunbunDayDictionary.Add(1995, 21);
        this.shunbunDayDictionary.Add(1996, 20);
        this.shunbunDayDictionary.Add(1997, 20);
        this.shunbunDayDictionary.Add(1998, 21);
        this.shunbunDayDictionary.Add(1999, 21);
        this.shunbunDayDictionary.Add(2000, 20);
        this.shunbunDayDictionary.Add(2001, 20);
        this.shunbunDayDictionary.Add(2002, 21);
        this.shunbunDayDictionary.Add(2003, 21);
        this.shunbunDayDictionary.Add(2004, 20);
        this.shunbunDayDictionary.Add(2005, 20);
        this.shunbunDayDictionary.Add(2006, 21);
        this.shunbunDayDictionary.Add(2007, 21);
        this.shunbunDayDictionary.Add(2008, 20);
        this.shunbunDayDictionary.Add(2009, 20);
        this.shunbunDayDictionary.Add(2010, 21);
        this.shunbunDayDictionary.Add(2011, 21);
        this.shunbunDayDictionary.Add(2012, 20);
        this.shunbunDayDictionary.Add(2013, 20);
        this.shunbunDayDictionary.Add(2014, 21);
        this.shunbunDayDictionary.Add(2015, 21);
        this.shunbunDayDictionary.Add(2016, 20);
        this.shunbunDayDictionary.Add(2017, 20);
        this.shunbunDayDictionary.Add(2018, 21);
        this.shunbunDayDictionary.Add(2019, 21);
        this.shunbunDayDictionary.Add(2020, 20);
        this.shunbunDayDictionary.Add(2021, 20);
        this.shunbunDayDictionary.Add(2022, 21);
        this.shunbunDayDictionary.Add(2023, 21);
        this.shunbunDayDictionary.Add(2024, 20);
        this.shunbunDayDictionary.Add(2025, 20);
        this.shunbunDayDictionary.Add(2026, 20);
        this.shunbunDayDictionary.Add(2027, 21);
        this.shunbunDayDictionary.Add(2028, 20);
        this.shunbunDayDictionary.Add(2029, 20);
        this.shunbunDayDictionary.Add(2030, 20);
        this.shunbunDayDictionary.Add(2031, 21);
        this.shunbunDayDictionary.Add(2032, 20);
        this.shunbunDayDictionary.Add(2033, 20);
        this.shunbunDayDictionary.Add(2034, 20);
        this.shunbunDayDictionary.Add(2035, 21);
        this.shunbunDayDictionary.Add(2036, 20);
        this.shunbunDayDictionary.Add(2037, 20);
        this.shunbunDayDictionary.Add(2038, 20);
        this.shunbunDayDictionary.Add(2039, 21);
        this.shunbunDayDictionary.Add(2040, 20);
        this.shunbunDayDictionary.Add(2041, 20);
        this.shunbunDayDictionary.Add(2042, 20);
        this.shunbunDayDictionary.Add(2043, 21);
        this.shunbunDayDictionary.Add(2044, 20);
        this.shunbunDayDictionary.Add(2045, 20);
        this.shunbunDayDictionary.Add(2046, 20);
        this.shunbunDayDictionary.Add(2047, 21);
        this.shunbunDayDictionary.Add(2048, 20);
        this.shunbunDayDictionary.Add(2049, 20);
        this.shunbunDayDictionary.Add(2050, 20);
    }

    private void InitializeDayOfShubun()
    {
        this.shubunDayDictionary.Add(1955, 24);
        this.shubunDayDictionary.Add(1956, 23);
        this.shubunDayDictionary.Add(1957, 23);
        this.shubunDayDictionary.Add(1958, 23);
        this.shubunDayDictionary.Add(1959, 24);
        this.shubunDayDictionary.Add(1960, 23);
        this.shubunDayDictionary.Add(1961, 23);
        this.shubunDayDictionary.Add(1962, 23);
        this.shubunDayDictionary.Add(1963, 24);
        this.shubunDayDictionary.Add(1964, 23);
        this.shubunDayDictionary.Add(1965, 23);
        this.shubunDayDictionary.Add(1966, 23);
        this.shubunDayDictionary.Add(1967, 24);
        this.shubunDayDictionary.Add(1968, 23);
        this.shubunDayDictionary.Add(1969, 23);
        this.shubunDayDictionary.Add(1970, 23);
        this.shubunDayDictionary.Add(1971, 24);
        this.shubunDayDictionary.Add(1972, 23);
        this.shubunDayDictionary.Add(1973, 23);
        this.shubunDayDictionary.Add(1974, 23);
        this.shubunDayDictionary.Add(1975, 24);
        this.shubunDayDictionary.Add(1976, 23);
        this.shubunDayDictionary.Add(1977, 23);
        this.shubunDayDictionary.Add(1978, 23);
        this.shubunDayDictionary.Add(1979, 24);
        this.shubunDayDictionary.Add(1980, 23);
        this.shubunDayDictionary.Add(1981, 23);
        this.shubunDayDictionary.Add(1982, 23);
        this.shubunDayDictionary.Add(1983, 23);
        this.shubunDayDictionary.Add(1984, 23);
        this.shubunDayDictionary.Add(1985, 23);
        this.shubunDayDictionary.Add(1986, 23);
        this.shubunDayDictionary.Add(1987, 23);
        this.shubunDayDictionary.Add(1988, 23);
        this.shubunDayDictionary.Add(1989, 23);
        this.shubunDayDictionary.Add(1990, 23);
        this.shubunDayDictionary.Add(1991, 23);
        this.shubunDayDictionary.Add(1992, 23);
        this.shubunDayDictionary.Add(1993, 23);
        this.shubunDayDictionary.Add(1994, 23);
        this.shubunDayDictionary.Add(1995, 23);
        this.shubunDayDictionary.Add(1996, 23);
        this.shubunDayDictionary.Add(1997, 23);
        this.shubunDayDictionary.Add(1998, 23);
        this.shubunDayDictionary.Add(1999, 23);
        this.shubunDayDictionary.Add(2000, 23);
        this.shubunDayDictionary.Add(2001, 23);
        this.shubunDayDictionary.Add(2002, 23);
        this.shubunDayDictionary.Add(2003, 23);
        this.shubunDayDictionary.Add(2004, 23);
        this.shubunDayDictionary.Add(2005, 23);
        this.shubunDayDictionary.Add(2006, 23);
        this.shubunDayDictionary.Add(2007, 23);
        this.shubunDayDictionary.Add(2008, 23);
        this.shubunDayDictionary.Add(2009, 23);
        this.shubunDayDictionary.Add(2010, 23);
        this.shubunDayDictionary.Add(2011, 23);
        this.shubunDayDictionary.Add(2012, 22);
        this.shubunDayDictionary.Add(2013, 23);
        this.shubunDayDictionary.Add(2014, 23);
        this.shubunDayDictionary.Add(2015, 23);
        this.shubunDayDictionary.Add(2016, 22);
        this.shubunDayDictionary.Add(2017, 23);
        this.shubunDayDictionary.Add(2018, 23);
        this.shubunDayDictionary.Add(2019, 23);
        this.shubunDayDictionary.Add(2020, 22);
        this.shubunDayDictionary.Add(2021, 23);
        this.shubunDayDictionary.Add(2022, 23);
        this.shubunDayDictionary.Add(2023, 23);
        this.shubunDayDictionary.Add(2024, 22);
        this.shubunDayDictionary.Add(2025, 23);
        this.shubunDayDictionary.Add(2026, 23);
        this.shubunDayDictionary.Add(2027, 23);
        this.shubunDayDictionary.Add(2028, 22);
        this.shubunDayDictionary.Add(2029, 23);
        this.shubunDayDictionary.Add(2030, 23);
        this.shubunDayDictionary.Add(2031, 23);
        this.shubunDayDictionary.Add(2032, 22);
        this.shubunDayDictionary.Add(2033, 23);
        this.shubunDayDictionary.Add(2034, 23);
        this.shubunDayDictionary.Add(2035, 23);
        this.shubunDayDictionary.Add(2036, 22);
        this.shubunDayDictionary.Add(2037, 23);
        this.shubunDayDictionary.Add(2038, 23);
        this.shubunDayDictionary.Add(2039, 23);
        this.shubunDayDictionary.Add(2040, 22);
        this.shubunDayDictionary.Add(2041, 23);
        this.shubunDayDictionary.Add(2042, 23);
        this.shubunDayDictionary.Add(2043, 23);
        this.shubunDayDictionary.Add(2044, 22);
        this.shubunDayDictionary.Add(2045, 22);
        this.shubunDayDictionary.Add(2046, 23);
        this.shubunDayDictionary.Add(2047, 23);
        this.shubunDayDictionary.Add(2048, 22);
        this.shubunDayDictionary.Add(2049, 22);
        this.shubunDayDictionary.Add(2050, 23);
    }
}
