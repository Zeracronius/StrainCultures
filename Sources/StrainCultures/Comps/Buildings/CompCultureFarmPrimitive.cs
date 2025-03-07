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

		public int TicksPerCulture = 50;
		public float FuelPerCulture = 0.1f;
	}

	public class CompCultureFarmPrimitive : CompCultureFarm
	{
		CompProperties_PrimitiveCultureFarm? _cultureProperties;
		CompRefuelable? _fuelComp;
		float _fuelPerTick = 0;
		int _ticksToCulture = 0;

		public CompCultureFarmPrimitive()
		{

		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref _ticksToCulture, "ticksToCulture");
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			_fuelComp = parent.GetComp<CompRefuelable>();

			_cultureProperties = (CompProperties_PrimitiveCultureFarm)props;

			_fuelPerTick = _cultureProperties.FuelPerCulture / _cultureProperties.TicksPerCulture;
			_ticksToCulture = _cultureProperties.TicksPerCulture;
		}

		public override void CompTick()
		{
			base.CompTick();
			if (_fuelComp != null && _culture != null)
			{
				if (_culture.stackCount == _culture.def.stackLimit)
					return;

				if (_fuelComp.Fuel > _fuelPerTick)
				{
					_fuelComp.ConsumeFuel(_fuelPerTick);
					_ticksToCulture--;
				}

				if (_ticksToCulture <= 0)
				{
					_culture.stackCount += 1;
					_ticksToCulture = _cultureProperties!.TicksPerCulture;
				}
			}
		}
	}
}
