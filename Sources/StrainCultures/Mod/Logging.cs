using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Mod
{
	internal static class Logging
	{
		private const string PREFIX = "[StrainCultures] ";

		internal static void Error(string message)
		{
			Log.Error(PREFIX + message);
		}

		internal static void Error(Exception exception)
		{
			Log.Error(PREFIX + exception);
		}

		internal static void Warning(string message)
		{
			Log.Warning(PREFIX + message);
		}

		internal static void Message(string message)
		{
			Log.Message(PREFIX + message);
		}
	}
}
