using System;
using System.IO;
using System.Linq;
using System.Text;

namespace HacknetDecypher
{
	public class Program
	{
		public static int Main(string[] argv)
		{
			if (argv.Length < 1)
			{
				Console.Error.WriteLine("Arguments missing.");
				PrintUsage();
				return 1;
			}

			var dataOrPath = argv[0];

			var data = File.Exists(dataOrPath)
				? File.ReadAllText(dataOrPath)
				: dataOrPath;

			var lines = data.Split(
				new[] { "\r\n", "\n" },
				StringSplitOptions.RemoveEmptyEntries
			);

			var headers = lines[0].Split("::");

			var contentCode = Check(headers[3], "ENCODED");
			var headerCode = argv.Length < 2
				? contentCode
				: Check(headers[4], argv[1]);

			var message = Decrypt(headers[1], headerCode);
			var link = Decrypt(headers[2], headerCode);
			var extension = headers.Length > 4
				? Decrypt(headers[4], headerCode)
				: String.Empty;

			var content = Decrypt(lines[1], contentCode);

			Console.WriteLine("Internal codes:");
			Console.WriteLine("\tHeaders: " + headerCode);
			Console.WriteLine("\tContent: " + contentCode);
			Console.WriteLine();

			Console.WriteLine("Headers:");
			Console.WriteLine("\tMessage: " + message);
			Console.WriteLine("\tLink: " + link);
			Console.WriteLine("\tFile extension: " + extension);
			Console.WriteLine();

			Console.WriteLine("Content:");
			Console.WriteLine(content);

			return 0;
		}

		private static ushort Check(string encrypted, string decrypted)
		{
			return Enumerable
				.Range(0, UInt16.MaxValue)
				.Select(Convert.ToUInt16)
				.Where(code => Decrypt(encrypted, code) == decrypted)
				.First();
		}

		private static string Decrypt(string data, ushort passcode)
		{
			var ret = new StringBuilder();
			var input = data.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			var halfmax = UInt16.MaxValue / 2;

			for (int i = 0; i < input.Length; i++)
			{
				var currentChar = Convert.ToInt32(input[i]);
				var newVal = currentChar - halfmax - passcode;
				newVal /= 1822;

				ret.Append((char)newVal);
			}

			return ret.ToString().Trim();
		}

		private static void PrintUsage()
		{
			Console.WriteLine("Usage: hacknet-decypher <message|message-file-path> [<expected-file-ext>]");
			Console.WriteLine("If the expected file extension is provided, the application tries to find a code that decrypts it correctly and uses that for the headers.");
			Console.WriteLine("The file extension should include a '.' at the beginning.");
		}
	}
}