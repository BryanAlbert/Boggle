namespace Boggle;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}


	public const string c_sendGameSelection = "SendGameSelection";


	protected override Window CreateWindow(IActivationState activationState)
	{
		Window window = base.CreateWindow(activationState);

#if WINDOWS
		window.Activated += async (sender, e) =>
		{
			Window window = sender as Window;
			window.Width = c_defaultWidth;
			window.Height = c_defaultHeight;

			// give it some time to complete window resizing task.
			await window.Dispatcher.DispatchAsync(() => { });

			DisplayInfo disp = DeviceDisplay.Current.MainDisplayInfo;
			window.X = (disp.Width / disp.Density - window.Width) / 2;
			window.Y = (disp.Height / disp.Density - window.Height) / 2;
		};
#endif

		return window;
	}


	private const int c_defaultWidth = 550;
	private const int c_defaultHeight = 700;
}
