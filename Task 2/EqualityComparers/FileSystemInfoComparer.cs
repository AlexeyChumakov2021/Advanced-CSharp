using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Task_2.EqualityComparers
{
	class FileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
	{
		public bool Equals(FileSystemInfo x, FileSystemInfo y)
		{
			if (x is null && y is null)
				return true;
			if (y is null || x is null)
				return false;

			return y.Name == x.Name && y.Attributes == x.Attributes;
		}

		public int GetHashCode([DisallowNull] FileSystemInfo obj)
		{
			return obj.GetHashCode();
		}
	}
}
