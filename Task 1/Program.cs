using System;

namespace Task_1
{
	class Program
	{
		static void Main()
		{
			string startDirectory = @"C:\Program Files";

			FileSystemVisitor fileSystemVisitor = new FileSystemVisitor(startDirectory, file => file.Name == "IIS", FileSystemFlag.TxtFile, FileSystemFlag.LongNameDirectory | FileSystemFlag.LongNameFile);

			fileSystemVisitor.Start += (directory => Console.WriteLine($"Searching has been started in directory \"{directory}\"\n"));
			fileSystemVisitor.Finish += (directory => Console.WriteLine($"\nSearching has been finished in directory \"{directory}\"")); ;

			fileSystemVisitor.FileFinded += ((file, stopFlag, ignoreFlag) =>
			{
				if (!(ignoreFlag.HasFlag(FileSystemFlag.File)
					 || (ignoreFlag.HasFlag(FileSystemFlag.LongNameFile) && file.Name.Length > 14)
					 || (ignoreFlag.HasFlag(FileSystemFlag.TxtFile) && file.Extension == ".txt")))
				{
					Console.WriteLine($"\"{file.Name}\" file was found");
				}

				if (stopFlag.HasFlag(FileSystemFlag.File)
					  || (stopFlag.HasFlag(FileSystemFlag.LongNameFile) && file.Name.Length > 14)
					  || (stopFlag.HasFlag(FileSystemFlag.TxtFile) && file.Extension == ".txt"))
				{
					Console.WriteLine("Stop searching!");
					return true;
				}
				else
				{
					return false;
				}
			});
			fileSystemVisitor.DirectoryFinded += ((directory, stopFlag, ignoreFlag) =>
			{
				if (!(ignoreFlag.HasFlag(FileSystemFlag.Directory)
					  || (ignoreFlag.HasFlag(FileSystemFlag.LongNameDirectory) && directory.Name.Length > 10)))
				{
					Console.WriteLine($"\"{directory.Name}\" directory was found");
				}

				if (stopFlag.HasFlag(FileSystemFlag.Directory)
					  || (stopFlag.HasFlag(FileSystemFlag.LongNameDirectory) && directory.Name.Length > 10))
				{
					Console.WriteLine("Stop searching!");
					return true;
				}
				else
				{
					return false;
				}
			});

			fileSystemVisitor.FilteredFileFinded += ((file, stopFlag, ignoreFlag) =>
			{
				if (!(ignoreFlag.HasFlag(FileSystemFlag.File)
					  || (ignoreFlag.HasFlag(FileSystemFlag.LongNameFile) && file.Name.Length > 14)
					  || (ignoreFlag.HasFlag(FileSystemFlag.TxtFile) && file.Extension == ".txt")))
				{
					Console.BackgroundColor = ConsoleColor.Green;
					Console.ForegroundColor = ConsoleColor.Black;

					Console.WriteLine(
						$"\"{file.Name}\" file was found and matched by filter");

					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.White;
				}

				if (stopFlag.HasFlag(FileSystemFlag.File)
					  || (stopFlag.HasFlag(FileSystemFlag.LongNameFile) && file.Name.Length > 14)
					  || (stopFlag.HasFlag(FileSystemFlag.TxtFile) && file.Extension == ".txt"))
				{
					Console.WriteLine("Stop searching!");
					return true;
				}
				else
				{
					return false;
				}
			});
			fileSystemVisitor.FilteredDirectoryFinded += ((directory, stopFlag, ignoreFlag) =>
			{
				if (!(ignoreFlag.HasFlag(FileSystemFlag.Directory)
					  || (ignoreFlag.HasFlag(FileSystemFlag.LongNameDirectory) && directory.Name.Length > 10)))
				{
					Console.BackgroundColor = ConsoleColor.Green;
					Console.ForegroundColor = ConsoleColor.Black;

					Console.WriteLine(
						$"\"{directory.Name}\" directory was found and matched by filter");

					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.White;
				}

				if (stopFlag.HasFlag(FileSystemFlag.Directory)
					  || (stopFlag.HasFlag(FileSystemFlag.LongNameDirectory) && directory.Name.Length > 10))
				{
					Console.WriteLine("Stop searching!");
					return true;
				}
				else
				{
					return false;
				}
			});

			foreach (var file in fileSystemVisitor)
			{
			}
		}
	}
}