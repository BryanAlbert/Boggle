namespace Boggle.Views;

public partial class NotesPage : ContentPage
{
	public NotesPage()
	{
		InitializeComponent();
	}

	private void ContentPageNavigatedTo(object sender, NavigatedToEventArgs e)
	{
		notesCollection.SelectedItem = null;
	}
}
