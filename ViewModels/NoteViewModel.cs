using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Boggle.ViewModels
{
	internal class NoteViewModel : ObservableObject, IQueryAttributable
	{
		public NoteViewModel()
		{
			m_note = new Models.Note();
			SaveCommand = new AsyncRelayCommand(Save);
			DeleteCommand = new AsyncRelayCommand(Delete);
		}

		public NoteViewModel(Models.Note note)
		{
			m_note = note;
			SaveCommand = new AsyncRelayCommand(Save);
			DeleteCommand = new AsyncRelayCommand(Delete);
		}


		void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
		{
			if (query.TryGetValue("load", out object value))
			{
				m_note = Models.Note.Load(value.ToString());
				RefreshProperties();
			}
		}


		public string Text
		{
			get => m_note.Text;
			set
			{
				if (m_note.Text != value)
				{
					m_note.Text = value;
					OnPropertyChanged();
				}
			}
		}

		public void Reload()
		{
			m_note = Models.Note.Load(m_note.Filename);
			RefreshProperties();
		}


		private async Task Save()
		{
			m_note.Date = DateTime.Now;
			m_note.Save();
			await Shell.Current.GoToAsync($"..?saved={m_note.Filename}");
		}

		private async Task Delete()
		{
			m_note.Delete();
			await Shell.Current.GoToAsync($"..?deleted={m_note.Filename}");
		}

		private void RefreshProperties()
		{
			OnPropertyChanged(nameof(Text));
			OnPropertyChanged(nameof(Date));
		}


		public DateTime Date => m_note.Date;
		public string Identifier => m_note.Filename;
		public ICommand SaveCommand { get; private set; }
		public ICommand DeleteCommand { get; private set; }

		private Models.Note m_note;
	}
}
