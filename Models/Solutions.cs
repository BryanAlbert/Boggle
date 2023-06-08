using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Boggle.Models
{
	internal class Solutions : ObservableCollection<Solution>
	{
		public Solutions(int wordLength, List<Solution> solutions) : base(new List<Solution>())
		{
			WordLength = wordLength;
			WordCount = solutions.Count;
			Score = solutions.Sum(x => x.Score);
		}


		public int WordLength { get; set; }
		public int WordCount { get; set; }
		public int Score { get; set; }
		public bool IsBusy
		{
			get => m_busy;
			set
			{
				m_busy = value;
				OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsBusy)));
			}
		}


		private bool m_busy;
	}
}
