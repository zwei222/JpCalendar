using JpCalendar.WebAPI.Dto.JsonSerializerContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, EraJsonSerializerContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, NationalHolidayJsonSerializerContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, NationalHolidaysJsonSerializerContext.Default);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
