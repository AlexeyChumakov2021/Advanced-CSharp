using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Task_1.Contracts;

namespace Task_1
{
	public class FileSystemVisitor : IEnumerable<FileSystemInfo>
	{
		private readonly IFileSystemInfosProvider _provider;
		private readonly string _directoryPath;
		private readonly Func<FileSystemInfo, bool> _filter;

		public FileSystemVisitor(IFileSystemInfosProvider provider, string directoryPath) : this(provider, directoryPath, null)
		{
		}

		public FileSystemVisitor(IFileSystemInfosProvider provider, string directoryPath, Func<FileSystemInfo, bool> filter)
		{
			_provider = provider;
			_directoryPath = directoryPath;
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
			Start?.Invoke(_directoryPath);

			IEnumerable<FileSystemInfo> fileInfos = _provider.GetFileSystemInfos(_directoryPath);

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

				if (e.Action is FileSystemInfoActionType.SkipItem)
					continue;

				yield return fileInfo;

				if (e.Action is FileSystemInfoActionType.StopSearch)
				{
					Finish?.Invoke(_directoryPath);

					yield break;
				}
			}

			Finish?.Invoke(_directoryPath);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}