using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class WordsViewModel : ObservableObject
	{
		public WordsViewModel()
		{
			WeakReferenceMessenger.Default.Register<GameViewModel>(this, OnGameUpdated);
			_ = WeakReferenceMessenger.Default.Send(App.c_isBoardGenerated);
			SolveCommand = new RelayCommand(OnSolve);
		}


		public bool IsBoardGenerated { get => m_isBoardGenerated; set => SetProperty(ref m_isBoardGenerated, value); }
		public string Letters { get => m_letters; set => SetProperty(ref m_letters, value); }
		public ICommand SolveCommand { get; set; }


		private void OnGameUpdated(object recipient, GameViewModel game)
		{
			IsBoardGenerated = game.IsBoardGenerated;
		}

		private void OnSolve()
		{
			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter,
			if (IsBoardGenerated)
			{

			}
		}


		private bool m_isBoardGenerated;
		private string m_letters;
	}
}
