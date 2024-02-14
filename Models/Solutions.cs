using System.Collections.ObjectModel;

namespace Boggle.Models
{
	internal class Solutions(KeyValuePair<int, List<Solution>> solutions, List<Solution> partial) : ObservableCollection<Solution>(partial)
	{
		public int WordLength { get; set; } = solutions.Key;
		public int WordCount { get; set; } = solutions.Value.Count;
		public int Score { get; set; } = solutions.Value.Sum(x => x.Score);

		public bool IsBusy
		{
			get => m_busy;
			set
			{
				m_busy = value;
				OnPropertyChanged(new(nameof(IsBusy)));
			}
		}


		private bool m_busy;
	}
}
