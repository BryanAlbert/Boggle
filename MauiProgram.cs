using CommunityToolkit.Maui;

namespace Boggle;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		_ = builder.UseMauiApp<App>().ConfigureFonts(fonts =>
		{
			_ = fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			_ = fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		}).UseMauiCommunityToolkit();

		return builder.Build();
	}
}
