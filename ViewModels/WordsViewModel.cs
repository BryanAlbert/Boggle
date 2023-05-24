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
				Solutions = new ObservableCollection<Solutions>();
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
		public int WordCount { get => m_wordCount; set => SetProperty(ref m_wordCount, value); }
		public int Score { get => m_score; set => SetProperty(ref m_score, value); }
		public ObservableCollection<Solutions> Solutions { get; private set; }
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

#if false
			// TODO: testing
			OnSolve();
#endif
		}

		private void OnSolve()
		{
			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter,
			if (BoardGenerated && !m_solved)
			{
				List<Solution> solutions = m_solver.Solve();
				Dictionary<int, List<Solution>> map = solutions.OrderByDescending(x => x.Word.Length).ThenBy(x => x.Word).
					GroupBy(x => x.Word.Length).ToDictionary(x => x.Key, x => x.ToList());

				foreach (KeyValuePair<int, List<Solution>> solution in map)
					Solutions.Add(new Solutions(solution.Key, solution.Value));

				Solved = true;
				Score = solutions.Sum(x => x.Score);
				WordCount = solutions.Count;
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
		private int m_wordCount;
	}
}
