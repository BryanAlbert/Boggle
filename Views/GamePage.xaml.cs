using Boggle.ViewModels;

namespace Boggle.Views;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();
		List<InputView> entries =
		[
			Entry00, Entry10, Entry20, Entry30, Entry40, Entry50,
			Entry01, Entry11, Entry21, Entry31, Entry41, Entry51,
			Entry02, Entry12, Entry22, Entry32, Entry42, Entry52,
			Entry03, Entry13, Entry23, Entry33, Entry43, Entry53,
			Entry04, Entry14, Entry24, Entry34, Entry44, Entry54,
			Entry05, Entry15, Entry25, Entry35, Entry45, Entry55
		];

		((GameViewModel) BindingContext).LetterEntries = [.. entries];
		((GameViewModel) BindingContext).LettersEntry = LettersEntry;
		m_previous = string.Empty;
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

	private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
	{
#if ANDROID
		// TODO: apparently the two-way binding or perhaps interactions between two interconnected two-way bindings 
		// breaks the cursor position--it always returns to position 0. When this gets fixed, remove this hack.
		if (e.OldTextValue == null)
			return;

		int index = 0;
		for (; index < e.OldTextValue.Length && index < e.NewTextValue.Length &&
			e.OldTextValue[index] == e.NewTextValue[index]; index++) ;

		if (e.OldTextValue.Length <= e.NewTextValue.Length)
			index++;

		if (index != ((Entry) sender).CursorPosition)
		{
			((Entry) sender).CursorPosition = index;
		}
#endif
	}
}
