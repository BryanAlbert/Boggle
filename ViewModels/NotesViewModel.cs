using Boggle.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class NotesViewModel : IQueryAttributable
	{
		public NotesViewModel()
		{
			Notes = new ObservableCollection<NoteViewModel>(Note.LoadAll().Select(n => new NoteViewModel(n)));
			NewCommand = new AsyncRelayCommand(NewNoteAsync);
			SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(SelectNoteAsync);
		}


		void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
		{
			if (query.TryGetValue("deleted", out object deleted))
			{
				string noteId = deleted.ToString();
				NoteViewModel matchedNote = Notes.FirstOrDefault(n => n.Identifier == noteId);
				if (matchedNote != null)
					_ = Notes.Remove(matchedNote);
			}
			else if (query.TryGetValue("saved", out object value))
			{
				string noteId = value.ToString();
				NoteViewModel matchedNote = Notes.FirstOrDefault(n => n.Identifier == noteId);
				if (matchedNote != null)
				{
					matchedNote.Reload();
					Notes.Move(Notes.IndexOf(matchedNote), 0);
				}
				else
				{
					Notes.Insert(0, new NoteViewModel(Note.Load(noteId)));
				}
			}
		}


		public ObservableCollection<NoteViewModel> Notes { get; }
		public ICommand NewCommand { get; }
		public ICommand SelectNoteCommand { get; }


		private async Task NewNoteAsync()
		{
			await Shell.Current.GoToAsync(nameof(Views.NotePage));
		}

		private async Task SelectNoteAsync(ViewModels.NoteViewModel note)
		{
			if (note != null)
				await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}?load={note.Identifier}");
		}
	}
}
