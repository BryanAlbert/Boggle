using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Boggle.ViewModels
{
	internal class GameViewModel : ObservableObject
	{
		public GameViewModel()
		{
			Letters = "SONGA1RAETDIRFSONKHIDNEPM";
			Letters = "NAAUSAONPTEPHBEBNLSIOIUTTDTAWTREM2TC";
			Letters = "T1LTATJGESFHDEYO";
			Cells5Visible = false;
			Cells6Visible = false;
		}

		public GameViewModel(Game game)
		{
			m_game = game;
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
		public bool Cells5Visible { get => m_cells5Visible; set => SetProperty(ref m_cells5Visible, value); }
		public bool Cells6Visible { get => m_cell6Visible; set => SetProperty(ref m_cell6Visible, value); }


		private readonly Game m_game;
		private string m_letters;
		private bool m_cells5Visible;
		private bool m_cell6Visible;
	}
}
