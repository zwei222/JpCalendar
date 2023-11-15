using System.Text.Json.Serialization;

namespace JpCalendar.WebAPI.Dto.JsonSerializerContexts;

[JsonSerializable(typeof(EraDto))]
internal partial class EraJsonSerializerContext : JsonSerializerContext
{
    // Empty
}
