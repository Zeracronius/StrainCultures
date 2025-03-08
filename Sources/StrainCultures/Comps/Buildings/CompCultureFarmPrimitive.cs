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

		public int ticksPerCulture = 50;
		public float fuelPerCulture = 0.1f;
	}

	public class CompCultureFarmPrimitive : CompCultureFarm
	{
		CompProperties_PrimitiveCultureFarm? _cultureProperties;
		CompTemperatureRuinable? _temperatureComp;
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
			_temperatureComp = parent.GetComp<CompTemperatureRuinable>();

			_cultureProperties = (CompProperties_PrimitiveCultureFarm)props;

			_fuelPerTick = _cultureProperties.fuelPerCulture / _cultureProperties.ticksPerCulture;
			_ticksToCulture = _cultureProperties.ticksPerCulture;
		}

		public override void CompTick()
		{
			base.CompTick();
			// No culture
			if (_culture == null)
				return;

			// Full
			if (_culture.stackCount == _culture.def.stackLimit)
				return;

			// Has fuel comp.
			if (_fuelComp != null)
			{
				if (_fuelComp.Fuel > _fuelPerTick)
				{
					_fuelComp.ConsumeFuel(_fuelPerTick);
				}
				else
				{
					return;
				}
			}

			// Has temperature comp.
			if (_temperatureComp != null)
			{
				if (_temperatureComp.Ruined)
				{
					return;
				}
			}

			_ticksToCulture--;
			if (_ticksToCulture <= 0)
			{
				_culture.stackCount += 1;
				_ticksToCulture = _cultureProperties!.ticksPerCulture;
			}
		}
	}
}
