using Boggle.Models;
using Boggle.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
	/// to get things working. Sadly, these events never seem to get fired. Actually, if the same board is solved twice in a row,
	/// it seems to work... 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
	{
		if (DeviceInfo.Current.Platform != DevicePlatform.WinUI || e.LastVisibleItemIndex < 0)
			return;

		if (sender is CollectionView view && view is IElementController controller &&
			view.RemainingItemsThresholdReachedCommand.CanExecute(null))
		{
#if false
			// reveal the wretched, wrecked bogusness of the controller.LogicalChildren
			for (int index = 0; index < controller.LogicalChildren.Count; index++)
			{
				if (controller.LogicalChildren[index].BindingContext is Solutions solutions)
					Debug.WriteLine($"{index}: Solutions for word length {solutions.WordLength} with count {solutions.Count}");
				else if (controller.LogicalChildren[index].BindingContext is Solution solution)
					Debug.WriteLine($"{index}: Solution {solution.Word}");
			}
#endif
			ObservableCollection<Solutions> solutions = ((WordsViewModel)BindingContext).Solutions;
			List<string> items = new();
			foreach (Solutions header in solutions)
			{
				items.Add($"Header for {header.WordLength}");
				foreach (Solution solution in header)
					items.Add(solution.Word);
			}

			int delta = items.Count - e.LastVisibleItemIndex;
			Debug.WriteLine($"Last word index {e.LastVisibleItemIndex} of {items.Count}: {items[e.LastVisibleItemIndex]}, {delta} from the end," +
				$" {delta - view.RemainingItemsThreshold} from the threshold of {view.RemainingItemsThreshold}");

			if (delta <= view.RemainingItemsThreshold)
				view.RemainingItemsThresholdReachedCommand.Execute(null);
		}
    }


	private void OnCollectionViewScrollToRequested(object sender, ScrollToRequestEventArgs e)
	{

	}
}
