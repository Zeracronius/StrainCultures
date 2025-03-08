using RimWorld;
using StrainCultures.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Comps
{
	public class CompProperties_PrimitiveCultureFarm : CompProperties_CultureFarm
	{
		public CompProperties_PrimitiveCultureFarm()
		{
			compClass = typeof(CompCultureFarmPrimitive);
		}

		public float fuelPerGrowth = 0.1f;
	}

	public class CompCultureFarmPrimitive : CompCultureFarm
	{
		CompRefuelable? _fuelComp;

		public CompCultureFarmPrimitive()
		{

		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			_fuelComp = parent.GetComp<CompRefuelable>();
		}

		protected override bool CanGrow()
		{
			if (base.CanGrow() == false)
				return false;

			// Has fuel comp.
			float fuelPerGrowth = ((CompProperties_PrimitiveCultureFarm)props).fuelPerGrowth;
			if (_fuelComp?.Fuel < fuelPerGrowth)
				return false;

			return true;
		}

		protected override void OnCultureGrown()
		{
			float fuelPerGrowth = ((CompProperties_PrimitiveCultureFarm)props).fuelPerGrowth;
			_fuelComp?.ConsumeFuel(fuelPerGrowth);
		}
	}
}
