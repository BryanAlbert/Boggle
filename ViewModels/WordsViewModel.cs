using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Maui.Alerts;
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
				AtScrollThresholdCommand = new AsyncRelayCommand(OnAtScrollThresholdAsync);
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
		public ObservableCollection<Solutions> Solutions { get; private set; }
		public bool IsBoardGenerated { get => m_isBoardGenerated; private set => SetProperty(ref m_isBoardGenerated, value); }
		public bool IsNotSolved { get => m_isNotSolved; private set => SetProperty(ref m_isNotSolved, value); }
		public bool IsSolving { get => m_isSolving; private set => SetProperty(ref m_isSolving, value); }
		public bool IsSolved { get => m_isSolved; private set => SetProperty(ref m_isSolved, value); }
		public bool IsCells5Visible { get => m_isCells5Visible; private set => SetProperty(ref m_isCells5Visible, value); }
		public bool IsCells6Visible { get => m_isCells6Visible; private set => SetProperty(ref m_isCells6Visible, value); }
		public bool HasScrolled
		{
			get { lock (m_lock) { return m_hasScrolled; } }
			set { lock (m_lock) { m_hasScrolled = value; } }
		}

		public int SolveCount
		{
			get { lock (m_lock) { return m_solveCount; } }
			private set { lock (m_lock) { m_solveCount = value; } }
		}

		public bool? AllSolutionsLoaded
		{
			get { lock (m_lock) { return m_allSolutionsLoaded; } }
			private set { lock (m_lock) { m_allSolutionsLoaded = value; } }
		}

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
			HasScrolled = false;
			AllSolutionsLoaded = false;
			Solutions.Clear();
			m_solutionsMap = null;
			SolveCount = 0;

#if false
			// TODO: testing
			_ = OnSolveAsync();
