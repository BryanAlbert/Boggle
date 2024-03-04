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
			Games = new(Game.LoadAll().Select(x => new GameViewModel(x)));
			SelectGameCommand = new RelayCommand<GameViewModel>(OnSelectGame);
			WeakReferenceMessenger.Default.Register<string>(this, OnRequest);
			
#if false
			// TODO: testing
			Selected = Games.FirstOrDefault();
#endif
		}


		public ObservableCollection<GameViewModel> Games { get; }
		public GameViewModel? Selected { get => m_selected; set => SetProperty(ref m_selected, value); }
		public ICommand SelectGameCommand { get; }


		private void OnSelectGame(GameViewModel? game)
		{
			ArgumentNullException.ThrowIfNull(game);
			Selected = game;
			game.IsGameSelected = true;
			_ = WeakReferenceMessenger.Default.Send(game);
		}

		private void OnRequest(object recipient, string message)
		{
			if (message == App.c_isGameSelected && Selected != null)
				_ = WeakReferenceMessenger.Default.Send(Selected);
		}


		private GameViewModel? m_selected;
	}
}
