using RimWorld;
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
		// Get all colony culture farms on map from buildings cache.
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Buildings.CultureFarm>();
		}

		// Check if pawn can extract culture from a culture farm.
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t is not Buildings.CultureFarm farm)
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
