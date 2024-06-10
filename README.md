# Embedded .NET MAUI Apps

A set of sample native applications with .NET MAUI embedded.

Projects:

* `EmbeddedMauiApps` - all the .NET MAUI code for your app
* `EmbeddedMauiApps.<platform>` - all the native app projects for your app
* `Microsoft.Maui.Controls.Embedding` - temporary workaround code for .NET MAUI _(see note 1)_
* `TestHarnessApp` - a test app that can be used to test .NET MAUI code without having to launch the native apps _(see note 2)_

Notes:

1. Currently, the .NET MAUI framework does not correctly handle embedded scenarios. This is
   a bug and will be fixed once this issue is closed: https://github.com/dotnet/maui/issues/1718
2. Currently, the Visual Studio tools to not support XAML hot reload and various Live inspections
   when the app is NOT a .NET MAUI app.
