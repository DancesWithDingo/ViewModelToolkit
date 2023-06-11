﻿using Microsoft.Extensions.Logging;

namespace ViewModelToolkitSample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .IocConfiguration()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.ConfigureMauiHandlers(handlers => {
            handlers.AddHandler(typeof(ContentPage), typeof(ViewModelToolkit.ModalPageHandler));
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

