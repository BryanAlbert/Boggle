namespace Boggle.Models
{
	internal class Solution
	{
		public Solution(string word, int[] path, ViewModels.GameViewModel game)
		{
			Word = word;
			Path = path;
			Score = game.ComputeScore(word);
		}


		public string Word { get; set; }
		public int Score { get; set; }
		public int[] Path { get; set; }
		public string RenderPath => string.Join(", ", Path.Select((p, i) => new { p, i }).
			Where(x => x.p > 0).OrderBy(x => x.p).Select(x => x.i));
	}
}
