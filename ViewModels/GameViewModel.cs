using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class GameViewModel : ObservableObject
	{
		public GameViewModel()
		{
			ScrambleCommand = new RelayCommand(OnScramble);
			WeakReferenceMessenger.Default.Register<string>(this, OnRequest);
			WeakReferenceMessenger.Default.Register<GameViewModel>(this, OnGameUpdated);
			_ = WeakReferenceMessenger.Default.Send(App.c_isGameSelected);

#if true
			m_seed = null;
#else
			// TODO: testing
			m_seed = 23;
			OnScramble();
#endif
		}

		public GameViewModel(Game game)
		{
			m_game = game;
			Name = game.Name;
			Size = game.Size;
		}


		public string Name { get => m_name; set => SetProperty(ref m_name, value); }
		public int Size { get => m_size; set => SetProperty(ref m_size, value); }
		public string ComboLetters { get => m_comboLetters; set => SetProperty(ref m_comboLetters, value); }
		public string ComboLetterIndices { get; set; }

		public string Letters
		{
			get => m_letters;
			set
			{
				if (value.Length == 2)
					return;

				string filtered = value;
				if (value.Length == 3)
				{
					filtered = m_letters;
					if (int.TryParse(value[0].ToString(), out int x) && int.TryParse(value[1].ToString(), out int y))
					{
						int index = x + y * Size;
						string test = value[2].ToString();
						if (test == " " || test == "$" || test == m_letters[index].ToString() && !char.IsDigit(test[0]))
							return;

						test = test.ToUpper();
						bool valid = ComboLetterIndices.Contains(test);
						if (valid)
							filtered = m_letters[..index] + test + m_letters[(index + 1)..];

						if (valid)
						{
							m_letters = null;
							MoveFocus(value[..2]);
						}
						else
						{
							ClearEntry(value[..2]);
						}
					}
				}

				if (m_seed.HasValue)
					m_letters = string.Empty;

				if (SetProperty(ref m_letters, filtered))
				{
					LettersList = m_letters.TrimEnd();
					IsBoardGenerated = !m_letters.Contains(' ');
					if (IsBoardGenerated)
						_ = WeakReferenceMessenger.Default.Send(this);
				}
			}
		}

		public string LettersList
		{
			get => m_lettersEntry;
			set
			{
				if (value == m_lettersEntry)
					return;

				string filtered = string.Empty;
				foreach (char letter in value.ToUpper())
				{
					if (ComboLetterIndices.Contains(letter))
						filtered += letter;
					else
						m_lettersEntry = null;
				}

				int square = Size * Size;
				if (filtered.Length > square)
				{
					filtered = filtered[..square];
					m_lettersEntry = null;
				}

				if (SetProperty(ref m_lettersEntry, filtered))
				{
					if (filtered.Length < square)
						filtered += new string(' ', square - filtered.Length);

					Letters = filtered;
				}
			}
		}

		public List<int> Scoring => m_game.Scoring.Select(x => int.Parse(x)).ToList();
		public int WordLength => m_game.WordSize;
		public string RenderSize => $"{m_game.Size}x{m_game.Size}";
		public string RenderScoring => string.Join(", ", m_game.Scoring);
		public List<string> Cubes => m_game.Cubes;
		public string Cubes1 => string.Join(", ", m_game.Cubes.Take(m_game.Size));
		public string Cubes2 => string.Join(", ", m_game.Cubes.Skip(m_game.Size).Take(m_game.Size));
		public string Cubes3 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 2).Take(m_game.Size));
		public string Cubes4 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 3).Take(m_game.Size));
		public string Cubes5 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 4).Take(m_game.Size));
		public string Cubes6 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 5).Take(m_game.Size));
		public bool IsGameSelected { get => m_isGameSelected; set => SetProperty(ref m_isGameSelected, value); }
		public bool IsBoardGenerated { get => m_isBoardGenerated; set => SetProperty(ref m_isBoardGenerated, value); }
		public bool Cells5Visible { get => m_cells5Visible; set => SetProperty(ref m_cells5Visible, value); }
		public bool Cells6Visible { get => m_cell6Visible; set => SetProperty(ref m_cell6Visible, value); }
		public InputView LettersEntry { get; set; }
		public InputView[] LetterEntries { get; set; }
		public ICommand ScrambleCommand { get; }


		public int ComputeScore(string word)
		{
			int score = word.Length > Scoring.Count ? Scoring.Last() : Scoring[word.Length - 1];
			return score < 0 ? -score * word.Length : score;
		}


		private void OnRequest(object recipient, string request)
		{
			if (request == App.c_isBoardGenerated)
				_ = WeakReferenceMessenger.Default.Send(this);
		}

		private void OnGameUpdated(object recipient, GameViewModel game)
		{
			if (game.Name != Name)
			{
				m_game = new Game(game);
				IsGameSelected = true;
				Name = m_game.Name;
				Size = m_game.Size;
				ComboLetters = m_game.ComboLettersList;
				ComboLetterIndices = c_validLetters + m_game.ComboLetterIndices;
				Cells5Visible = m_game.Size > 4;
				Cells6Visible = m_game.Size > 5;
				Letters = new string(' ', m_game.Size * m_game.Size);
			}
		}

		private void OnScramble()
		{
			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter,
			// there is no ChangeCanExecute so it is never enabled.
			// See https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/relaycommand and 
			// https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/observableproperty
			if (m_game != null)
			{
				Letters = m_game.Scramble(m_seed);
#if WINDOWS
				// not on Android to try and keep the keyboard hidden
				_ = LettersEntry?.Focus();
#endif
			}
		}

		private void MoveFocus(string coordinates)
		{
			int next = GetIndexFromCoordinates(coordinates).Item2;
			if (LetterEntries[next].Text.Length == 0)
				_ = LetterEntries[next].Focus();
			else
				_ = LettersEntry.Focus();
		}

		private void ClearEntry(string coordinates)
		{
			int current = GetIndexFromCoordinates(coordinates).Item1;
			LetterEntries[current].Text = string.Empty;
		}

		private Tuple<int, int> GetIndexFromCoordinates(string coordinates)
		{
			return coordinates switch
			{
				"00" => Tuple.Create(0, 1),
				"10" => Tuple.Create(1, 2),
				"20" => Tuple.Create(2, 3),
				"30" => Tuple.Create(3, Size == 4 ? 6 : 4),
				"40" => Tuple.Create(4, Size == 5 ? 6 : 5),
				"50" => Tuple.Create(5, 6),
				"01" => Tuple.Create(6, 7),
				"11" => Tuple.Create(7, 8),
				"21" => Tuple.Create(8, 9),
				"31" => Tuple.Create(9, Size == 4 ? 12 : 10),
				"41" => Tuple.Create(10, Size == 5 ? 12 : 11),
				"51" => Tuple.Create(11, 12),
				"02" => Tuple.Create(12, 13),
				"12" => Tuple.Create(13, 14),
				"22" => Tuple.Create(14, 15),
				"32" => Tuple.Create(15, Size == 4 ? 18 : 16),
				"42" => Tuple.Create(16, Size == 5 ? 18 : 17),
				"52" => Tuple.Create(17, 18),
				"03" => Tuple.Create(18, 19),
				"13" => Tuple.Create(19, 20),
				"23" => Tuple.Create(20, 21),
				"33" => Tuple.Create(21, Size == 4 ? 24 : 22),
				"43" => Tuple.Create(22, Size == 5 ? 24 : 23),
				"53" => Tuple.Create(23, 24),
				"04" => Tuple.Create(24, 25),
				"14" => Tuple.Create(25, 26),
				"24" => Tuple.Create(26, 27),
				"34" => Tuple.Create(27, Size == 4 ? 30 : 28),
				"44" => Tuple.Create(28, Size == 5 ? 30 : 29),
				"54" => Tuple.Create(29, 30),
				"05" => Tuple.Create(30, 31),
				"15" => Tuple.Create(31, 32),
				"25" => Tuple.Create(32, 33),
				"35" => Tuple.Create(33, Size == 4 ? -1 : 34),
				"45" => Tuple.Create(34, Size == 5 ? -1 : 35),
				_ => Tuple.Create(0, 0),
			};
		}


		private const string c_validLetters = "ABCDEFGHIJKLMNOPRSTUVWXYZ";
		private readonly int? m_seed;
		private Game m_game;
		private string m_name;
		private int m_size;
		private string m_comboLetters;
		private string m_letters;
		private string m_lettersEntry;
		private bool m_cells5Visible;
		private bool m_cell6Visible;
		private bool m_isGameSelected;
		private bool m_isBoardGenerated;
	}
}
