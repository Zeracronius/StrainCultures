using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Defs
{
	[DefOf]
	public static class DefOfStrains
	{
		static DefOfStrains()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DefOfStrains));
		}

		public static HediffDef? SC_Infection;
		public static HediffDef? SC_Mutated;
	}
}
