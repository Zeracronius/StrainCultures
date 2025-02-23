using RimWorld;
using StrainCultures.Hediffs;
using StrainCultures.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.RecipeWorkers.Surgeries
{
	internal class InjectStrainCultureWorker : Recipe_Surgery
	{
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			StrainCulture culture = ingredients.OfType<StrainCulture>().FirstOrDefault();
			if (culture == null)
			{
				Mod.Logging.Warning($"Attempted to inject pawn '{pawn.Name}' with strain culture, but no culture was found in the ingredients.");
				return;
			}

			pawn.health.AddHediff(new Infection(culture));
			culture.DeSpawn();
		}
	}
}
