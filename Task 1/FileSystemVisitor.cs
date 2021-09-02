using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Task_1
{
	public class FileSystemVisitor : IEnumerable<FileSystemInfo>
	{
		private readonly string _startDirectoryPath;

		private readonly Func<FileSystemInfo, bool> _filter;

		private readonly FileSystemFlag _stopFlag;
		private readonly FileSystemFlag _ignoreFlag;

		public FileSystemVisitor(string startDirectoryPath, FileSystemFlag stopFlag, FileSystemFlag ignoreFlag) : this(startDirectoryPath, null, stopFlag, ignoreFlag)
		{
		}

		public FileSystemVisitor(string startDirectoryPath, Func<FileSystemInfo, bool> filter, FileSystemFlag stopFlag, FileSystemFlag ignoreFlag)
		{
			_startDirectoryPath = startDirectoryPath;

			_filter = filter;

			_stopFlag = stopFlag;
			_ignoreFlag = ignoreFlag;
		}

		public event Action<string> Start;
		public event Action<string> Finish;

		public event Func<FileSystemInfo, FileSystemFlag, FileSystemFlag, bool> FileFinded;
		public event Func<FileSystemInfo, FileSystemFlag, FileSystemFlag, bool> DirectoryFinded;

		public event Func<FileSystemInfo, FileSystemFlag, FileSystemFlag, bool> FilteredFileFinded;
		public event Func<FileSystemInfo, FileSystemFlag, FileSystemFlag, bool> FilteredDirectoryFinded;

		public IEnumerator<FileSystemInfo> GetEnumerator()
		{
			Start?.Invoke(_startDirectoryPath);

			DirectoryInfo directory = new DirectoryInfo(_startDirectoryPath);

			IEnumerable<FileSystemInfo> fileInfos = directory.GetFileSystemInfos();

			foreach (FileSystemInfo fileInfo in fileInfos)
			{
				bool isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);

				bool? stopSearching;

				if (_filter is not null && _filter(fileInfo))
				{
					if (isDirectory)
						stopSearching = FilteredDirectoryFinded?.Invoke(fileInfo, _stopFlag, _ignoreFlag);
					else
						stopSearching = FilteredFileFinded?.Invoke(fileInfo, _stopFlag, _ignoreFlag);
				}
				else
				{
					if (isDirectory)
						stopSearching = DirectoryFinded?.Invoke(fileInfo, _stopFlag, _ignoreFlag);
					else
						stopSearching = FileFinded?.Invoke(fileInfo, _stopFlag, _ignoreFlag);
				}

				if (stopSearching == true)
					yield break;
				else
					yield return fileInfo;
			}

			Finish?.Invoke(_startDirectoryPath);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}