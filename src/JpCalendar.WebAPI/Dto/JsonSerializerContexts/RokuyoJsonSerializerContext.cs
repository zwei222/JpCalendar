using System.Text.Json.Serialization;

namespace JpCalendar.WebAPI.Dto.JsonSerializerContexts;

[JsonSerializable(typeof(RokuyoDto))]
internal partial class RokuyoJsonSerializerContext : JsonSerializerContext
{
    // Empty
}

[JsonSerializable(typeof(RokuyoDto[]))]
internal partial class RokuyoArrayJsonSerializerContext : JsonSerializerContext
{
    // Empty
}
