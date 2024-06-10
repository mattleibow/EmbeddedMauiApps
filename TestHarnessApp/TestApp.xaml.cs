namespace TestHarnessApp;

public partial class TestApp : EmbeddedMauiApps.App
{
	public TestApp()
	{
		// Capture the added resources from the base app
		var baseResources = Resources;

		InitializeComponent();

		// Add the base resources to the merged dictionaries
		Resources.MergedDictionaries.Add(baseResources);

		MainPage = new HostPage();
	}
}
