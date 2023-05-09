namespace Boggle.ViewModels
{
	internal class AboutViewModel
	{
		public string Title => AppInfo.Name;
		public string Version => AppInfo.VersionString;
		public string Instructions => "Choose a Boggle game to play from the Games page. Generate a" +
			" board on the Game page and solve it on the Solve page.";
	}
}
