namespace Boggle.ViewModels
{
	internal class AboutViewModel
	{
#pragma warning disable CA1822 // Mark members as static (TODO: binding error if we mark it static)
		public string Title => AppInfo.Name;
		public string Version => AppInfo.VersionString;
#pragma warning restore CA1822 // Mark members as static
	}
}
