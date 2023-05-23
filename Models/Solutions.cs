namespace Boggle.Models
{
	internal class Solutions : List<Solution>
	{
		public Solutions(int wordLength, List<Solution> solutions) : base(solutions)
		{
			WordLength = wordLength;
		}


		public int WordLength { get; set; }
		public int Score { get; set; }
		public int TotalScore { get; set; }
	}
}
