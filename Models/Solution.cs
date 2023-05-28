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
		public string RenderPath => string.Join(", ", Path.Select((v, i) => new { v, i }).
			Where(x => x.v > 0).OrderBy(x => x.v).Select(x => x.i));
	}
}
