using RimWorld;
using StrainCultures.Comps;
using StrainCultures.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace StrainCultures.Jobs
{
	/// <summary>
	/// WorkGiver for automatically assigning pawns to extract culture from culture farms when nearing full.
	/// </summary>
	internal class WorkGiver_ExtractCulture : WorkGiver_Scanner
	{
		List<ThingDef>? _farmDefs = null;
		List<Thing> _things = new List<Thing>();


		// Get all colony culture farms on map from buildings cache.
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			if (_farmDefs == null)
			{
				_farmDefs = new List<ThingDef>();
				foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
				{
					if (thingDef.comps.Any(x => x is CompProperties_CultureFarm))
						_farmDefs.Add(thingDef);
				}
			}

			// Find all current buildings matching a def that has a culture farm comp.
			_things.Clear();
			for (int i = _farmDefs.Count - 1; i >= 0; i--)
				_things.AddRange(pawn.Map.listerBuildings.AllBuildingsColonistOfDef(_farmDefs[i]));

			return _things;
		}

		// Check if pawn can extract culture from a culture farm.
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			CompCultureFarm farm = t.TryGetComp<CompCultureFarm>();

			if (farm == null)
				return false;

			if (farm.ShouldExtract == false)
				return false;

			return pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced);
		}

		// Assign job.
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(DefOfStrains.SC_ExtractCultureJob, t);
		}
	}
}
