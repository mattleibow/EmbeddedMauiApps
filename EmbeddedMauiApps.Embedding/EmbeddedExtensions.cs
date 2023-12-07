using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;

#if ANDROID
using PlatformWindow = Android.App.Activity;
using PlatformApplication = Android.App.Application;
#elif IOS || MACCATALYST
using PlatformWindow = UIKit.UIWindow;
using PlatformApplication = UIKit.IUIApplicationDelegate;
#elif WINDOWS
#endif

namespace Microsoft.Maui.Controls;

public static class EmbeddedExtensions
{
    public static MauiAppBuilder UseMauiEmbedding(this MauiAppBuilder builder, PlatformApplication platformApplication)
    {
        builder.Services.AddSingleton(platformApplication);

        builder.Services.AddSingleton<EmbeddedPlatformApplication>();

        builder.Services.AddScoped<EmbeddedWindowProvider>();

#if IOS || MACCATALYST
        builder.Services.AddScoped<PlatformWindow>(svc =>
            svc.GetRequiredService<EmbeddedWindowProvider>().PlatformWindow ?? throw new InvalidOperationException("EmbeddedWindowProvider did not have a platform window."));
#endif

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMauiInitializeService, EmbeddedInitializeService>());

        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID || IOS || MACCATALYST
            handlers.AddHandler(typeof(Window), typeof(EmbeddedWindowHandler));
#endif
        });

        return builder;
    }

    public static IMauiContext CreateWindowScope(this MauiApp mauiApp, PlatformWindow platformWindow, Window window)
    {
        var windowScope = mauiApp.Services.CreateScope();

        var windowContext = new MauiContext(windowScope.ServiceProvider);

        var wndProvider = windowContext.Services.GetRequiredService<EmbeddedWindowProvider>();
        wndProvider.SetWindow(platformWindow, window);

        window.ToHandler(windowContext);

        return windowContext;
    }

    private class EmbeddedInitializeService : IMauiInitializeService
    {
        public void Initialize(IServiceProvider services) =>
            services.GetRequiredService<EmbeddedPlatformApplication>();
    }
}
