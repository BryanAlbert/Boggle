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
			WeakReferenceMessenger.Default.Register<string>(this, OnRequest);
			WeakReferenceMessenger.Default.Register<GameViewModel>(this, OnGameUpdated);
			_ = WeakReferenceMessenger.Default.Send(App.c_isGameSelected);
			ScrambleCommand = new RelayCommand(OnScramble);
		}

		public GameViewModel(Game game)
		{
			m_game = game;
			Name = game.Name;
			Size = game.Size;
		}


		public string Name { get => m_name; set => SetProperty(ref m_name, value); }
		public string Letters { get => m_letters; set => SetProperty(ref m_letters, value); }
		public int Size { get => m_size; set => SetProperty(ref m_size, value); }
		public string RenderSize => $"{m_game.Size}x{m_game.Size}";
		public int WordLength => m_game.WordSize;
		public List<int> Scoring => m_game.Scoring.Select(x => int.Parse(x)).ToList();
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
		public ICommand ScrambleCommand { get; }


		public int ComputeScore(string word)
		{
			int score = word.Length > Scoring.Count ? Scoring.Last() : Scoring[word.Length - 1];
			return score < 0 ? -score * word.Length : score;
		}


		private void OnRequest(object recipient, string request)
		{
			if (request == App.c_isBoardGenerated && IsBoardGenerated)
				_ = WeakReferenceMessenger.Default.Send(this);
		}

		private void OnGameUpdated(object recipient, GameViewModel game)
		{
			if (game != this)
			{
				m_game = new Game(game);
				IsGameSelected = true;
				Name = game.Name;
				Size = game.Size;
				Cells5Visible = game.Size > 4;
				Cells6Visible = game.Size > 5;
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
				Letters = m_game.Scramble();
				IsBoardGenerated = true;
				_ = WeakReferenceMessenger.Default.Send(this);
			}
		}


		private string m_name;
		private Game m_game;
		private string m_letters;
		private int m_size;
		private bool m_cells5Visible;
		private bool m_cell6Visible;
		private bool m_isGameSelected;
		private bool m_isBoardGenerated;
	}
}
