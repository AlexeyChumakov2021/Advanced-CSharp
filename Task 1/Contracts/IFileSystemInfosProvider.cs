using System.Collections.Generic;
using System.IO;

namespace Task_1.Contracts
{
	public interface IFileSystemInfosProvider
	{
		IEnumerable<FileSystemInfo> GetFileSystemInfos(string directoryPath);
	}
}