using System.Runtime.CompilerServices;
using JpCalendar.Internals;

namespace JpCalendar.Services.Implementations;

internal sealed partial class JpCalendarService
{
    private const string SubstitutionDay = "振替休日";

    private const string NationalHolidayByLaw = "国民の休日";

    private const int MonthOfVernalEquinoxDay = 3;

    private const int MonthOfAutumnalEquinoxDay = 9;

    private const double FractionOfYear = 0.242194;

    private static readonly
#if NET6_0_OR_GREATER
        DateOnly
#else
        DateTime
#endif
        StartSubstitutionDay = new(1973, 4, 30);

    private static readonly
#if NET6_0_OR_GREATER
        DateOnly
#else
        DateTime
#endif
        StartNationalHolidayByLaw = new(1988, 1, 1);

    private readonly NationalHolidayList nationalHolidayList;

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNationalHoliday(DateTime dateTime)
    {
        return this.IsNationalHoliday(DateOnly.FromDateTime(dateTime));
    }
#endif

    public bool IsNationalHoliday(
#if NET6_0_OR_GREATER
        DateOnly
#else
        DateTime
#endif
            date)
    {
        var isNationalHoliday = this.IsNationalHolidayInner(date);

        if (isNationalHoliday)
        {
            return true;
        }

        return this.IsNationalHolidayByLaw(date);
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetNationalHolidayName(DateTime dateTime)
    {
        return this.GetNationalHolidayName(DateOnly.FromDateTime(dateTime));
    }
#endif

    public string? GetNationalHolidayName(
#if NET6_0_OR_GREATER
        DateOnly
#else
        DateTime
#endif
            date)
    {
        foreach (var nationalHoliday in this.nationalHolidayList.GetNationalHolidays(date.Month))
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

#if NET6_0_OR_GREATER
            if (exceptionDate is not null && exceptionDate != default(DateOnly))
#else
            if (exceptionDate is not null && exceptionDate != default(DateTime))
#endif
            {
                if (exceptionDate.Value.Month != date.Month)
                {
                    continue;
                }

                if (exceptionDate.Value.Day != date.Day)
                {
                    if ((exceptionDate.Value.Day + nationalHoliday.TransferPeriod) == date.Day &&
                        date.AddDays(-(nationalHoliday.TransferPeriod)).DayOfWeek is DayOfWeek.Sunday &&
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
                    if ((nationalHoliday.Day + nationalHoliday.TransferPeriod) == date.Day &&
                        date.AddDays(-(nationalHoliday.TransferPeriod)).DayOfWeek is DayOfWeek.Sunday &&
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

        if (this.IsVernalEquinoxDay(date.Year, date.Month, date.Day, out var isSubstitutionDay))
        {
            return "春分の日";
        }

        if (isSubstitutionDay)
        {
            return SubstitutionDay;
        }

        if (this.IsAutumnalEquinoxDay(date.Year, date.Month, date.Day, out isSubstitutionDay))
        {
            return "秋分の日";
        }

        if (isSubstitutionDay)
        {
            return SubstitutionDay;
        }

        if (this.IsNationalHolidayByLaw(date))
        {
            return NationalHolidayByLaw;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsNationalHolidayByLaw(
#if NET6_0_OR_GREATER
        DateOnly
#else
        DateTime
#endif
            date)
    {
        return this.IsNationalHolidayInner(date.AddDays(-1)) &&
               this.IsNationalHolidayInner(date.AddDays(1)) &&
               date.DayOfWeek is not DayOfWeek.Sunday &&
               date >= StartNationalHolidayByLaw;
    }

    private bool IsNationalHolidayInner(
#if NET6_0_OR_GREATER
        DateOnly
#else
        DateTime
#endif
            date)
    {
        foreach (var nationalHoliday in this.nationalHolidayList.GetNationalHolidays(date.Month))
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

#if NET6_0_OR_GREATER
            if (exceptionDate is not null && exceptionDate != default(DateOnly))
#else
            if (exceptionDate is not null && exceptionDate != default(DateTime))
#endif
            {
                if (exceptionDate.Value.Month != date.Month)
                {
                    continue;
                }

                if (exceptionDate.Value.Day != date.Day)
                {
                    if ((exceptionDate.Value.Day + nationalHoliday.TransferPeriod) == date.Day &&
                        date.AddDays(-(nationalHoliday.TransferPeriod)).DayOfWeek is DayOfWeek.Sunday &&
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
                    if ((nationalHoliday.Day + nationalHoliday.TransferPeriod) == date.Day &&
                        date.AddDays(-(nationalHoliday.TransferPeriod)).DayOfWeek is DayOfWeek.Sunday &&
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

        if (this.IsVernalEquinoxDay(date.Year, date.Month, date.Day, out var isSubstitutionDay))
        {
            return true;
        }

        if (isSubstitutionDay)
        {
            return true;
        }

        if (this.IsAutumnalEquinoxDay(date.Year, date.Month, date.Day, out isSubstitutionDay))
        {
            return true;
        }

        if (isSubstitutionDay)
        {
            return true;
        }

        return false;
    }

    private bool IsVernalEquinoxDay(int year, int month, int day, out bool isSubstitutionDay)
    {
        isSubstitutionDay = false;

        var vernalEquinoxDay = this.GetVernalEquinoxDay(year, month);

        if (day == vernalEquinoxDay)
        {
            return true;
        }

        if (day - 1 == vernalEquinoxDay)
        {
#if NET6_0_OR_GREATER
            var date  = new DateOnly(year, month, vernalEquinoxDay);
#else
            var date  = new DateTime(year, month, vernalEquinoxDay);
#endif

            if (date.DayOfWeek is DayOfWeek.Sunday &&
                date >= StartSubstitutionDay)
            {
                isSubstitutionDay = true;
            }
        }

        return false;
    }

    private int GetVernalEquinoxDay(int year, int month)
    {
        if (month != MonthOfVernalEquinoxDay)
        {
            return -1;
        }

        double dateOfPassage;
        int leapYear;

        if (year < 1851)
        {
            // Cannot be calculated before 1850.
            return -1;
        }
        else if (year < 1900)
        {
            dateOfPassage = 19.8277;
            leapYear = 1983;
        }
        else if (year < 1980)
        {
            dateOfPassage = 20.8357;
            leapYear = 1983;
        }
        else if (year < 2100)
        {
            dateOfPassage = 20.8431;
            leapYear = 1980;
        }
        else if (year < 2151)
        {
            dateOfPassage = 21.8510;
            leapYear = 1980;
        }
        else
        {
            // Cannot be calculated after 2150.
            return -1;
        }

        var value = Math.Truncate(dateOfPassage + FractionOfYear * (year - 1980)) -
                    Math.Truncate((year - leapYear) / 4D);

        return (int)value;
    }

    private bool IsAutumnalEquinoxDay(int year, int month, int day, out bool isSubstitutionDay)
    {
        isSubstitutionDay = false;

        var autumnalEquinoxDay = this.GetAutumnalEquinoxDay(year, month);

        if (day == autumnalEquinoxDay)
        {
            return true;
        }

        if (day - 1 == autumnalEquinoxDay)
        {
#if NET6_0_OR_GREATER
            var date = new DateOnly(year, month, autumnalEquinoxDay);
#else
            var date = new DateTime(year, month, autumnalEquinoxDay);
#endif

            if (date.DayOfWeek is DayOfWeek.Sunday &&
                date >= StartSubstitutionDay)
            {
                isSubstitutionDay = true;
            }
        }

        return false;
    }

    private int GetAutumnalEquinoxDay(int year, int month)
    {
        if (month != MonthOfAutumnalEquinoxDay)
        {
            return -1;
        }

        double dateOfPassage;
        int leapYear;

        if (year < 1850)
        {
            // Cannot be calculated before 1850.
            return -1;
        }
        else if (year < 1900)
        {
            dateOfPassage = 22.2588;
            leapYear = 1983;
        }
        else if (year < 1980)
        {
            dateOfPassage = 23.2588;
            leapYear = 1983;
        }
        else if (year < 2100)
        {
            dateOfPassage = 23.2488;
            leapYear = 1980;
        }
        else if (year < 2151)
        {
            dateOfPassage = 24.2488;
            leapYear = 1980;
        }
        else
        {
            // Cannot be calculated after 2150.
            return -1;
        }

        var value = Math.Truncate(dateOfPassage + FractionOfYear * (year - 1980)) -
                    Math.Truncate((year - leapYear) / 4D);

        return (int)value;
    }

    private NationalHoliday[] GetNationalHolidays()
    {
        return new[]
        {
            new NationalHoliday
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
            },
            new NationalHoliday
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
#if NET6_0_OR_GREATER
                EndDate = new DateOnly(1999, 12, 31),
#else
                EndDate = new DateTime(1999, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = false,
                IsFixedWeek = true,
                TransferPeriod = 1,
                Name = "成人の日",
                Month = 1,
                Day = -1,
                Week = 2,
                DayOfWeek = 1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2000, 1, 1),
#else
                StartDate = new DateTime(2000, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "建国記念日",
                Month = 2,
                Day = 11,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1967, 1, 1),
#else
                StartDate = new DateTime(1967, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "天皇誕生日",
                Month = 2,
                Day = 23,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2020, 1, 1),
#else
                StartDate = new DateTime(2020, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "大喪の礼",
                Month = 2,
                Day = 24,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1989, 2, 24),
                EndDate = new DateOnly(1989, 2, 24),
#else
                StartDate = new DateTime(1989, 2, 24),
                EndDate = new DateTime(1989, 2, 24),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "結婚の儀",
                Month = 4,
                Day = 10,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1959, 4, 10),
                EndDate = new DateOnly(1959, 4, 10),
#else
                StartDate = new DateTime(1959, 4, 10),
                EndDate = new DateTime(1959, 4, 10),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
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
#if NET6_0_OR_GREATER
                EndDate = new DateOnly(1988, 12, 31),
#else
                EndDate = new DateTime(1988, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "みどりの日",
                Month = 4,
                Day = 29,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1989, 1, 1),
                EndDate = new DateOnly(2006, 12, 31),
#else
                StartDate = new DateTime(1989, 1, 1),
                EndDate = new DateTime(2006, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "昭和の日",
                Month = 4,
                Day = 29,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2007, 1, 1),
#else
                StartDate = new DateTime(2007, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "休日",
                Month = 5,
                Day = 1,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2019, 5, 1),
                EndDate = new DateOnly(2019, 5, 1),
#else
                StartDate = new DateTime(2019, 5, 1),
                EndDate = new DateTime(2019, 5, 1),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
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
#if NET6_0_OR_GREATER
                EndDate = new DateOnly(2006, 12, 31),
#else
                EndDate = new DateTime(2006, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 3,
                Name = "憲法記念日",
                Month = 5,
                Day = 3,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2007, 1, 1),
#else
                StartDate = new DateTime(2007, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 2,
                Name = "みどりの日",
                Month = 5,
                Day = 4,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2007, 1, 1),
#else
                StartDate = new DateTime(2007, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "こどもの日",
                Month = 5,
                Day = 5,
                Week = -1,
                DayOfWeek = -1,
                StartDate = null,
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "結婚の儀",
                Month = 6,
                Day = 9,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1993, 6, 9),
                EndDate = new DateOnly(1993, 6, 9),
#else
                StartDate = new DateTime(1993, 6, 9),
                EndDate = new DateTime(1993, 6, 9),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "海の日",
                Month = 7,
                Day = 20,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1996, 1, 1),
                EndDate = new DateOnly(2002, 12, 31),
#else
                StartDate = new DateTime(1996, 1, 1),
                EndDate = new DateTime(2002, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = false,
                IsFixedWeek = true,
                TransferPeriod = 1,
                Name = "海の日",
                Month = 7,
                Day = -1,
                Week = 3,
                DayOfWeek = 1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2003, 1, 1),
#else
                StartDate = new DateTime(2003, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = new[]
                {
#if NET6_0_OR_GREATER
                    new DateOnly(2020, 7, 23),
                    new DateOnly(2021, 7, 22),
#else
                    new DateTime(2020, 7, 23),
                    new DateTime(2021, 7, 22),
#endif
                },
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "スポーツの日",
                Month = 7,
                Day = 24,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2020, 7, 24),
                EndDate = new DateOnly(2020, 7, 24),
#else
                StartDate = new DateTime(2020, 7, 24),
                EndDate = new DateTime(2020, 7, 24),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "スポーツの日",
                Month = 7,
                Day = 23,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2021, 7, 23),
                EndDate = new DateOnly(2021, 7, 23),
#else
                StartDate = new DateTime(2021, 7, 23),
                EndDate = new DateTime(2021, 7, 23),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "山の日",
                Month = 8,
                Day = 11,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2016, 1, 1),
#else
                StartDate = new DateTime(2016, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = new[]
                {
#if NET6_0_OR_GREATER
                    new DateOnly(2020, 8, 10), new DateOnly(2021, 8, 8),
#else
                    new DateTime(2020, 8, 10),
                    new DateTime(2021, 8, 8),
#endif
                },
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "敬老の日",
                Month = 9,
                Day = 15,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1966, 1, 1),
                EndDate = new DateOnly(2002, 12, 31),
#else
                StartDate = new DateTime(1966, 1, 1),
                EndDate = new DateTime(2002, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = false,
                IsFixedWeek = true,
                TransferPeriod = 1,
                Name = "敬老の日",
                Month = 9,
                Day = -1,
                Week = 3,
                DayOfWeek = 1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2003, 1, 1),
#else
                StartDate = new DateTime(2003, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "体育の日",
                Month = 10,
                Day = 10,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1966, 1, 1),
                EndDate = new DateOnly(1999, 12, 31),
#else
                StartDate = new DateTime(1966, 1, 1),
                EndDate = new DateTime(1999, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = false,
                IsFixedWeek = true,
                TransferPeriod = 1,
                Name = "体育の日",
                Month = 10,
                Day = -1,
                Week = 2,
                DayOfWeek = 1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2000, 1, 1),
                EndDate = new DateOnly(2019, 12, 31),
#else
                StartDate = new DateTime(2000, 1, 1),
                EndDate = new DateTime(2019, 12, 31),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
            {
                IsFixedDay = false,
                IsFixedWeek = true,
                TransferPeriod = 1,
                Name = "スポーツの日",
                Month = 10,
                Day = -1,
                Week = 2,
                DayOfWeek = 1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2020, 1, 1),
#else
                StartDate = new DateTime(2020, 1, 1),
#endif
                EndDate = null,
                ExceptionDate = new[]
                {
#if NET6_0_OR_GREATER
                    new DateOnly(2020, 7, 24),
                    new DateOnly(2021, 7, 23),
#else
                    new DateTime(2020, 7, 24),
                    new DateTime(2021, 7, 23),
#endif
                },
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "即位礼正殿の儀",
                Month = 10,
                Day = 22,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(2019, 10, 22),
                EndDate = new DateOnly(2019, 10, 22),
#else
                StartDate = new DateTime(2019, 10, 22),
                EndDate = new DateTime(2019, 10, 22),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
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
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "即位礼正殿の儀",
                Month = 11,
                Day = 12,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1990, 11, 12),
                EndDate = new DateOnly(1990, 11, 12),
#else
                StartDate = new DateTime(1990, 11, 12),
                EndDate = new DateTime(1990, 11, 12),
#endif
                ExceptionDate = null,
            },
            new NationalHoliday
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
            },
            new NationalHoliday
            {
                IsFixedDay = true,
                IsFixedWeek = false,
                TransferPeriod = 1,
                Name = "天皇誕生日",
                Month = 12,
                Day = 23,
                Week = -1,
                DayOfWeek = -1,
#if NET6_0_OR_GREATER
                StartDate = new DateOnly(1989, 1, 1),
                EndDate = new DateOnly(2018, 12, 31),
#else
                StartDate = new DateTime(1989, 1, 1),
                EndDate = new DateTime(2018, 12, 31),
#endif
                ExceptionDate = null,
            },
        };
    }
}
