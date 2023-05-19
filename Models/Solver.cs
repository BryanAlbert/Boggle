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
				string[] data = reader.ReadToEnd().Split("\r\n");
				int start = 0;
				while (data[start++][0] == '#') ;
				if (int.TryParse(data[start - 1], out int wordCount))
				{
					int index = 0;
					m_dictionary = Array.CreateInstance(typeof(string), wordCount);
					foreach (string word in data.Skip(start))
					{
						if (string.IsNullOrEmpty(word) || word[0] == '#')
							continue;

						if (index >= wordCount)
							throw new Exception($"Dictionary contains more than the {wordCount} words specified");

						m_dictionary.SetValue(word, index++);
					}

					if (index != wordCount)
						throw new Exception($"Dictionary specified {wordCount} words, only contains {index}");
				}
				else
				{
					throw new Exception($"Dictionary missing word count.");
				}
			}
		}


		public List<Solution> WordList { get; set; } = new List<Solution>();
		public GameViewModel Game
		{
			get => m_game;
			set
			{
				m_game = value;
				FilterWordList();
			}
		}


		public List<Solution> Solve()
		{
			WordList.Clear();
			if (Game == null)
				throw new ArgumentNullException(nameof(Game));

			int[] path = new int[Game.Letters.Length];
			for (int index = 0; index < Game.Letters.Length; index++)
			{
				string word = $"{Models.Game.RenderLetter(Game.Letters[index])}";
				if (word == "  ")
					continue;

				path[index] = 1;
				FindWordsAt(path, word, index);
				path[index] = 0;
			}

			return WordList.OrderBy(x => x.Score).ThenBy(x => x.Word).ToList();
		}


		private void FilterWordList()
		{
			// TODO: make a list of letters in use, remove all words which don't contain these letters
			if (Game == null)
				throw new ArgumentNullException(nameof(Game));
		}

		private void FindWordsAt(int[] path, string word, int from)
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
					WordList.Add(new Solution(testWord, pathCopy, Game));

				FindWordsAt(pathCopy, testWord, index);
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
			int found = Array.BinarySearch(m_dictionary, word);
			if (found >= 0)
				return true;

			found = ~found;
			if (found >= m_dictionary.Length)
				return false;
		
			string nextWord = (string) m_dictionary.GetValue(found);
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
		private readonly Array m_dictionary;
		private GameViewModel m_game;
	}
}
