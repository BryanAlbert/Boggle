using Boggle.ViewModels;
using System.Diagnostics;
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
			IEnumerable<int> comboIndices = Cubes.SelectMany(x => x.Where(char.IsDigit)).Distinct().Order().
				Select(x => int.Parse(x.ToString()));

			ComboLetterIndices = string.Concat(comboIndices);
			ComboLettersList = string.Join(", ", comboIndices.Select(x => $"{x}={m_comboLetters[x]}"));
			HasBonusCube = !Cubes.Any(x => x == c_bonusCube);
			BonusLetterIndices = c_bonusCube;
			BonusLettersList = c_bonusLettersList;
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
			return int.TryParse(letter.ToString(), out int number) ?
				m_comboLetters[number] : letter.ToString();
		}

		public static string RenderWord(string word)
		{
			StringBuilder builder = new();
			foreach (char letter in word)
			{
				if (int.TryParse(letter.ToString(), out int number))
					_ = builder.Append(m_comboLetters[number]);
				else
					_ = builder.Append(letter);
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


		public static readonly string c_bonusCube = "123456";


		public string Name { get; set; }
		public int Size { get; set; }
		public int WordSize { get; set; }
		public List<string> Scoring { get; set; }
		public List<string> Cubes { get; set; }
		public double Order { get; set; }
		public string Filename { get; set; }

		[JsonIgnore]
		public bool HasBonusCube { get; }
		[JsonIgnore]
		public string ComboLetterIndices { get; }
		[JsonIgnore]
		public string ComboLettersList { get; }
		[JsonIgnore]
		public string BonusLetterIndices { get; }
		[JsonIgnore]
		public string BonusLettersList { get; }


		public string Scramble(int? seed)
		{
			if (seed.HasValue)
				m_random = new(seed.Value);

			string letters = string.Empty;
			List<int> used = [];
			for (int index = 0; index < Size * Size; index++)
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


		private static void SaveDefaultJson()
		{
			foreach (string jsonText in m_defaultGames)
			{
				try
				{
					// we don't overwrite existing json files in case the user (somehow) modified them
					// reinstall will be necessary if these are changed between releases
					Game json = JsonSerializer.Deserialize<Game>(jsonText);
					string filepath = Path.Combine(FileSystem.AppDataDirectory, json.Filename);
					if (!File.Exists(filepath))
						SaveJson(filepath, JsonSerializer.Serialize(json, m_serializerOptions));
				}
				catch (Exception exception)
				{
					Debug.WriteLine($"Exception processing json {jsonText}: {exception.Message}");
				}
			}
		}


		private static readonly JsonSerializerOptions m_serializerOptions = new() { WriteIndented = true };
		private static readonly List<string> m_defaultGames =
		[
			@"{ ""Name"": ""Boggle Classic"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""1ABJMO"", ""AACIOT"", ""ABILTY"", ""ACDEMP"", ""ACELRS"", ""ADENVZ"", ""AHMORS"", ""BFIORX"", ""DENOSW"", " +
				@"""DKNOTU"", ""EEFHIY"", ""EGINTV"", ""EGKLUY"", ""EHINPS"", ""ELPSTU"", ""GILRUW"" ], ""Order"": 0.0, ""Filename"": ""Classic4x4.game.json"" }",

			@" { ""Name"": ""Big Boggle Classic"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""1BJKXZ"", ""1IKLUW"", ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", " +
				@"""AFIRSY"", ""CCENST"", ""CEIILT"", ""CEILPT"", ""CEIPST"", ""CIMOTU"", ""DDHNOT"", ""DHHLNO"", ""DHHLOR"", ""DHLNOR"", " +
				@"""EIIITT"", ""ENSSSU"", ""FIPRSY"", ""GORRVW"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 1.0, ""Filename"": ""BigClassic5x5.game.json"" } ",

			@" { ""Name"": ""Super Big Boggle"", ""Size"": 6, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"", ""-2"" ], " +
				@"""Cubes"": [ ""000EIO"", ""123456"", ""1JKWXZ"", ""AAAFRS"", ""AAEEEE"", ""AAEEOO"", ""AAFIRS"", ""ABDEIO"", ""ADENNN"", ""AEEEEM"", " +
				@"""AEEGMU"", ""AEGMNN"", ""AEILMN"", ""AEINOU"", ""AFIRSY"", ""BBJKXZ"", ""CCENST"", ""CDDLNN"", ""CEIITT"", ""CEIPST"", ""CFGNUY"", " +
				@"""DDHNOT"", ""DHHLOR"", ""DHHNOW"", ""DHLNOR"", ""EHILRS"", ""EIILST"", ""EILPST"", ""EMOTTT"", ""ENSSSU"", ""GORRVW"", ""HIRSTV"", " +
				@"""HOPRST"", ""IPRSYY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 2.0, ""Filename"": ""SuperBig6x6.game.json"" } ",

			@"{ ""Name"": ""Boggle Update"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""1HIMNU"", ""AAEEGN"", ""ABBJOO"", ""ACHOPS"", ""AFFKPS"", ""AOOTTW"", ""CIMOTU"", ""DEILRX"", ""DELRVY"", ""DISTTY"", " +
				@"""EEGHNW"", ""EEINSU"", ""EHRTVW"", ""EIOSST"", ""ELRTTY"", ""HLNNRZ"" ], ""Order"": 3.0, ""Filename"": ""Updated4x4.game.json"" }",

			@" { ""Name"": ""International Boggle"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""1ABJMO"", ""AAEIOT"", ""ABILRT"", ""ACDEMP"", ""ACELRS"", ""ADENVZ"", ""AFIORX"", ""AIMORS"", ""DENOST"", " +
				@"""EEFHIS"", ""EGINTV"", ""EGLNUY"", ""EHINRS"", ""EILRUW"", ""EKNOTU"", ""ELPSTU"" ], ""Order"": 4.0, ""Filename"": ""German4x4.game.json"" } ",

			@" { ""Name"": ""Boggle Dutch"", ""Size"": 4, ""WordSize"": 3, ""Scoring"": [ ""0"", ""0"", ""1"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""1BDJMO"", ""AAEIOW"", ""ABILNT"", ""ACDEMP"", ""ACEHRS"", ""ADENVZ"", ""AFIKRX"", ""AIMORS"", ""DENOST"", " +
				@"""EEFHIS"", ""EGINTV"", ""EGJNUY"", ""EGLRUW"", ""EHINRS"", ""EKNOTZ"", ""ELNPTU"" ], ""Order"": 5.0, ""Filename"": ""Dutch4x4.game.json"" } ",

			@" { ""Name"": ""Big Boggle Deluxe"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""1BJKXZ"", ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", ""AFIRSY"", " +
				@"""CCNSTW"", ""CEIILT"", ""CEILPT"", ""CEIPST"", ""DDLNOR"", ""DHHLOR"", ""DHHNOT"", ""DHLNOR"", ""EIIITT"", ""EMOTTT"", " +
				@"""ENSSSU"", ""FIPRSY"", ""GORRVW"", ""HIPRRY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 7.0, ""Filename"": ""BigDeluxe5x5.game.json"" } ",

			@" { ""Name"": ""Big Boggle Challenge"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""1BJKXZ"", ""1IKLUW"", ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", " +
				@"""AFIRSY"", ""CCENST"", ""CEIILT"", ""CEILPT"", ""CEIPST"", ""DDHNOT"", ""DHHLOR"", ""DHLNOR"", ""EIIITT"", ""EMOTTT"", ""ENSSSU"", " +
				@"""FIPRSY"", ""GORRVW"", ""IPRRRY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 8.0, ""Filename"": ""BigChallenge5x5.game.json"" } ",
			
			@" { ""Name"": ""Big Boggle 2012"", ""Size"": 5, ""WordSize"": 4, ""Scoring"": [ ""0"", ""0"", ""0"", ""1"", ""2"", ""3"", ""5"", ""11"" ], " +
				@"""Cubes"": [ ""123456"", ""AAAFRS"", ""AAEEEE"", ""AAFIRS"", ""ADENNN"", ""AEEEEM"", ""AEEGMU"", ""AEGMNN"", ""AFIRSY"", " +
				@"""BBJKXZ"", ""CCENST"", ""CEIPST"", ""DDHNOT"", ""DHHLOR"", ""DHHNOW"", ""DHLNOR"", ""EIIITT"", ""EIILST"", ""EILPST"", " +
				@"""EMOTTT"", ""ENSSSU"", ""GORRVW"", ""IPRSYY"", ""NOOTUW"", ""OOOTTU"" ], ""Order"": 9.0, ""Filename"": ""Big20125x5.game.json"" } "
		];

		private static readonly string[] m_comboLetters = ["  ", "Qu", "In", "Th", "Er", "He", "An"];
		private static readonly string c_bonusLettersList = "1=Qu, 2=In, 3=Th, 4=Er, 5=He, 6=An";


		private Random m_random;
	}
}
