namespace Boggle.Views;

public partial class WordsPage : ContentPage
{
	public WordsPage()
	{
		InitializeComponent();
	}


	// TODO: remove the HeightRequest and this code once this bug is addressed: https://github.com/dotnet/maui/issues/8888
	private async void VerticalStackLayoutPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "Height" && WordList.IsVisible)
		{
			// TODO: remove Delay once this bug is fixed: https://github.com/dotnet/maui/issues/11789
			await Task.Delay(1);
			VerticalStackLayout verticalStackLayout = (VerticalStackLayout) sender;
			Grid parent = (Grid) verticalStackLayout.Parent;
			WordList.HeightRequest = parent.Height - verticalStackLayout.Height -
				((Grid) WordList.Parent).Children.Where(x => x is Label).Max(x => ((Label) x).Height) - 15;
		}
	}
}
