using Boggle.ViewModels;

namespace Boggle.Views;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();
		List<InputView> entries = new()
		{
			Entry00, Entry10, Entry20, Entry30, Entry40, Entry50,
			Entry01, Entry11, Entry21, Entry31, Entry41, Entry51,
			Entry02, Entry12, Entry22, Entry32, Entry42, Entry52,
			Entry03, Entry13, Entry23, Entry33, Entry43, Entry53,
			Entry04, Entry14, Entry24, Entry34, Entry44, Entry54,
			Entry05, Entry15, Entry25, Entry35, Entry45, Entry55
		};

		((GameViewModel) BindingContext).LetterEntries = entries.ToArray();
		((GameViewModel) BindingContext).LettersEntry = LettersEntry;
	}


	private void OnEntryFocused(object sender, FocusEventArgs e)
	{
		InputView[] entries = ((GameViewModel) BindingContext).LetterEntries;
		int index = Array.IndexOf(entries, (Entry) sender);
		int start = index;
		while (index > 0 && (entries[index - 1].Text.Length == 0 || entries[index - 1].Text == "$"))
			index--;

		while (entries[index].Text == "$")
			index++;

		if (start != index)
			_ = entries[index].Focus();

		m_previous = ((Entry) sender).Text;
		((Entry) sender).Text = string.Empty;
    }

	private void OnEntryUnfocused(object sender, FocusEventArgs e)
	{
		if (((Entry) sender).Text.Length == 0)
			((Entry) sender).Text = m_previous;
	}


	private string m_previous;
}