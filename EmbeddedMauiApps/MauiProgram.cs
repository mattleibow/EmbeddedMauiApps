using Microsoft.Extensions.Logging;

namespace EmbeddedMauiApps;

public static class MauiProgram
{
	/// <summary>
	/// Creates a new MauiApp instance using the default application.
	/// </summary>
	/// <param name="additional">An optional action to further configure the MauiAppBuilder.</param>
	/// <returns>A new MauiApp instance.</returns>
	public static MauiApp CreateMauiApp(Action<MauiAppBuilder>? additional = null) =>
		CreateMauiApp<App>(additional);

	/// <summary>
	/// Creates a new MauiApp instance using the specified application.
	/// </summary>
	/// <typeparam name="TApp">The application type.</typeparam>
	/// <param name="additional">An optional action to further configure the MauiAppBuilder.</param>
	/// <returns>A new MauiApp instance.</returns>
	public static MauiApp CreateMauiApp<TApp>(Action<MauiAppBuilder>? additional = null)
		where TApp : App
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<TApp>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		additional?.Invoke(builder);

		return builder.Build();
	}
}
