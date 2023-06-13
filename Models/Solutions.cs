using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Boggle.Models
{
	internal class Solutions : ObservableCollection<Solution>
	{
		public Solutions(KeyValuePair<int, List<Solution>> solutions, List<Solution> partial) : base(partial)
		{
			WordLength = solutions.Key;
			WordCount = solutions.Value.Count;
			Score = solutions.Value.Sum(x => x.Score);
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
