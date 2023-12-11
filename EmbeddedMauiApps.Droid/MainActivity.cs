using Android.Widget;
using Microsoft.Maui.Platform;

using static Android.Views.ViewGroup.LayoutParams;

namespace EmbeddedMauiApps.Droid;

[Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
public class MainActivity : Activity
{
    public static readonly Lazy<MauiApp> MauiApp = new(() =>
    {
        var mauiApp = MauiProgram.CreateMauiApp(builder =>
        {
            builder.UseMauiEmbedding();
        });

        return mauiApp;
    });

    public static bool UseWindowContext = false;

    private MyMauiContent? mauiView;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        var rootLayout = FindViewById<LinearLayout>(Resource.Id.rootLayout)!;

        // Create Android button
        var firstButton = new Android.Widget.Button(this);
        firstButton.Text = "Android Button Above MAUI";
        rootLayout.AddView(firstButton, new LinearLayout.LayoutParams(MatchParent, WrapContent));

        // Create .NET MAUI view

        // 1. Ensure app is built before creating MAUI views
        var mauiApp = MainActivity.MauiApp.Value;
        // 2. Create MAUI views
        mauiView = new MyMauiContent();
        // 3. Create MAUI context
        var mauiContext = UseWindowContext
            ? mauiApp.CreateEmbeddedWindowContext(this) // 3a. Create window context
            : new MauiContext(mauiApp.Services);        // 3b. Or, create app context
        // 4. Create platform view
        var nativeView = mauiView.ToPlatformEmbedded(mauiContext);
        // 5. Continue
        rootLayout.AddView(nativeView, new LinearLayout.LayoutParams(MatchParent, WrapContent));

        // Create Android button
        var secondButton = new Android.Widget.Button(this);
        secondButton.Text = "Android Button Below MAUI";
        rootLayout.AddView(secondButton, new LinearLayout.LayoutParams(MatchParent, WrapContent));

        // Create Android button
        var lastButton = new Android.Widget.Button(this);
        lastButton.Text = "Android Button Magic";
        lastButton.Click += OnMagicClicked;
        rootLayout.AddView(lastButton, new LinearLayout.LayoutParams(MatchParent, WrapContent));
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
