using Boggle.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class GamesViewModel
	{
		public GamesViewModel()
		{
			Games = new ObservableCollection<GameViewModel>(Game.LoadAll().Select(g => new GameViewModel(g)));
			SelectGameCommand = new AsyncRelayCommand<GamesViewModel>(SelectGameAsync);
		}


		public ObservableCollection<GameViewModel> Games { get; }
		public ICommand SelectGameCommand { get;}


		private async Task SelectGameAsync(GamesViewModel game)
		{
			// send a Messaging Service message that this is the selected game
			throw new NotImplementedException();
		}
	}
}
