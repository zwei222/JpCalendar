using JpCalendar.Services;

namespace SampleConsoleApp;

public class ConsoleApp
{
    private readonly IJpCalendarService jpCalendarService;

    public ConsoleApp(IJpCalendarService jpCalendarService)
    {
        this.jpCalendarService = jpCalendarService;
    }

    public void ShowJapaneseCalendar()
    {
        DateTime date;

        while (true)
        {
            Console.Write("Please enter a date : ");

            var input = Console.ReadLine();

            if (DateTime.TryParse(input, out date))
            {
                break;
            }

            Console.WriteLine("Incorrect date format.");
        }

        var japaneseStringDate = this.ToJapaneseString(date, "yy年MM月dd日");
        var rokuyo = this.GetRokuyo(date);

        Console.WriteLine($"{japaneseStringDate} ({rokuyo})");
        this.ShowJapaneseNationalHoliday(date);
    }

    private string ToJapaneseString(DateTime date, string format)
    {
        return $"{this.jpCalendarService.GetEra(date).Name}{date.ToString(format, this.jpCalendarService.JapaneseCultureInfo)}";
    }

    private void ShowJapaneseNationalHoliday(DateTime date)
    {
        if (this.jpCalendarService.IsNationalHoliday(date) is false)
        {
            Console.WriteLine($"{date:yyyy-MM-dd} is not a japanese national holiday.");
            return;
        }

        var name = this.jpCalendarService.GetNationalHolidayName(date);

        Console.WriteLine(name);
    }

    private string GetRokuyo(DateTime date)
    {
        return this.jpCalendarService.GetRokuyo(date);
    }
}
