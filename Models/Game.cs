using Boggle.ViewModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Boggle.Models
{
	internal class Game
	{
		public Game()
		{
			m_random = new Random();
		}

		public Game(GameViewModel game) : this()
		{
			Name = game.Name;
			Size = game.Size;
			WordSize = game.WordLength;
			Scoring = game.Scoring.Select(x => x.ToString()).ToList();
			Cubes = game.Cubes;
			IEnumerable<int> comboIndicies = Cubes.SelectMany(x => x.Where(char.IsDigit)).Distinct().Order().
				Select(x => int.Parse(x.ToString()));

			ComboLetterIndicies = string.Concat(comboIndicies);
			ComboLettersList = string.Join(", ", comboIndicies.Select(x => $"{x}={m_comboLetters[x]}"));
		}


		public static IEnumerable<Game> LoadAll()
		{
			SaveDefaultJson();
			return Directory.EnumerateFiles(FileSystem.AppDataDirectory, "*.game.json").
				Select(x => Load(x)).OrderBy(x => x.Order);
		}

		public static Game Load(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Unable to find file on local storage.", filePath);

			Game game = JsonSerializer.Deserialize<Game>(File.ReadAllText(filePath));
			return game;
		}

		public static void SaveJson(string filepath, string json)
		{
			File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, filepath), json);
		}

		public static string RenderLetter(char letter)
		{
			if (int.TryParse(letter.ToString(), out int number))
				return m_comboLetters[number];

			return letter.ToString();
		}

		public static string RenderWord(string word)
		{
			StringBuilder builder = new();
			foreach (char letter in word)
			{
				if (int.TryParse(letter.ToString(), out int number))
					builder.Append(m_comboLetters[number]);
				else
					builder.Append(letter);
			}

			return builder.ToString();
		}

		public static string ParseLetter(string letter)
		{
			if (letter.Length < 2)
				return letter;

			string result = Array.IndexOf(m_comboLetters, letter).ToString();
			return result == "-1" ? letter : result;
		}


		public static string[] m_comboLetters = { "  ", "Qu", "In", "Th", "Er", "He", "An" };


		public string Scramble()
		{
			string letters = string.Empty;
			List<int> used = new();
			for (int index = 0; index < Cubes.Count; index++)
			{
				int cube = m_random.Next(Cubes.Count);
				while (used.Contains(cube % Cubes.Count))
					cube++;

				cube %= Cubes.Count;
				used.Add(cube);
				int side = m_random.Next(6);
				letters += Cubes[cube][side];
			}

			return letters;
		}


		public string Name { get; set; }
		public int Size { get; set; }
		public int WordSize { get; set; }
		public List<string> Scoring { get; set; }
		public List<string> Cubes { get; set; }
		public double Order { get; set; }
		public string Filename { get; set; }

		[JsonIgnore]
		public string ComboLetterIndicies { get; }
		[JsonIgnore]
		public string ComboLettersList { get; set; }


		private static void SaveDefaultJson()
		{
			foreach (string jsonText in m_defaultGames)
			{
				Game json = JsonSerializer.Deserialize<Game>(jsonText);
				string filepath = Path.Combine(FileSystem.AppDataDirectory, json.Filename);
				if (!File.Exists(filepath))
					SaveJson(filepath, JsonSerializer.Serialize(json, m_serializerOptions));
			}
		}


		private readonly Random m_random;
		private static readonly JsonSerializerOptions m_serializerOptions = new() { WriteIndented = true };
		private static readonly List<string> m_defaultGames = new()
		{
			@"{ ""Name"": ""Boggle Classic"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""AEANEG"", ""AHSPCO"", ""ASPFFK"", ""OBJOAB"", ""IOTMUC"", ""RYVDEL"", ""LREIXD"", ""EIUNES"", ""WNGEEH""," +
				@" ""LNHNRZ"", ""TSTIYD"", ""OWTOAT"", ""ERTTYL"", ""TOESSI"", ""TERWHV"", ""NUIHM1"" ], ""Order"": 0.0, ""Filename"": ""Classic4x4.game.json"" }",

			@" { ""Name"": ""Big Boggle Classic"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", ""AFIRSY"", ""BJK1XZ""," +
				@"""CCENST"", ""CEIILT"", ""CEIPST"", ""DDHNOT"", ""DHHLOR"", ""DHHLOR"", ""DHLNOR"", ""EIIITT"", ""CEILPT"", ""EMOTTT""," +
				@"""ENSSSU"", ""FIPRSY"", ""GORRVW"", ""IPRRRY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 1.0, ""Filename"": ""BigClassic5x5.game.json"" } ",

			@" { ""Name"": ""Super Big Boggle"", ""Size"": 6, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"", ""-2"" ]," +
				@"""Cubes"": [ ""AAAFRS"", ""AAEEEE"", ""AAEEOO"", ""AAFIRS"", ""ABDEIO"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN""," +
				@"""AEILMN"", ""AEINOU"", ""AFIRSY"", ""123456"", ""BBJKXZ"", ""CCENST"", ""CDDLNN"", ""CEIITT"", ""CEIPST"", ""CFGNUY""," +
				@"""DDHNOT"", ""DHHLOR"", ""DHHNOW"", ""DHLNOR"", ""EHILRS"", ""EIILST"", ""EILPST"", ""EIO000"", ""EMTTTO"", ""ENSSSU""," +
				@"""GORRVW"", ""HIRSTV"", ""HOPRST"", ""IPRSYY"", ""JK1WXZ"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 2.0, ""Filename"": ""SuperBig6x6.game.json"" } ",

			@"{ ""Name"": ""Boggle Update"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@" ""Cubes"": [ ""AAEEGN"", ""ELRTTY"", ""AOOTTW"", ""ABBJOO"", ""EHRTVW"", ""CIMOTU"", ""DISTTY"", ""EIOSST"", ""DELRVY""," +
				@"""ACHOPS"", ""HIMN1U"", ""EEINSU"", ""EEGHNW"", ""AFFKPS"", ""HLNNRZ"", ""DEILRX"" ], ""Order"": 3.0, ""Filename"": ""Updated4x4.game.json"" }",

			@" { ""Name"": ""Boggle German"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""PTESUL"", ""ENTVIG"", ""PEDCAM"", ""RESCAL"", ""VANZED"", ""RILWEU"", ""FEESIH"", ""TONKEU"", ""RESNIH""," +
				@"""TAAEIO"", ""ENTSOD"", ""BO1JAM"", ""ROSMAI"", ""YUNGLE"", ""FOXRAI"", ""BARTIL"" ], ""Order"": 4.0, ""Filename"": ""German4x4.game.json"" } ",

			@" { ""Name"": ""Boggle Dutch"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""AAEIOW"", ""AIMORS"", ""EGNJUY"", ""ABILTN"", ""ACDEMP"", ""EGINTV"", ""GELRUW"", ""ELPNTU"", ""DENOST""," +
				@"""ACEHRS"", ""DBJMO1"", ""EEFHIS"", ""EHINRS"", ""EKNOTZ"", ""ADENVZ"", ""AIFKRX"" ], ""Order"": 5.0, ""Filename"": ""Dutch4x4.game.json"" } ",

			@" { ""Name"": ""Boggle French"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""AAEIOT"", ""AIMORS"", ""EGNLUY"", ""ABILTR"", ""ACDEMP"", ""EGINTV"", ""EILRUW"", ""ELPSTU"", ""DENOST""," +
				@"""ACELRS"", ""ABJMO1"", ""EEFHIS"", ""EHINRS"", ""EKNOTU"", ""ADENVZ"", ""AIFORX"" ], ""Order"": 6.0, ""Filename"": ""French4x4.game.json"" } ",

			@" { ""Name"": ""Big Boggle Deluxe"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", ""AFIRSY"", ""BJK1XZ""," +
				@"""CCNSTW"", ""CEIILT"", ""CEIPST"", ""DDLNOR"", ""DHHLOR"", ""DHHNOT"", ""DHLNOR"", ""EIIITT"", ""CEILPT"", ""EMOTTT""," +
				@"""ENSSSU"", ""FIPRSY"", ""GORRVW"", ""HIPRRY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 7.0, ""Filename"": ""BigDeluxe5x5.game.json"" } ",

			@" { ""Name"": ""Big Boggle Challenge"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", ""AFIRSY"", ""BJK1XZ""," +
				@"""CCENST"", ""CEIILT"", ""CEIPST"", ""DDHNOT"", ""DHHLOR"", ""IKLM1U"", ""DHLNOR"", ""EIIITT"", ""CEILPT"", ""EMOTTT""," +
				@"""ENSSSU"", ""FIPRSY"", ""GORRVW"", ""IPRRRY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 8.0, ""Filename"": ""BigChallenge5x5.game.json"" } ",
			
			@" { ""Name"": ""Big Boggle 2012"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ]," +
				@"""Cubes"": [ ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", ""AFIRSY"", ""BBJKXZ""," +
				@"""CCENST"", ""EIILST"", ""CEIPST"", ""DDHNOT"", ""DHHLOR"", ""DHHNOW"", ""DHLNOR"", ""EIIITT"", ""EILPST"", ""EMOTTT""," +
				@"""ENSSSU"", ""123456"", ""GORRVW"", ""IPRSYY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 9.0, ""Filename"": ""Big20125x5.game.json"" } "
		};
	}
}
