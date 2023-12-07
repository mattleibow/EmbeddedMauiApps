namespace EmbeddedMauiApps.MacCatalyst;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    // Maui App here

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions) => true;

    public override UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options) =>
        Enumerable.FirstOrDefault<NSUserActivity>(options.UserActivities)?.ActivityType == "NewTaskWindow"
            ? new UISceneConfiguration("New Task Configuration", UIWindowSceneSessionRole.Application)
            : new UISceneConfiguration("Default Configuration", UIWindowSceneSessionRole.Application);

    public static readonly Lazy<MauiApp> MauiApp = new(() =>
    {
        var mauiApp = MauiProgram.CreateMauiApp(builder =>
        {
            builder.UseMauiEmbedding(UIApplication.SharedApplication.Delegate);
        });

        return mauiApp;
    });
}
