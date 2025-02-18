using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Mod
{
	[StaticConstructorOnStartup]
	internal static class Initialiser
	{
		static Initialiser()
		{
			StrainCulturesMod.Harmony.PatchAll();
			StrainCulturesMod.Current = LoadedModManager.GetMod<StrainCulturesMod>();
			StrainCulturesMod.Settings = StrainCulturesMod.Current.GetSettings<StrainCulturesSettings>();
		}
	}
}
