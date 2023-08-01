#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

namespace JpCalendar.Internals;

internal sealed class NationalHolidayList
{
    private readonly Dictionary<int, NationalHoliday[]> nationalHolidayDictionary;

    public NationalHolidayList(NationalHoliday[] nationalHolidays)
    {
        this.nationalHolidayDictionary = new Dictionary<int, NationalHoliday[]>();

        foreach (var nationalHolidayGroup in nationalHolidays.GroupBy(item => item.Month))
        {
            this.nationalHolidayDictionary.Add(nationalHolidayGroup.Key, nationalHolidayGroup.ToArray());
        }
    }

    public Span<NationalHoliday> GetNationalHolidays(int month)
    {
#if NET6_0_OR_GREATER
        ref var nationalHolidays = ref CollectionsMarshal.GetValueRefOrNullRef(
            this.nationalHolidayDictionary,
            month);

        if (Unsafe.IsNullRef(ref nationalHolidays) is false)
#else
        if (this.nationalHolidayDictionary.TryGetValue(month, out var nationalHolidays))
#endif
        {
            return nationalHolidays.AsSpan();
        }

        return Span<NationalHoliday>.Empty;
    }
}
