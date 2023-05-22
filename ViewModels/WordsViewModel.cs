using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class WordsViewModel : ObservableObject
	{
		public WordsViewModel()
		{
			try
			{
				m_solver = new Solver();
				Solutions = new ObservableCollection<Solution>();
				WeakReferenceMessenger.Default.Register<GameViewModel>(this, OnGameUpdated);
				_ = WeakReferenceMessenger.Default.Send(App.c_isGameSelected);
				_ = WeakReferenceMessenger.Default.Send(App.c_isBoardGenerated);
				if (!IsBoardGenerated)
					Message = "Pick a game on the Games page!";

				SolveCommand = new RelayCommand(OnSolve);
				SelectWordCommand = new RelayCommand<Solution>(OnWordSelected);
			}
			catch (Exception exception)
			{
				IsBoardGenerated = false;
				Message = exception.Message;
			}
		}


		public string Message { get => m_message; set => SetProperty(ref m_message, value); }
		public string Name { get => m_name; set => SetProperty(ref m_name, value); }
		public ObservableCollection<Solution> Solutions { get; private set; }
		public bool IsBoardGenerated { get => m_isBoardGenerated; set => SetProperty(ref m_isBoardGenerated, value); }
		public ICommand SolveCommand { get; set; }
		public ICommand SelectWordCommand { get; set; }


		private void OnGameUpdated(object recipient, GameViewModel game)
		{
			m_solver.Game = game;
			IsBoardGenerated = game.IsBoardGenerated;
			Message = "Scramble the board on the Game page!";
			Name = game.Name;
			Solutions.Clear();
			m_solved = false;
		}

		private void OnSolve()
		{
			// TODO: figure out how to use the RelayCommand constructor that takes the Func<bool> canExecute parameter,
			if (IsBoardGenerated && !m_solved)
			{
				foreach (Solution solution in m_solver.Solve())
					Solutions.Add(solution);
			
				m_solved = true;
			}
		}

		private void OnWordSelected(Solution solution)
		{
			throw new NotImplementedException();
		}


		private readonly Solver m_solver;
		private string m_message;
		private bool m_isBoardGenerated;
		private string m_name;
		private bool m_solved;
	}
}
