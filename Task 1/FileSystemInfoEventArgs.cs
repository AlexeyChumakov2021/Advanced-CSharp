using System;
using System.IO;

namespace Task_1
{
	public class FileSystemInfoEventArgs : EventArgs
	{
		public FileSystemInfo Item { get; set; }
		public FileSystemInfoActionType Action { get; set; }
	}
}
