namespace Boggle.Views;

public partial class WordsPage : ContentPage
{
	public WordsPage()
	{
		InitializeComponent();
	}


	/// <summary>
	/// Since Microsoft doesn't care about Windows, MAUI doens't execute the RemainingItemsThresholdReachedCommand
	/// when the view is scrolled to the end. Since debugging on Android is so abysmal, we have to make hacks like this
	/// to get things working. Sadly, these events never seem to get fired, not even on Android.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
	{
		if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
			return;

		if (sender is CollectionView view && view is IElementController controller)
		{
			int count = controller.LogicalChildren.Count;
			if (e.LastVisibleItemIndex - count + 1 + view.RemainingItemsThreshold >= 0 &&
				view.RemainingItemsThresholdReachedCommand.CanExecute(null))
					view.RemainingItemsThresholdReachedCommand.Execute(null);
		}
    }

	private void OnCollectionViewScrollToRequested(object sender, ScrollToRequestEventArgs e)
	{

	}
}
