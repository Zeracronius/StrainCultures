using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrainCultures.Utilities
{
	internal class TimeMetrics
	{
		/// <summary>
		/// Ticks per TickRare
		/// </summary>
		public const int TICKS_RARE = 250;

		/// <summary>
		/// Ticks between TickLong.
		/// </summary>
		public const int TICKS_LONG = 2000;

		/// <summary>
		/// The ticks per day
		/// </summary>
		public const int TICKS_PER_DAY = 60000;

		/// <summary>
		/// The ticks per hour
		/// </summary>
		public const int TICKS_PER_HOUR = 2500;

		/// <summary>
		/// The ticks per real second at 1x speed.
		/// </summary>
		public const int TICKS_PER_REAL_SECOND = 60;

		/// <summary>
		/// The ticks per game year
		/// </summary>
		public const long TICKS_PER_YEAR = 3600000L;
	}
}
