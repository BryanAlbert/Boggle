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

				SolveCommand = new AsyncRelayCommand(OnSolveAsync);
				SelectWordCommand = new RelayCommand<Solution>(OnWordSelected);
				SelectHeaderCommand = new RelayCommand<Solutions>(OnHeaderSelected);
			}
			catch (Exception exception)
			{
				BoardGenerated = false;
				Message = exception.Message;
			}
		}


		public string Message { get => m_message; set => SetProperty(ref m_message, value); }
		public string Name { get => m_name; set => SetProperty(ref m_name, value); }
		public int Size { get => m_size; set => SetProperty(ref m_size, value); }
		public string Letters { get => m_letters; set => SetProperty(ref m_letters, value); }
		public int[] Path { get => m_path; set => SetProperty(ref m_path, value); }
		public int WordCount { get => m_wordCount; set => SetProperty(ref m_wordCount, value); }
		public int Score { get => m_score; set => SetProperty(ref m_score, value); }
		public ObservableCollection<Solutions> Solutions { get; private set; }
		public bool BoardGenerated { get => m_boardGenerated; set => SetProperty(ref m_boardGenerated, value); }
		public bool Solved { get => m_solved; private set => SetProperty(ref m_solved, value); }
		public bool Cells5Visible { get => m_cells5Visible; set => SetProperty(ref m_cells5Visible, value); }
		public bool Cells6Visible { get => m_cell6Visible; set => SetProperty(ref m_cell6Visible, value); }
		public ICommand SolveCommand { get; }
		public ICommand SelectHeaderCommand { get; }
		public ICommand SelectWordCommand { get; }


		private void OnGameUpdated(object recipient, GameViewModel game)
		{
			m_solver.Game = game;
			BoardGenerated = game.IsBoardGenerated;
			Message = "Scramble the board on the Game page!";
			Name = game.Name;
			Letters = game.Letters;
			Size = game.Size;
			Path = null;
			Cells5Visible = game.Size > 4;
			Cells6Visible = game.Size > 5;
			Solved = false;
			Solutions.Clear();
			m_solutionsMap = null;

#if false
			// TODO: testing
			OnSolve();
#endif
		}

		private async Task OnSolveAsync()
		{
			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter,
			if (BoardGenerated && !m_solved)
			{
				List<Solution> solutions = await Task.Run(async () => await m_solver.SolveAsync((x) => Path = x));
				Path = null;

				m_solutionsMap = solutions.OrderByDescending(x => x.Word.Length).ThenBy(x => x.Word).
					GroupBy(x => x.Word.Length).ToDictionary(x => x.Key, x => x.ToList());

				foreach (KeyValuePair<int, List<Solution>> solution in m_solutionsMap)
					Solutions.Add(new Solutions(solution.Key, solution.Value));

				Solved = true;
				Score = solutions.Sum(x => x.Score);
				WordCount = solutions.Count;
			}
		}

		private void OnHeaderSelected(Solutions selected)
		{
			Solutions solutions = Solutions.First(x => x.WordLength == selected.WordLength);
			if (solutions.Count == 0)
			{
				foreach (Solution solution in m_solutionsMap[selected.WordLength])
					solutions.Add(solution);
			}
			else
			{
				solutions.Clear();
				Path = null;
			}
		}

		private void OnWordSelected(Solution solution)
		{
			Path = solution?.Path;
		}


		private readonly Solver m_solver;
		private string m_message;
		private bool m_boardGenerated;
		private string m_name;
		private int m_size;
		private string m_letters;
		private int[] m_path;
		private bool m_solved;
		private bool m_cells5Visible;
		private bool m_cell6Visible;
		private int m_score;
		private int m_wordCount;
		Dictionary<int, List<Solution>> m_solutionsMap;
	}
}
