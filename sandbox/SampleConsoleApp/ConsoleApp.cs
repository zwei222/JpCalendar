using JpCalendar;

namespace SampleConsoleApp;

public class ConsoleApp
{
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
        return $"{Calendar.GetEra(date).Name}{date.ToString(format, Calendar.JapaneseCultureInfo)}";
    }

    private void ShowJapaneseNationalHoliday(DateTime date)
    {
        if (Calendar.IsNationalHoliday(date) is false)
        {
            Console.WriteLine($"{date:yyyy-MM-dd} is not a japanese national holiday.");
            return;
        }

        var name = Calendar.GetNationalHolidayName(date);

        Console.WriteLine(name);
    }

    private string GetRokuyo(DateTime date)
    {
        return Calendar.GetRokuyo(date);
    }
}
