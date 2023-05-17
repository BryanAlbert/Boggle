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

	protected override Window CreateWindow(IActivationState activationState)
	{
		Window window = base.CreateWindow(activationState);

#if true
		window.Activated += async (sender, e) =>
		{
			if (!m_initialized)
			{
				m_initialized = true;
				Window window = sender as Window;
				window.Width = c_defaultWidth;
				window.Height = c_defaultHeight;

				// give it some time to complete window resizing task.
				await window.Dispatcher.DispatchAsync(() => { });

				DisplayInfo disp = DeviceDisplay.Current.MainDisplayInfo;
				window.X = (disp.Width / disp.Density - window.Width) / 2;
				window.Y = (disp.Height / disp.Density - window.Height) / 2;
			}
		};
#endif

		return window;
	}


	private bool m_initialized;
	private const int c_defaultWidth = 550;
	private const int c_defaultHeight = 700;
}
