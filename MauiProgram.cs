using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace PythoPlus
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("consolas.ttf", "Consolas");
                    fonts.AddFont("arialmt.ttf", "Arial");
                    fonts.AddFont("arial_bolditalicmt.ttf", "Arial Bold Italic");
                    fonts.AddFont("timesnewromanpsmt.ttf", "Times New Roman");
                    fonts.AddFont("timesnewromanps_italicmt.ttf", "Times New Roman Bold Italic");
                    fonts.AddFont("molot.otf", "Molot");
                });

            builder.Services.AddSingleton<IFontService, FontService>();


            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("PythoPlus.appConnSettings.json");


            builder.Services.AddSingleton<MongoDbService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
