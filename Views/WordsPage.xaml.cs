namespace Boggle.Views;

public partial class WordsPage : ContentPage
{
	public WordsPage()
	{
		InitializeComponent();
	}


	// TODO: remove the HeightRequest and this code once this bug is addressed: https://github.com/dotnet/maui/issues/8888
	private async void WordListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "Height")
		{
			// TODO: remove Delay once this bug is fixed: https://github.com/dotnet/maui/issues/11789
			await Task.Delay(1);
			CollectionView view = (CollectionView) sender;
			Grid parent = (Grid) view.Parent;
			VerticalStackLayout grandParent = (VerticalStackLayout) parent.Parent;
			double height = grandParent.Height;
			height -= grandParent.Children.Where(x => x is Label && ((Label) x).IsVisible).Sum(x => ((Label) x).Height);
			height -= grandParent.Children.First(x => x is Frame).Height;
			height -= parent.Children.Where(x => x is Label && ((Label) x).IsVisible).Max(x => ((Label) x).Height);
			view.HeightRequest = height;
		}
	}
}
