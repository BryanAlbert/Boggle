namespace Boggle.Models
{
	internal class Solutions : List<Solution>
	{
		public Solutions(int wordLength, List<Solution> solutions) : base(solutions)
		{
			WordLength = wordLength;
			WordCount = solutions.Count;
			Score = solutions.Sum(x => x.Score);
		}


		public int WordLength { get; set; }
		public int WordCount { get; set; }
		public int Score { get; set; }
	}
}
