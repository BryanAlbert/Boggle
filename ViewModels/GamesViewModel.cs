using Boggle.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class GamesViewModel : ObservableObject
	{
		public GamesViewModel()
		{
			Games = new ObservableCollection<GameViewModel>(Game.LoadAll().Select(g => new GameViewModel(g)));
			SelectGameCommand = new RelayCommand<GameViewModel>(OnSelectGame);
			WeakReferenceMessenger.Default.Register<string>(this, OnRequest);
		}


		public GameViewModel Selected { get => m_selected; set => SetProperty(ref m_selected, value); }
		public ObservableCollection<GameViewModel> Games { get; }
		public ICommand SelectGameCommand { get;}


		private void OnSelectGame(GameViewModel game)
		{
			Selected = game;
			_ = WeakReferenceMessenger.Default.Send(game);
		}

		private void OnRequest(object recipient, string message)
		{
			if (message == App.c_sendGameSelection && m_selected != null)
				_ = WeakReferenceMessenger.Default.Send(m_selected);
		}


		private GameViewModel m_selected;
	}
}
