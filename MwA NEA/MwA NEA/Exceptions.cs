using System;

namespace MwA_NEA
{
	public class RegionNotBoundedException : Exception
	{
		public RegionNotBoundedException() { }
		public RegionNotBoundedException(string message) : base(message) { }
	}
}