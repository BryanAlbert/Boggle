using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Boggle.ViewModels
{
	internal class GameViewModel : ObservableObject
	{
		public GameViewModel()
		{
			Letters = "T1LTATJGESFHDEYO";
		}

		public GameViewModel(Game game)
		{
			m_game = game;
			Letters = "T1LTATJGESFHDEYO";
		}


		public string Name => m_game.Name;
		public string Size => $"{m_game.Size}x{m_game.Size}";
		public int WordLength => m_game.WordSize;
		public string Scoring => string.Join(", ", m_game.Scoring);
		public string Cubes1 => string.Join(", ", m_game.Cubes.Take(m_game.Size));
		public string Cubes2 => string.Join(", ", m_game.Cubes.Skip(m_game.Size).Take(m_game.Size));
		public string Cubes3 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 2).Take(m_game.Size));
		public string Cubes4 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 3).Take(m_game.Size));
		public string Cubes5 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 4).Take(m_game.Size));
		public string Cubes6 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 5).Take(m_game.Size));
		public string Letters { get => m_letters; set => SetProperty(ref m_letters, value); }


		private readonly Game m_game;
		private string m_letters;
	}
}
