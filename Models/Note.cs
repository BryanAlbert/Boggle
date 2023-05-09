namespace Boggle.Models;

internal class Note
{
	public Note()
	{
		Filename = $"{Path.GetRandomFileName()}.note.txt";
		Date = DateTime.Now;
		Text = string.Empty;
	}


	public static IEnumerable<Note> LoadAll()
	{
		string appDataPath = FileSystem.AppDataDirectory;
		return Directory.EnumerateFiles(appDataPath, "*.note.txt").
			Select(filename => Load(Path.GetFileName(filename))).
			OrderByDescending(note => note.Date);
	}

	public static Note Load(string filename)
	{
		filename = Path.Combine(FileSystem.AppDataDirectory, filename);
		if (!File.Exists(filename))
			throw new FileNotFoundException("Unable to find file on local storage.", filename);

		return new()
		{
			Filename = Path.GetFileName(filename),
			Text = File.ReadAllText(filename),
			Date = File.GetLastWriteTime(filename)
		};
	}


	public string Filename { get; set; }
	public string Text { get; set; }
	public DateTime Date { get; set; }


	public void Save() =>
		File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, Filename), Text);

	public void Delete() =>
		File.Delete(Path.Combine(FileSystem.AppDataDirectory, Filename));
}
