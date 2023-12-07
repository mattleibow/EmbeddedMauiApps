﻿using Microsoft.Maui.Handlers;
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

internal class EmbeddedWindowHandler : ElementHandler<IWindow, PlatformWindow>, IWindowHandler
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

    protected override PlatformWindow CreatePlatformElement() =>
        MauiContext!.Services.GetRequiredService<PlatformWindow>() ??
        throw new InvalidOperationException("EmbeddedWindowHandler could not locate a platform window.");
}