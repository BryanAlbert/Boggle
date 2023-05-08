namespace Boggle.Views;

public partial class NotesPage : ContentPage
{
	public NotesPage()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		notesCollection.SelectedItem = null;
	}
}
