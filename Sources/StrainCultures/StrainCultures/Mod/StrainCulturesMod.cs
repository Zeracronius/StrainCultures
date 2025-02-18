using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace StrainCultures.Mod
{
	internal class StrainCulturesMod : Verse.Mod
	{
		internal static Harmony Harmony = new Harmony("StrainCultures");

#pragma warning disable CS8618 // Will always be initialized by Initialiser.
		internal static StrainCulturesSettings Settings;
		internal static StrainCulturesMod Current;
#pragma warning restore CS8618

		public StrainCulturesMod(ModContentPack content) 
			: base(content)
		{
		}

		public override string SettingsCategory()
		{
			return "Strain cultures";
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{

		}
	}
}
