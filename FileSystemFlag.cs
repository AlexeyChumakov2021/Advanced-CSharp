using System;

namespace Advanced_CSharp
{
	[Flags]
	public enum FileSystemFlag
	{
		None = 0,
		Directory = 1,
		File = 2,
		TxtFile = 4,
		LongNameFile = 8,
		LongNameDirectory = 16
	}
}
