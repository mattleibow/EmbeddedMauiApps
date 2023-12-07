using Microsoft.Maui.Platform;

namespace EmbeddedMauiApps.MacCatalyst;

public class MainViewController : UIViewController
{
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        ConfigureViewController();
    }

    private void ConfigureViewController()
    {
        Title = "Main View Controller";

        View!.BackgroundColor = UIColor.SystemBackground;

        // StackView
        var stackView = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Alignment = UIStackViewAlignment.Fill,
            Distribution = UIStackViewDistribution.Fill,
            Spacing = 8,
            TranslatesAutoresizingMaskIntoConstraints = false
        };
        View.AddSubview(stackView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            stackView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 20),
            stackView.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor, 20),
            stackView.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor, -20),
            stackView.BottomAnchor.ConstraintLessThanOrEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20)
        });

        // Create UIKit button
        var createTaskButton = new UIButton(UIButtonType.System);
        createTaskButton.SetTitle("UIKit Button", UIControlState.Normal);
        stackView.AddArrangedSubview(createTaskButton);

        // Create .NET MAUI view
        var mauiApp = CreateMauiApp();
        var mauiView = CreateMauiView(mauiApp);
        var nativeView = CreateNativeView(mauiApp, mauiView);
        stackView.AddArrangedSubview(nativeView);

        AddNavBarButtons();
    }

    private MauiApp CreateMauiApp()
    {
        var mauiApp = MauiProgram.CreateMauiApp(builder =>
        {
            builder.UseMauiEmbedding(UIApplication.SharedApplication.Delegate);

            //builder.Services.AddSingleton(typeof(UIWindow), (services) => Window);
        });

        return mauiApp;
    }

    private VisualElement CreateMauiView(MauiApp mauiApp)
    {
        var view = new MyMauiContent();
        return view;
    }

    private UIView CreateNativeView(MauiApp mauiApp, VisualElement mauiView)
    {
        var mauiWindow = new Window();
        //mauiApp.Services.GetRequiredService<IApplication>()
        mauiWindow.AddLogicalChild(mauiView);

        var window =
            View?.Window ??
            ParentViewController?.View?.Window ??
            mauiApp.Services.GetRequiredService<IUIApplicationDelegate>().GetWindow();

        var mauiContext = mauiApp.CreateWindowScope(window, mauiWindow);

        var platformView = mauiView.ToPlatform(mauiContext);

        return platformView;
    }

    private void AddNavBarButtons()
    {
        var addNewWindowButton = new UIBarButtonItem(
            UIImage.GetSystemImage("macwindow.badge.plus"),
            UIBarButtonItemStyle.Plain,
            (sender, e) => RequestSession());

        var addNewTaskButton = new UIBarButtonItem(
            UIBarButtonSystemItem.Add,
            (sender, e) => RequestSession("NewTaskWindow"));

        NavigationItem.RightBarButtonItems = new [] { addNewTaskButton, addNewWindowButton };
    }

    private void RequestSession(string? activityType = null)
    {
        var activity = activityType is null
            ? null
            : new NSUserActivity(activityType);

        if (OperatingSystem.IsMacCatalystVersionAtLeast(17))
        {
            var request = UISceneSessionActivationRequest.Create();
            request.UserActivity = activity;

            UIApplication.SharedApplication.ActivateSceneSession(request, error =>
            {
                Console.WriteLine(new NSErrorException(error));
            });
        }
        else
        {
            UIApplication.SharedApplication.RequestSceneSessionActivation(null, activity, null, error =>
            {
                Console.WriteLine(new NSErrorException(error));
            });
        }
    }
}
