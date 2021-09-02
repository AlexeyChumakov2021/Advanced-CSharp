using System.Collections.Generic;
using System.IO;
using Task_1.Contracts;

namespace Task_1
{
	public class FileSystemInfosProvider : IFileSystemInfosProvider
	{
		public IEnumerable<FileSystemInfo> GetFileSystemInfos(string directoryPath)
		{
			DirectoryInfo directory = new DirectoryInfo(directoryPath);
			return directory.GetFileSystemInfos();
		}
	}
}
