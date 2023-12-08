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
            builder.UseMauiEmbedding((Android.App.Application)Android.App.Application.Context);
        });

        return mauiApp;
    });

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
        var mauiApp = MainActivity.MauiApp.Value;
        var mauiView = CreateMauiView();
        var nativeView = CreateNativeView(mauiApp, mauiView);
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

    private VisualElement CreateMauiView()
    {
        mauiView = new MyMauiContent();
        return mauiView;
    }

    private Android.Views.View CreateNativeView(MauiApp mauiApp, VisualElement mauiView)
    {
        var mauiWindow = new Window();
        mauiWindow.AddLogicalChild(mauiView);

        var mauiContext = mauiApp.CreateWindowScope(this, mauiWindow);

        var platformView = mauiView.ToPlatform(mauiContext);

        return platformView;
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
