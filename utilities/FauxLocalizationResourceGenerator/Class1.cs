using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Resources;


namespace FauxLocalizationResourceGenerator
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string folder;
			string locale;

			if (args.Length < 1)
			{
				Console.WriteLine("Usage: FauxLocalizationResourceGenerator folder [locale]");
				return;
			}
			folder = args[0];
			if (args.Length > 1)
			{
				locale = args[1];
			}
			else
			{
				locale = "fo-fo";
			}
			if (RecursivelyCallRewrite(folder, locale))
				Console.WriteLine("Success");
			else
				Console.WriteLine("Failure");
		}
		
		private static bool RecursivelyCallRewrite(string folder, string locale)
		{
			if (!Directory.Exists(folder))
			{
				Console.WriteLine("Directory " + folder + " does not exist");
				return false;
			}
			string[] dirs = Directory.GetDirectories(folder);
			string[] files = Directory.GetFiles(folder, "*.resx");
 
			foreach (string dir in dirs)
			{
				RecursivelyCallRewrite(dir, locale);
			}

			foreach (string file in files)
			{
				OutputAltResx(file, locale);
			}
			return true;
		}
	
		private static bool OutputAltResx(string file, string locale)
		{
			Console.WriteLine("Processing: " + file);	
			ResXResourceReader resxReader = new ResXResourceReader(file);
			FileInfo currentResxFile = new FileInfo(file);
			string newResxFileName = String.Format("{0}.{1}{2}", currentResxFile.FullName.Substring(0, currentResxFile.FullName.Length - 5), locale, currentResxFile.Extension);
			ResXResourceWriter resxWriter = new ResXResourceWriter(newResxFileName);
			foreach (DictionaryEntry resource in resxReader)
			{
				if ("Sidebar.WidthInPixels".Equals(resource.Key))
				{
					resxWriter.AddResource(resource.Key.ToString(), "225");
					continue;
				}
				
				if (resource.Value.GetType() == typeof(string))
					resxWriter.AddResource(resource.Key.ToString(), MungeResource(resource.Value as string,  StaticKeyValue(resource.Key.ToString())));
				else
					resxWriter.AddResource(resource.Key.ToString(), resource.Value);
			}
			resxWriter.Generate();
			resxWriter.Close();		
			return true;
		}

		private static bool StaticKeyValue(string valToCheck)
		{
			if (valToCheck.EndsWith(".ContextMenuPath")) return true;		
			if (valToCheck.EndsWith(".MainMenuPath")) return true;	
			if (valToCheck.StartsWith("Font.Size.")) return true;	
			if (valToCheck.StartsWith("Command.") && valToCheck.EndsWith(".Shortcut")) return true;	
			if (valToCheck == "Font") return true;
			if (valToCheck == "Sidebar.WidthInPixels") return true;
			return false;
		}

		private static string MungeResource(string origResource, bool excludeAppend)
		{
			int appendLength = (int)Math.Ceiling(origResource.Length * 0.4);
			char[] append = new char[appendLength];
			for( int i = 0; i < appendLength; i++)
				append[i] = FILLER;
			string replaceString = origResource;
/*
			replaceString = replaceString.Replace('a', 'Ā');
			replaceString = replaceString.Replace('b', 'ß');
			replaceString = replaceString.Replace('c', 'ç');
			replaceString = replaceString.Replace('d', 'ď');
			replaceString = replaceString.Replace('g', 'ġ');
			replaceString = replaceString.Replace('h', 'ħ');
			replaceString = replaceString.Replace('i', 'į');
			replaceString = replaceString.Replace('j', 'ĵ');
			replaceString = replaceString.Replace('k', 'ĸ');
			replaceString = replaceString.Replace('l', 'Ļ');
			replaceString = replaceString.Replace('n', 'Ñ');
			replaceString = replaceString.Replace('o', 'Ő');
			replaceString = replaceString.Replace('r', 'Ŕ');
			replaceString = replaceString.Replace('s', 'Ś');
			replaceString = replaceString.Replace('t', 'Ŧ');
			replaceString = replaceString.Replace('u', 'Ü');
			replaceString = replaceString.Replace('w', 'ώ');
			replaceString = replaceString.Replace('x', '×');
			replaceString = replaceString.Replace('z', 'Ż');2
*/
			if (excludeAppend)
				return replaceString;
			else
				return START + replaceString + new string(append) + END;
		}

		private const char FILLER = 'é';
		private const string START = "'も雅"; //"\u00A1"; //"*'àも雅";
		private const string END = "\""; //"!"; // "\"*";
	}
}
