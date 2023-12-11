using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EmbeddedMauiApps.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
{
    public static readonly Lazy<MauiApp> MauiApp = new(() =>
    {
        var mauiApp = MauiProgram.CreateMauiApp(builder =>
        {
            builder.UseMauiEmbedding();
        });

        return mauiApp;
    });

    public static bool UseWindowContext = true;

    private MyMauiContent? mauiView;

    public MainWindow()
    {
        InitializeComponent();

        // StackPanel
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
            Spacing = 8,
            Padding = new Microsoft.UI.Xaml.Thickness(20)
        };
        RootLayout.Children.Add(stackPanel);

        // Create WinUI button
        var firstButton = new Microsoft.UI.Xaml.Controls.Button();
        firstButton.Content = "WinUI Button Above MAUI";
        stackPanel.Children.Add(firstButton);

        // Create .NET MAUI view

        // 1. Ensure app is built before creating MAUI views
        var mauiApp = MainWindow.MauiApp.Value;
        // 2. Create MAUI views
        mauiView = new MyMauiContent();
        // 3. Create MAUI context
        var mauiContext = UseWindowContext
            ? mauiApp.CreateEmbeddedWindowContext(this) // 3a. Create window context
            : new MauiContext(mauiApp.Services);        // 3b. Or, create app context
        // 4. Create platform view
        var nativeView = mauiView.ToPlatformEmbedded(mauiContext);
        // 5. Continue
        stackPanel.Children.Add(nativeView);

        // Create WinUI button
        var secondButton = new Microsoft.UI.Xaml.Controls.Button();
        secondButton.Content = "WinUI Button Below MAUI";
        stackPanel.Children.Add(secondButton);

        // Create WinUI button
        var thirdButton = new Microsoft.UI.Xaml.Controls.Button();
        thirdButton.Content = "WinUI Button Magic";
        thirdButton.Click += OnMagicClicked;
        stackPanel.Children.Add(thirdButton);
    }

    private async void OnMagicClicked(object? sender, RoutedEventArgs e)
    {
        if (mauiView?.DotNetBot is not Microsoft.Maui.Controls.Image bot)
            return;

        await bot.RotateTo(360, 1000);
        bot.Rotation = 0;

        bot.HeightRequest = 90;
    }

    private void OnNewWindowClicked(object sender, RoutedEventArgs e)
    {
        var window = new MainWindow();
        window.Activate();
    }
}
