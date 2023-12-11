using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Platform;

#if ANDROID
using PlatformWindow = Android.App.Activity;
using PlatformApplication = Android.App.Application;
#elif IOS || MACCATALYST
using PlatformWindow = UIKit.UIWindow;
using PlatformApplication = UIKit.IUIApplicationDelegate;
#elif WINDOWS
using PlatformWindow = Microsoft.UI.Xaml.Window;
using PlatformApplication = Microsoft.UI.Xaml.Application;
#endif

namespace Microsoft.Maui.Controls;

public static class EmbeddedExtensions
{
    public static MauiAppBuilder UseMauiEmbedding(this MauiAppBuilder builder, PlatformApplication? platformApplication = null)
    {
#if ANDROID
        platformApplication ??= (Android.App.Application)Android.App.Application.Context;
#elif IOS || MACCATALYST
        platformApplication ??= UIKit.UIApplication.SharedApplication.Delegate;
#elif WINDOWS
        platformApplication ??= Microsoft.UI.Xaml.Application.Current;
#endif

        builder.Services.AddSingleton(platformApplication);

        builder.Services.AddSingleton<EmbeddedPlatformApplication>();

        builder.Services.AddScoped<EmbeddedWindowProvider>();

        // returning null is acceptable here as the platform window is optional - but we do not know for sure until we resolve it
        builder.Services.AddScoped<PlatformWindow>(svc => svc.GetRequiredService<EmbeddedWindowProvider>().PlatformWindow!);

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMauiInitializeService, EmbeddedInitializeService>());

        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(Window), typeof(EmbeddedWindowHandler));
        });

        return builder;
    }

    public static IMauiContext CreateWindowScope(this MauiApp mauiApp, PlatformWindow platformWindow, Window window)
    {
        var windowScope = mauiApp.Services.CreateScope();

#if ANDROID
        var windowContext = new MauiContext(windowScope.ServiceProvider, platformWindow);
#else
        var windowContext = new MauiContext(windowScope.ServiceProvider);
#endif

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
