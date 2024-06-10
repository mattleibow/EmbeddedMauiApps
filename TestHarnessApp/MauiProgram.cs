namespace TestHarnessApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp() =>
		EmbeddedMauiApps.MauiProgram.CreateMauiApp<TestApp>(builder =>
		{
			// TODO: any extra test harness configuration such as service stubs
			//       or mocks can be added here
		});
}