#endif
		}

		private async Task OnSolveAsync()
		{
			if (!IsBoardGenerated || HasScrolled || DeviceInfo.Platform == DevicePlatform.Android && IsSolved)
				return;

			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter
			// there is no ChangeCanExecute so it is never enabled.
			// See https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/relaycommand and 
			// https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/observableproperty
			if (SolveCount++ < 2)
			{
				// TODO: remove this hack when bugs are fixed: allow solving twice since the Scrolled event, requried for
				// a workaround for Windows not executing the RemainingItemsThresholdReachedCommand ICommand 
				// when loading the CollectionView dynamically, required since it's so freaking slow, isn't fired unless 
				// the CollectionView is loaded twice, apparently (and even then, it doesn't always work)
				if (SolveCount > 1)
				{
					Solutions.Clear();
					await Task.Delay(500);
				}

				IsNotSolved = false;
				IsSolving = true;
				List<Solution> solutions = await Task.Run(async () => await m_solver.SolveAsync((p, s, c) =>
				{
					Path = p;
					Score = s;
					WordCount = c;
				}));

				Path = null;
				m_solutionsMap = solutions.OrderByDescending(x => x.Word.Length).ThenBy(x => x.Word).
					GroupBy(x => x.Word.Length).ToDictionary(x => x.Key, x => x.ToList());

				// TODO: remove this hackishness when more MAUI bugs are fixed, in the meantime, add filler at the end
				// to make the CollectionView scroll the real solutions into view when requested
				List<Solution> bogusList = new();
				for (int bogus = 0; bogus < 10; bogus++)
					bogusList.Add(new Solution());

				m_solutionsMap.Add(0, bogusList);

				IsSolved = true;
				IsSolving = false;
				Score = solutions.Sum(x => x.Score);
				WordCount = solutions.Count;
				m_nextKey = m_solutionsMap.First().Key;
				m_nextValue = 0;
				await LoadSolutionsAsync();

				// TODO: figure out a better way to do this... since CollectionView's spacing doesn't work on Windows,
				// it's only Android that can show more than the ten Solutions on one page, so add more
				if (DeviceInfo.Platform == DevicePlatform.Android)
				{
					// hopefully show the first batch of solutions first
					await Task.Delay(250);
					await LoadSolutionsAsync();
				}
				else
				{
					m_toastTimer = new(async (x) =>
					{
						if (!HasScrolled && (int) x == SolveCount && AllSolutionsLoaded == false)
						{
							if (SolveCount == 1)
								await Toast.Make("Can't scroll? Tap the S button to try again...").Show(m_cancellationTokenSource.Token);
							else if (SolveCount == 2)
								await Toast.Make("Still can't scroll? Tap the S button to load all solutions...").Show(m_cancellationTokenSource.Token);
						}
					}, SolveCount, TimeSpan.FromSeconds(4), Timeout.InfiniteTimeSpan);
				}
			}
			else if (AllSolutionsLoaded == false)
			{
				// TODO: remove when bug is fixed... hack on Windows, since the RemainingItemsTreshold(Reached|Command)
				// are broken, we use the Solve command to activate loading more solutions, as would happen when scrolling
				// the list past the threshold. Shouldn't be necessary if the othe hack (above) is working...
				await Toast.Make("Loading solutions...").Show(m_cancellationTokenSource.Token);
				_ = Task.Run(LoadAllSolutionsAsync);
			}
			else
			{
				await Toast.Make("All ths solutions have been loaded.").Show(m_cancellationTokenSource.Token);
			}
		}

		private async Task OnAtScrollThresholdAsync()
		{
			await LoadSolutionsAsync();
		}

		private async Task LoadAllSolutionsAsync()
		{
			AllSolutionsLoaded = null;
			while (!HasScrolled && await LoadSolutionsAsync()) ;
		}

		private async Task<bool> LoadSolutionsAsync()
		{
			try
			{
				await m_loadSemaphore.WaitAsync();

				// TODO: change this to something like the following when bugs are fixed and we can scroll
				// to the bottom of the CollectionView properly
				//			if (WordCount == Solutions.Where(x => x.WordLength > 0).Sum(x => x.Count))
				if (Solutions.Any(x => x.WordLength == 0 && x.Count == 10))
				{
					AllSolutionsLoaded = true;
					return false;
				}

				int remaining = c_scrollBatchSize;
				foreach (KeyValuePair<int, List<Solution>> solutions in m_solutionsMap.Where(x => x.Key <= m_nextKey))
				{
					if (remaining == 0)
					{
						m_nextKey = solutions.Key;
						m_nextValue = 0;
						break;
					}

					int take = Math.Min(remaining, solutions.Value.Count - m_nextValue);
					Solutions addSolutions = new(solutions, solutions.Value.Skip(m_nextValue).Take(take).ToList());
					if (m_nextValue == 0)
					{
						await MainThread.InvokeOnMainThreadAsync(() => Solutions.Add(addSolutions));
					}
					else
					{
						Solutions existing = Solutions.First(x => x.WordLength == m_nextKey);
						foreach (Solution add in addSolutions)
							await MainThread.InvokeOnMainThreadAsync(() => existing.Add(add));
					}

					if ((remaining -= take) == 0)
					{
						if (take == solutions.Value.Count - m_nextValue)
							continue;

						m_nextKey = solutions.Key;
						m_nextValue += take;
						break;
					}

					m_nextValue = 0;
				}

				return true;
			}
			finally
			{
				_ = m_loadSemaphore.Release();
			}
		}

		private void OnWordSelected(Solution solution)
		{
			Path = solution?.Path;
		}


		private const int c_scrollBatchSize = 10;
		private readonly object m_lock = new();
		private readonly Solver m_solver;
		private readonly CancellationTokenSource m_cancellationTokenSource = new();
		private string m_message;
		private bool m_isBoardGenerated;
		private bool m_isNotSolved;
		private bool m_isSolving;
		private bool m_isSolved;
		private bool? m_allSolutionsLoaded;
		private bool m_hasScrolled;
		private bool m_isCells5Visible;
		private bool m_isCells6Visible;
		private string m_name;
		private int m_size;
		private string m_letters;
		private int[] m_path;
		private int m_score;
		private int m_wordCount;
		Dictionary<int, List<Solution>> m_solutionsMap;
		private int m_nextKey;
		private int m_nextValue;
		private int m_solveCount;
		private Timer m_toastTimer;
		private static readonly SemaphoreSlim m_loadSemaphore = new SemaphoreSlim(1, 1);
	}
}
