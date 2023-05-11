using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Boggle.ViewModels
{
	internal class GameViewModel : ObservableObject
	{
		public GameViewModel()
		{
			// TODO: for testing
#if false
			Letters = "SONGA1RAETDIRFSONKHIDNEPM";
			Letters = "T1LTATJGESFHDEYO";
			Letters = "NAAUSAONPTEPHBEBNLSIOIUTTDTAWTREM2TC";
			Cells5Visible = true;
			Cells6Visible = true;
			m_game = new Game { Size = 6 };
			GameSelected = true;
#endif
		}

		public GameViewModel(Game game)
		{
			m_game = game;
		}


		public bool GameSelected { get => m_isGameSelected; set => SetProperty(ref m_isGameSelected, value); }
		public int Size => m_game?.Size ?? 0;
		public string Name => m_game.Name;
		public string RenderSize => $"{m_game.Size}x{m_game.Size}";
		public int WordLength => m_game.WordSize;
		public string Scoring => string.Join(", ", m_game.Scoring);
		public string Cubes1 => string.Join(", ", m_game.Cubes.Take(m_game.Size));
		public string Cubes2 => string.Join(", ", m_game.Cubes.Skip(m_game.Size).Take(m_game.Size));
		public string Cubes3 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 2).Take(m_game.Size));
		public string Cubes4 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 3).Take(m_game.Size));
		public string Cubes5 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 4).Take(m_game.Size));
		public string Cubes6 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 5).Take(m_game.Size));
		public string Letters { get => m_letters; set => SetProperty(ref m_letters, value); }
		public bool Cells5Visible { get => m_cells5Visible; set => SetProperty(ref m_cells5Visible, value); }
		public bool Cells6Visible { get => m_cell6Visible; set => SetProperty(ref m_cell6Visible, value); }


		private readonly Game m_game;
		private string m_letters;
		private bool m_cells5Visible;
		private bool m_cell6Visible;
		private bool m_isGameSelected;
	}
}
