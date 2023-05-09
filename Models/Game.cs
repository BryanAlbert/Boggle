using System.Text.Json;

namespace Boggle.Models
{
	internal class Game
	{
		public Game()
		{
		}


		public static IEnumerable<Game> LoadAll()
		{
			return Directory.EnumerateFiles(FileSystem.AppDataDirectory, "*.game.json").
				Select(x => Load(x)).OrderBy(x => x.Order);
		}

		public static Game Load(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Unable to find file on local storage.", filePath);

			Game game = JsonSerializer.Deserialize<Game>(File.ReadAllText(filePath));
			return game;
		}


		public string Name { get; set; }
		public int Size { get; set; }
		public int WordSize { get; set; }
		public List<string> Scoring { get; set; }
		public List<string> Cubes { get; set; }
		public double Order { get; set; }
	}
}
