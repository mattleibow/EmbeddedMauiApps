using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;

namespace EmbeddedMauiApps.MacCatalyst;

public class MainViewController : UIViewController
{
    public static readonly Lazy<MauiApp> MauiApp = new(() =>
    {
        var mauiApp = MauiProgram.CreateMauiApp(builder =>
        {
            builder.UseMauiEmbedding();
        });

        return mauiApp;
    });

    private MyMauiContent? mauiView;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Main View Controller";

        View!.BackgroundColor = UIColor.SystemBackground;

        // StackView
        var stackView = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Alignment = UIStackViewAlignment.Fill,
            Distribution = UIStackViewDistribution.Fill,
            Spacing = 8,
            TranslatesAutoresizingMaskIntoConstraints = false,
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
        var firstButton = new UIButton(UIButtonType.System);
        firstButton.SetTitle("UIKit Button Above MAUI", UIControlState.Normal);
        stackView.AddArrangedSubview(firstButton);

        // Create .NET MAUI view
        var mauiApp = MainViewController.MauiApp.Value;
        mauiView = new MyMauiContent();
        var nativeView = CreateNativeView(mauiApp, mauiView);
        stackView.AddArrangedSubview(nativeView);

        // Create UIKit button
        var secondButton = new UIButton(UIButtonType.System);
        secondButton.SetTitle("UIKit Button Below MAUI", UIControlState.Normal);
        stackView.AddArrangedSubview(secondButton);

        // Create UIKit button
        var thirdButton = new UIButton(UIButtonType.System);
        thirdButton.SetTitle("UIKit Button Magic", UIControlState.Normal);
        thirdButton.TouchUpInside += OnMagicClicked;
        stackView.AddArrangedSubview(thirdButton);

        AddNavBarButtons();
    }

    private UIView CreateNativeView(MauiApp mauiApp, VisualElement mauiView)
    {
        var mauiWindow = new Window();
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

    private async void OnMagicClicked(object? sender, EventArgs e)
    {
        if (mauiView?.DotNetBot is not Image bot)
            return;

        await bot.RotateTo(360, 1000);
        bot.Rotation = 0;

        bot.HeightRequest = 90;
    }
}
