using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class WordsViewModel : ObservableObject
	{
		public WordsViewModel()
		{
			try
			{
				m_solver = new Solver();
				Solutions = new ObservableCollection<Solution>();
				WeakReferenceMessenger.Default.Register<GameViewModel>(this, OnGameUpdated);
				_ = WeakReferenceMessenger.Default.Send(App.c_isGameSelected);
				_ = WeakReferenceMessenger.Default.Send(App.c_isBoardGenerated);
				if (m_solver.Game == null)
					Message = "Pick a game on the Games page!";

				SolveCommand = new RelayCommand(OnSolve);
				SelectWordCommand = new RelayCommand<Solution>(OnWordSelected);
			}
			catch (Exception exception)
			{
				BoardGenerated = false;
				Message = exception.Message;
			}
		}


		public string Message { get => m_message; set => SetProperty(ref m_message, value); }
		public string Name { get => m_name; set => SetProperty(ref m_name, value); }
		public int Score { get => m_score; set => SetProperty(ref m_score, value); }
		public ObservableCollection<Solution> Solutions { get; private set; }
		public bool BoardGenerated { get => m_boardGenerated; set => SetProperty(ref m_boardGenerated, value); }
		public bool Solved { get => m_solved; private set => SetProperty(ref m_solved, value); }
		public ICommand SolveCommand { get; set; }
		public ICommand SelectWordCommand { get; set; }

		private void OnGameUpdated(object recipient, GameViewModel game)
		{
			m_solver.Game = game;
			BoardGenerated = game.IsBoardGenerated;
			Message = "Scramble the board on the Game page!";
			Name = game.Name;
			Solutions.Clear();
			Solved = false;
		}

		private void OnSolve()
		{
			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter,
			if (BoardGenerated && !m_solved)
			{
				foreach (Solution solution in m_solver.Solve())
					Solutions.Add(solution);
			
				Solved = true;
				Score = Solutions.Sum(x => x.Score);
			}
		}

		private void OnWordSelected(Solution solution)
		{
			throw new NotImplementedException();
		}


		private readonly Solver m_solver;
		private string m_message;
		private bool m_boardGenerated;
		private string m_name;
		private bool m_solved;
		private int m_score;
	}
}
