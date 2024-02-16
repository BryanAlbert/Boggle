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
				m_solver = new();
				SolutionsCollection = [];
				WeakReferenceMessenger.Default.Register<GameViewModel>(this, OnGameUpdated);
				_ = WeakReferenceMessenger.Default.Send(App.c_isGameSelected);
				_ = WeakReferenceMessenger.Default.Send(App.c_isBoardGenerated);
				if (m_solver.Game == null)
					Message = "Pick a game on the Games page!";

				SolveCommand = new AsyncRelayCommand(OnSolveAsync);
				SelectWordCommand = new RelayCommand<Solution>(OnWordSelected);
				IsNotSolved = true;
			}
			catch (Exception exception)
			{
				IsBoardGenerated = false;
				Message = exception.Message;
			}
		}


		public string Message { get => m_message; private set => SetProperty(ref m_message, value); }
		public string Name { get => m_name; private set => SetProperty(ref m_name, value); }
		public int Size { get => m_size; private set => SetProperty(ref m_size, value); }
		public string Letters { get => m_letters; private set => SetProperty(ref m_letters, value); }
		public int[] Path { get => m_path; private set => SetProperty(ref m_path, value); }
		public int WordCount { get => m_wordCount; private set => SetProperty(ref m_wordCount, value); }
		public int Score { get => m_score; private set => SetProperty(ref m_score, value); }
		public ObservableCollection<Solutions> SolutionsCollection { get; }
		public bool IsBoardGenerated { get => m_isBoardGenerated; private set => SetProperty(ref m_isBoardGenerated, value); }
		public bool IsNotSolved { get => m_isNotSolved; private set => SetProperty(ref m_isNotSolved, value); }
		public bool IsSolving { get => m_isSolving; private set => SetProperty(ref m_isSolving, value); }
		public bool IsSolved { get => m_isSolved; private set => SetProperty(ref m_isSolved, value); }
		public bool IsCells5Visible { get => m_isCells5Visible; private set => SetProperty(ref m_isCells5Visible, value); }
		public bool IsCells6Visible { get => m_isCells6Visible; private set => SetProperty(ref m_isCells6Visible, value); }

		public ICommand SolveCommand { get; }
		public ICommand AtScrollThresholdCommand { get; }
		public ICommand SelectWordCommand { get; }

		private void OnGameUpdated(object recipient, GameViewModel game)
		{
			m_solver.Game = game;
			IsBoardGenerated = game.IsBoardGenerated;
			Message = "Scramble the board on the Game page!";
			Name = game.Name;
			Letters = game.Letters;
			Size = game.Size;
			Path = null;
			IsCells5Visible = game.Size > 4;
			IsCells6Visible = game.Size > 5;
			IsNotSolved = true;
			IsSolved = false;
			SolutionsCollection.Clear();

#if false
			// TODO: testing
			_ = OnSolveAsync();
#endif
		}

		private async Task OnSolveAsync()
		{
			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter
			// there is no ChangeCanExecute so it is never enabled.
			// See https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/relaycommand and 
			// https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/observableproperty
			if (IsSolved)
				return;


			IsNotSolved = false;
			IsSolving = true;
			List<Solution> solutions = await Task.Run(async () => await m_solver.SolveAsync((p, s, c) =>
			{
				Path = p;
				Score = s;
				WordCount = c;
			}));

			Dictionary<int, List<Solution>> solutionsMap = solutions.OrderByDescending(x => x.Word.Length).
				ThenBy(x => x.Word).GroupBy(x => x.Word.Length).ToDictionary(x => x.Key, x => x.ToList());

			foreach (KeyValuePair<int, List<Solution>> group in solutionsMap)
				SolutionsCollection.Add(new(group, [.. group.Value]));

			Path = null;
			IsSolved = true;
			IsSolving = false;
			Score = solutions.Sum(x => x.Score);
			WordCount = solutions.Count;
		}

		private void OnWordSelected(Solution solution)
		{
			Path = solution?.Path;
		}


		private readonly Solver m_solver;
		private string m_message;
		private bool m_isBoardGenerated;
		private bool m_isNotSolved;
		private bool m_isSolving;
		private bool m_isSolved;
		private bool m_isCells5Visible;
		private bool m_isCells6Visible;
		private string m_name;
		private int m_size;
		private string m_letters;
		private int[] m_path;
		private int m_score;
		private int m_wordCount;
	}
}
