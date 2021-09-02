using System;

namespace Task_1
{
	class Program
	{
		static void Main()
		{
			string startDirectory = @"C:\Program Files";

			FileSystemInfosProvider provider = new FileSystemInfosProvider();

			FileSystemVisitor fileSystemVisitor = new FileSystemVisitor(provider, startDirectory, file => file.Name is "IIS");

			fileSystemVisitor.Start += (directory => Console.WriteLine($"Searching has been started in directory \"{directory}\"\n"));
			fileSystemVisitor.Finish += (directory => Console.WriteLine($"\nSearching has been finished in directory \"{directory}\""));

			fileSystemVisitor.FileFinded += ((_, e) =>
			{
				if (e.Item.Name.Length > 14 || e.Item.Extension is ".txt")
				{
					e.Action = FileSystemInfoActionType.SkipItem;
				}
				else
					Console.WriteLine($"\"{e.Item.Name}\" file was found");
			});
			fileSystemVisitor.DirectoryFinded += ((_, e) =>
			{
				if (e.Item.Name.Length > 14)
				{
					e.Action = FileSystemInfoActionType.SkipItem;
				}
				else
					Console.WriteLine($"\"{e.Item.Name}\" directory was found");
			});

			fileSystemVisitor.FilteredFileFinded += ((_, e) =>
			{
				if (e.Item.Name.Length > 14 || e.Item.Extension is ".txt")
				{
					e.Action = FileSystemInfoActionType.SkipItem;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.Green;
					Console.ForegroundColor = ConsoleColor.Black;

					Console.WriteLine(
						$"\"{e.Item.Name}\" file was found and matched by filter");

					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.White;

					e.Action = FileSystemInfoActionType.StopSearch;
				}
			});
			fileSystemVisitor.FilteredDirectoryFinded += ((_, e) =>
			{
				if (e.Item.Name.Length > 14)
				{
					e.Action = FileSystemInfoActionType.SkipItem;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.Green;
					Console.ForegroundColor = ConsoleColor.Black;

					Console.WriteLine(
						$"\"{e.Item.Name}\" directory was found and matched by filter");

					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.White;

					e.Action = FileSystemInfoActionType.StopSearch;
				}
			});

			fileSystemVisitor.Visit();
		}
	}
}