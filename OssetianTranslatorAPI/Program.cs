using OssetianTranslatorAPI.Configuration;
using OssetianTranslatorAPI.Interfaces;
using OssetianTranslatorAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenAiSettings>(
    builder.Configuration.GetSection(OpenAiSettings.SectionName));

builder.Services.AddHttpClient<IOpenAiTranslationService, OpenAiTranslationService>((sp, client) =>
{
    var settings = builder.Configuration.GetSection(OpenAiSettings.SectionName).Get<OpenAiSettings>()!;
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
});

builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
