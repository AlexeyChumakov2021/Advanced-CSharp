using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using NUnit.Framework;
using Task_1;
using Task_1.Contracts;
using Task_2.EqualityComparers;

namespace Task_2
{
	public class FileSystemVisitorTests
	{
		private static string directory = @"D:\NewFolder\";

		private static IEnumerable<FileSystemInfo> FileSystemInfos =>
			new FileSystemInfo[]
			{
				new FileInfo($"{directory}desktop.docx")
				{
					Attributes = FileAttributes.Normal
				},
				new DirectoryInfo($"{directory}dotnet")
				{
					Attributes = FileAttributes.Directory
				},
				new DirectoryInfo($"{directory}Microsoft SQL Server")
				{
					Attributes = FileAttributes.Directory
				},
				new FileInfo($"{directory}new.txt")
				{
					Attributes = FileAttributes.Normal
				},
				new FileInfo($"{directory}VeryVeryVeryVeryBigTextFile.docx")
				{
					Attributes = FileAttributes.Normal
				},
			};

		private void InitializeEventsFileSystemVisitor(FileSystemVisitor fileSystemVisitor)
		{
			fileSystemVisitor.Start += (s => Console.WriteLine($"Searching has been started in directory \"\"\n"));
			fileSystemVisitor.Finish += (s => Console.WriteLine($"\nSearching has been finished in directory \"\""));

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
		}

		[Test]
		public void FileSystemVisitor_Visit_ReturnsAllFileInfosExceptSkiped()
		{
			//Arrange
			Mock<IFileSystemInfosProvider> mockProvider = new Mock<IFileSystemInfosProvider>();
			mockProvider
				.Setup(x => x.GetFileSystemInfos(directory))
				.Returns(FileSystemInfos);

			FileSystemVisitor fileSystemVisitor = new FileSystemVisitor(mockProvider.Object, directory);
			InitializeEventsFileSystemVisitor(fileSystemVisitor);

			IEnumerable<FileSystemInfo> expected = FileSystemInfos.Where(x => !(x.Name.Length > 14 || x.Extension is ".txt"));

			//Act
			IEnumerable<FileSystemInfo> actual = fileSystemVisitor.Visit();

			//Assert
			Assert.That(actual,
				Is.EqualTo(expected).Using(new FileSystemInfoComparer()));
		}

		[Test]
		public void FileSystemVisitor_Visit_ReturnsFileInfosByExpression()
		{
			//Arrange
			Mock<IFileSystemInfosProvider> mockProvider = new Mock<IFileSystemInfosProvider>();
			mockProvider
				.Setup(x => x.GetFileSystemInfos(directory))
				.Returns(FileSystemInfos);

			FileSystemVisitor fileSystemVisitor = new FileSystemVisitor(mockProvider.Object, directory, x =>  x.Name == "dotnet");
			InitializeEventsFileSystemVisitor(fileSystemVisitor);

			IEnumerable<FileSystemInfo> expected = FileSystemInfos.Take(2);

			//Act
			IEnumerable<FileSystemInfo> actual = fileSystemVisitor.Visit();

			//Assert
			Assert.That(actual,
				Is.EqualTo(expected).Using(new FileSystemInfoComparer()));
		}
	}
}