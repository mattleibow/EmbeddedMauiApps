using Microsoft.Maui.Handlers;
using Microsoft.Extensions.DependencyInjection.Extensions;

#if ANDROID
using PlatformWindow = Android.App.Activity;
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

public class EmbeddedPlatformApplication : IPlatformApplication
{
    private readonly MauiContext rootContext;
    private readonly IMauiContext applicationContext;

    private EmbeddedPlatformApplication(IServiceProvider services)
    {
        IPlatformApplication.Current = this;

#if ANDROID
        var platformApp = services.GetRequiredService<PlatformApplication>();
        rootContext = new MauiContext(services, platformApp);
#else
        rootContext = new MauiContext(services);
#endif

        applicationContext = MakeApplicationScope(rootContext);

        Services = applicationContext.Services;

        Application = Services.GetRequiredService<IApplication>();
    }

    public IServiceProvider Services { get; }

    public IApplication Application { get; }

    public static IPlatformApplication Create(IServiceProvider services) =>
        new EmbeddedPlatformApplication(services);

    private static IMauiContext MakeApplicationScope(IMauiContext rootContext)
    {
        var scopedContext = new MauiContext(rootContext.Services);

        InitializeScopedServices(scopedContext);

        return scopedContext;
    }

    private static void InitializeScopedServices(IMauiContext scopedContext)
    {
        var scopedServices = scopedContext.Services.GetServices<IMauiInitializeScopedService>();

        foreach (var service in scopedServices)
            service.Initialize(scopedContext.Services);
    }
}

public class EmbeddedWindowHandler : ElementHandler<IWindow, PlatformWindow>, IWindowHandler
{
    public static IPropertyMapper<IWindow, IWindowHandler> Mapper =
        new PropertyMapper<IWindow, IWindowHandler>(ElementHandler.ElementMapper)
        {
        };

    public static CommandMapper<IWindow, IWindowHandler> CommandMapper =
        new CommandMapper<IWindow, IWindowHandler>(ElementHandler.ElementCommandMapper)
        {
        };

    public EmbeddedWindowHandler()
        : base(Mapper)
    {
    }

    protected override PlatformWindow CreatePlatformElement()
    {
#if ANDROID
        var window = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity!;
#elif IOS || MACCATALYST
        var window = Microsoft.Maui.ApplicationModel.Platform.GetCurrentUIViewController().View.Window;
#elif WINDOWS
// TODO
#endif
        return window;
    }
}
