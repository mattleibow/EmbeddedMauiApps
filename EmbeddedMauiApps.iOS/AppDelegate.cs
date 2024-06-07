namespace EmbeddedMauiApps.iOS;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {

	public override UIWindow? Window {
		get;
		set;
	}

    // Maui App here

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions) => true;

    public override UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options) =>
        Enumerable.FirstOrDefault<NSUserActivity>(options.UserActivities)?.ActivityType == "NewTaskWindow"
            ? new UISceneConfiguration("New Task Configuration", UIWindowSceneSessionRole.Application)
            : new UISceneConfiguration("Default Configuration", UIWindowSceneSessionRole.Application);

    //public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    //{
    //    // create a new window instance based on the screen size
    //    Window = new UIWindow(UIScreen.MainScreen.Bounds);

    //    // create a UIViewController with a single UILabel
    //    var vc = new UIViewController();
    //    vc.View!.AddSubview(new UILabel(Window!.Frame)
    //    {
    //        BackgroundColor = UIColor.SystemBackground,
    //        TextAlignment = UITextAlignment.Center,
    //        Text = "Hello, iOS!",
    //        AutoresizingMask = UIViewAutoresizing.All,
    //    });
    //    Window.RootViewController = vc;

    //    // make the window visible
    //    Window.MakeKeyAndVisible();

    //    return true;
    //}
}