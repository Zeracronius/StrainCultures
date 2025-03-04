using RimWorld;
using StrainCultures.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.RecipeWorkers.Surgeries
{
	internal class ExtractStrainCultureWorker : Recipe_Surgery
	{
		List<Infection> _resultCache = new List<Infection>();

		public override bool AvailableOnNow(Thing thing, BodyPartRecord? part = null)
		{
			if (thing is Pawn pawn)
			{
				GetInertInfections(pawn);

				if (_resultCache.Any() == false)
					return false;
			}

			return base.AvailableOnNow(thing, part);
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (pawn is Pawn target)
			{
				GetInertInfections(target);

				int count = _resultCache.Count;
				if (count == 0)
					return;


				CheckSurgeryFail(billDoer, pawn, ingredients, part, bill);

				for (int i = 0; i < count; i++)
				{
					_resultCache[i].ExtractCulture();
				}
			}
		}


		private List<Infection> GetInertInfections(Pawn target)
		{
			_resultCache.Clear();
			target.health.hediffSet.GetHediffs(ref _resultCache, x => x.State == Infection.InfectionState.Inert);
			return _resultCache;
		}
	}
}
