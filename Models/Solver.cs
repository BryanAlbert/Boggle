using Boggle.Models;
using Boggle.ViewModels;
using System.Reflection;

namespace Boggle
{
	internal class Solver
	{
		public Solver()
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(c_dictionaryPath);
			{
				using StreamReader reader = new(stream);
				m_dictionary = reader.ReadToEnd().Split("\r\n");
			}
		}


		public List<Solution> WordList { get; set; } = new List<Solution>();
		public GameViewModel Game
		{
			get => m_game;
			set
			{
				m_game = value;
				if (m_game.IsBoardGenerated)
					FilterWordList();
			}
		}


		public async Task<List<Solution>> SolveAsync(Action<int[], int, int> setPath)
		{
			m_setPath = setPath;
			WordList.Clear();
			if (Game == null)
				throw new InvalidOperationException("Game has not been initialized.");

			int[] path = new int[Game.Letters.Length];
			for (int index = 0; index < Game.Letters.Length; index++)
			{
				string word = $"{Models.Game.RenderLetter(Game.Letters[index])}";
				if (word == "  ")
					continue;

				path[index] = 1;
				await FindWordsAtAsync(path, word, index);
				path[index] = 0;
			}

			return WordList.OrderBy(x => x.Score).ThenBy(x => x.Word).ToList();
		}


		private void FilterWordList()
		{
			if (Game == null)
				throw new ArgumentNullException(nameof(Game));

			string letters = Models.Game.RenderWord(string.Join("", Game.Letters.Distinct())).ToUpper();
			List<string> words = new();
			foreach (string word in m_dictionary)
				if (!word.Any(x => !letters.Contains(x)))
					words.Add(word);

			m_abridged = words.ToArray();
		}

		private async Task FindWordsAtAsync(int[] path, string word, int from)
		{
			foreach (Directions direction in Enum.GetValues(typeof(Directions)))
			{
				int index = GetIndexInDirection(path, from, direction);
				if (index == -1)
					continue;

				string letter = Models.Game.RenderLetter(Game.Letters[index]);
				if (letter == "  ")
					continue;

				string testWord = word + letter;
				bool? isWord = IsWord(testWord.Trim().ToUpper());
				if (isWord == false)
					continue;

				int[] pathCopy = new int[path.Length];
				Array.Copy(path, pathCopy, Game.Letters.Length);
				pathCopy[index] = testWord.Length;
				if (isWord == true && testWord.Length >= Game.WordLength && !WordList.Any(x => x.Word == testWord))
				{
					WordList.Add(new Solution(testWord, pathCopy, Game));
					await MainThread.InvokeOnMainThreadAsync(() =>
						m_setPath(pathCopy, WordList.Sum(x => x.Score), WordList.Count));
				}

				await FindWordsAtAsync(pathCopy, testWord, index);
			}
		}

		private int GetIndexInDirection(int[] path, int from, Directions direction)
		{
			int index;
			switch (direction)
			{
				case Directions.E:
					index = AtRightEdge(from) ? -1 : from + 1;
					break;
				case Directions.SE:
					index = AtRightEdge(from) || AtBottomEdge(from) ? -1 : from + Game.Size + 1;
					break;
				case Directions.S:
					index = AtBottomEdge(from) ? -1 : from + Game.Size;
					break;
				case Directions.SW:
					index = AtBottomEdge(from) || AtLeftEdge(from) ? -1 : from + Game.Size - 1;
					break;
				case Directions.W:
					index = AtLeftEdge(from) ? -1 : from - 1;
					break;
				case Directions.NW:
					index = AtLeftEdge(from) || AtTopEdge(from) ? -1 : from - Game.Size - 1;
					break;
				case Directions.N:
					index = AtTopEdge(from) ? -1 : from - Game.Size;
					break;
				case Directions.NE:
					index = AtTopEdge(from) || AtRightEdge(from) ? -1 : from - Game.Size + 1;
					break;
				default:
					return -1;
			}

			return index != -1 && path[index] == 0 ? index : -1;
		}

		private bool? IsWord(string word)
		{
			int found = Array.BinarySearch(m_abridged, word);
			if (found >= 0)
				return true;

			found = ~found;
			if (found >= m_abridged.Length)
				return false;
		
			string nextWord = (string) m_abridged.GetValue(found);
			return (word.Length < nextWord.Length && nextWord[..word.Length] == word) ? null : false;
		}

		private bool AtRightEdge(int from)
		{
			return (from + 1) % Game.Size == 0;
		}

		private bool AtBottomEdge(int from)
		{
			return from > Game.Letters.Length - Game.Size - 1;
		}

		private bool AtLeftEdge(int from)
		{
			return (from + Game.Size) % Game.Size == 0;
		}

		private bool AtTopEdge(int from)
		{
			return from < Game.Size;
		}


		private enum Directions { E, SE, S, SW, W, NW, N, NE }
		private const string c_dictionaryPath = "Boggle.Dictionary.txt";
		private readonly string[] m_dictionary;
		private Array m_abridged;
		private GameViewModel m_game;
		private Action<int[], int, int> m_setPath;
	}
}
