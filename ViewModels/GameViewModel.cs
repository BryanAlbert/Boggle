using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Boggle.ViewModels
{
	internal class GameViewModel : ObservableObject
	{
		public GameViewModel()
		{
			WeakReferenceMessenger.Default.Register<GameViewModel>(this, OnGameSelected);
			_ = WeakReferenceMessenger.Default.Send(App.c_sendGameSelection);

			// TODO: for testing
#if false
			Letters = "SONGA1RAETDIRFSONKHIDNEPM";
			Letters = "T1LTATJGESFHDEYO";
			Letters = "NAAUSAONPTEPHBEBNLSIOIUTTDTAWTREM2TC";
			Cells5Visible = true;
			Cells6Visible = true;
			m_game = new Game { Size = 6, Name = "Super Big Boggle" };
			Name = m_game.Name;
			Letters = new string(' ', Size * Size);
			GameSelected = true;
#endif
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
		public List<string> Scoring => m_game.Scoring;
		public string RenderScoring => string.Join(", ", m_game.Scoring);
		public List<string> Cubes => m_game.Cubes;
		public string Cubes1 => string.Join(", ", m_game.Cubes.Take(m_game.Size));
		public string Cubes2 => string.Join(", ", m_game.Cubes.Skip(m_game.Size).Take(m_game.Size));
		public string Cubes3 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 2).Take(m_game.Size));
		public string Cubes4 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 3).Take(m_game.Size));
		public string Cubes5 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 4).Take(m_game.Size));
		public string Cubes6 => string.Join(", ", m_game.Cubes.Skip(m_game.Size * 5).Take(m_game.Size));
		public bool IsGameSelected { get => m_isGameSelected; set => SetProperty(ref m_isGameSelected, value); }
		public bool Cells5Visible { get => m_cells5Visible; set => SetProperty(ref m_cells5Visible, value); }
		public bool Cells6Visible { get => m_cell6Visible; set => SetProperty(ref m_cell6Visible, value); }


		private void OnGameSelected(object recipient, GameViewModel game)
		{
			m_game = new Game(game);
			IsGameSelected = true;
			Name = game.Name;
			Size = game.Size;
			Cells5Visible = game.Size > 4;
			Cells6Visible = game.Size > 5;
			Letters = new string(' ', m_game.Size* m_game.Size);
		}


		private string m_name;
		private Game m_game;
		private string m_letters;
		private int m_size;
		private bool m_cells5Visible;
		private bool m_cell6Visible;
		private bool m_isGameSelected;
	}
}
