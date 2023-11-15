using System.Text.Json.Serialization;

namespace JpCalendar.WebAPI.Dto.JsonSerializerContexts;

[JsonSerializable(typeof(NationalHolidayDto))]
internal partial class NationalHolidayJsonSerializerContext : JsonSerializerContext
{
    // Empty
}

[JsonSerializable(typeof(NationalHolidayDto[]))]
internal partial class NationalHolidaysJsonSerializerContext : JsonSerializerContext
{
    // Empty
}
