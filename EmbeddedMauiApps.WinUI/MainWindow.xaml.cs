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
            builder.UseMauiEmbedding(Microsoft.UI.Xaml.Application.Current);
        });

        return mauiApp;
    });

    private MyMauiContent? mauiView;

    public MainWindow()
    {
        InitializeComponent();

        // StackPanel
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
            Spacing = 8,
            Padding = new Microsoft.UI.Xaml.Thickness(20)
        };
        Content = stackPanel;

        // Create WinUI button
        var firstButton = new Microsoft.UI.Xaml.Controls.Button();
        firstButton.Content = "WinUI Button Above MAUI";
        stackPanel.Children.Add(firstButton);

        // Create .NET MAUI view
        var mauiApp = MainWindow.MauiApp.Value;
        var mauiView = CreateMauiView();
        var nativeView = CreateNativeView(mauiApp, mauiView);
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

    private VisualElement CreateMauiView()
    {
        mauiView = new MyMauiContent();
        return mauiView;
    }

    private FrameworkElement CreateNativeView(MauiApp mauiApp, VisualElement mauiView)
    {
        var mauiWindow = new Microsoft.Maui.Controls.Window();
        mauiWindow.AddLogicalChild(mauiView);

        var mauiContext = mauiApp.CreateWindowScope(this, mauiWindow);

        var platformView = mauiView.ToPlatform(mauiContext);

        return platformView;
    }

    private async void OnMagicClicked(object? sender, RoutedEventArgs e)
    {
        if (mauiView?.DotNetBot is not Microsoft.Maui.Controls.Image bot)
            return;

        await bot.RotateTo(360, 1000);
        bot.Rotation = 0;

        bot.HeightRequest = 90;
    }
}
