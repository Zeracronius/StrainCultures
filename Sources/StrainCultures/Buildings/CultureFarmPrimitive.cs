﻿using RimWorld;
using StrainCultures.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Buildings
{
	public class CultureFarmPrimitive : CultureFarm
	{
		const int TICK_PER_CULTURE = 50;
		const float FUEL_PER_CULTURE = 0.1f;
		const float FUEL_PER_TICK = FUEL_PER_CULTURE / TICK_PER_CULTURE;

		CompRefuelable? _fuelComp;
		int _ticksToCulture = TICK_PER_CULTURE;
		

		public CultureFarmPrimitive()
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
		}

		public override void CompTick()
		{
			base.CompTick();
			if (_fuelComp != null && _culture != null)
			{
				if (_culture.stackCount == _culture.def.stackLimit)
					return;

				if (_fuelComp.Fuel > FUEL_PER_TICK)
				{
					_fuelComp.ConsumeFuel(FUEL_PER_TICK);
					_ticksToCulture--;
				}
				
				if (_ticksToCulture <= 0)
				{
					_culture.stackCount += 1;
					_ticksToCulture = TICK_PER_CULTURE;
				}
			}
		}
	}
}
