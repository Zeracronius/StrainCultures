using RimWorld;
using StrainCultures.Hediffs;
using StrainCultures.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Comps
{

	public class CompProperties_InjectStrainCulture : CompProperties
	{
		public CompProperties_InjectStrainCulture()
		{
			compClass = typeof(CompTargetEffect_InjectStrainCulture);
		}
	}


	public class CompTargetEffect_InjectStrainCulture : CompTargetEffect
	{
		public override void DoEffectOn(Pawn _, Thing target)
		{
			if (target is Pawn pawn)
			{
				StrainCulture? culture = parent as StrainCulture;
				if (culture == null)
				{
					Mod.Logging.Warning($"Attempted to inject pawn '{pawn.Name}' with {parent.GetType().Name} strain culture, but no culture was found in the ingredients.");
					return;
				}

				Infection.ApplyInfection(culture, pawn);
			}
		}
	}
}
