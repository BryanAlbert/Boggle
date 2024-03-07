namespace Boggle;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}


	public const string c_isGameSelected = "IsGameSelected";
	public const string c_isBoardGenerated = "IsBoardGenerated";

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Window window = base.CreateWindow(activationState);

#if WINDOWS
		window.Activated += async (sender, e) =>
		{
			if (!m_initialized)
			{
				if (sender is not Window window)
					return;

				window.Width = c_defaultWidth;
				window.Height = c_defaultHeight;
				m_initialized = true;

				// give it some time to complete window resizing task
				await window.Dispatcher.DispatchAsync(() => { });

				DisplayInfo display = DeviceDisplay.Current.MainDisplayInfo;

#if false || !DEBUG
				window.X = (display.Width / display.Density - window.Width) / 2;
				window.Y = (display.Height / display.Density - window.Height) / 2;
#elif true
				// TODO: for debugging, show on the monitor to the left of the main display
				window.X = -(display.Width / display.Density - window.Width) / 2;
				window.Y = (display.Height / display.Density - window.Height) / 2;
#else
				// TODO: for debugging, show on the monitor below the main display
				window.X = (display.Width / display.Density - window.Width) / 2;
				window.Y = 1080;
#endif
			}
		};
#endif

				return window;
	}


#if WINDOWS
	private bool m_initialized;
#endif

	private const int c_defaultWidth = 550;
	private const int c_defaultHeight = 760;
}
