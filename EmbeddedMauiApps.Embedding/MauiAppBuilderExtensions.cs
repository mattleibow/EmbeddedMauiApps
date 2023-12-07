using Microsoft.Extensions.DependencyInjection.Extensions;

#if ANDROID
using PlatformApplication = Android.App.Application;
#elif IOS || MACCATALYST
using PlatformWindow = UIKit.UIWindow;
using PlatformApplication = UIKit.IUIApplicationDelegate;
#elif WINDOWS
#endif

namespace Microsoft.Maui.Controls;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMauiEmbedding(this MauiAppBuilder builder, PlatformApplication platformApplication)
    {
        builder.Services.AddSingleton(platformApplication);

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMauiInitializeService, EmbeddedInitializeService>());

        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID || IOS || MACCATALYST
            //handlers.AddHandler(typeof(Window), typeof(EmbeddedWindowHandler));
#endif
        });

        return builder;
    }

    private class EmbeddedInitializeService : IMauiInitializeService
    {
        public void Initialize(IServiceProvider services) =>
            EmbeddedPlatformApplication.Create(services);
    }
}
