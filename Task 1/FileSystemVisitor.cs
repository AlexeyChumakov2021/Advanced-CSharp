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

		public FileSystemVisitor(string startDirectoryPath) : this(startDirectoryPath, null)
		{
		}

		public FileSystemVisitor(string startDirectoryPath, Func<FileSystemInfo, bool> filter)
		{
			_startDirectoryPath = startDirectoryPath;

			_filter = filter;
		}

		public event Action<string> Start;
		public event Action<string> Finish;

		public event EventHandler<FileSystemInfoEventArgs> FileFinded;
		public event EventHandler<FileSystemInfoEventArgs> DirectoryFinded;

		public event EventHandler<FileSystemInfoEventArgs> FilteredFileFinded;
		public event EventHandler<FileSystemInfoEventArgs> FilteredDirectoryFinded;

		public IEnumerable<FileSystemInfo> Visit()
		{
			foreach (var _ in this)
			{
			}

			return this;
		}

		public IEnumerator<FileSystemInfo> GetEnumerator()
		{
			Start?.Invoke(_startDirectoryPath);

			DirectoryInfo directory = new DirectoryInfo(_startDirectoryPath);
			IEnumerable<FileSystemInfo> fileInfos = directory.GetFileSystemInfos();

			foreach (FileSystemInfo fileInfo in fileInfos)
			{
				FileSystemInfoEventArgs e = new FileSystemInfoEventArgs
				{
					Item = fileInfo,
					Action = FileSystemInfoActionType.None
				};

				bool isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);

				if (_filter is not null && _filter(fileInfo))
				{
					if (isDirectory)
						FilteredDirectoryFinded?.Invoke(this, e);
					else
						FilteredFileFinded?.Invoke(this, e);
				}
				else
				{
					if (isDirectory)
						DirectoryFinded?.Invoke(this, e);
					else
						FileFinded?.Invoke(this, e);
				}

				if (e.Action is FileSystemInfoActionType.StopSearch)
				{
					Finish?.Invoke(_startDirectoryPath);
					yield break;
				}
				else
				{
					if(e.Action is FileSystemInfoActionType.SkipItem)
						continue;
					yield return fileInfo;
				}
			}

			Finish?.Invoke(_startDirectoryPath);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}