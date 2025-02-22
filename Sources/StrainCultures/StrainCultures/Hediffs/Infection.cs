using StrainCultures.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Hediffs
{
	internal class Infection : Hediff
	{
		private StrainCulture _strain;

#pragma warning disable CS8618
		public Infection()
		{
			
		}
#pragma warning restore CS8618

		public Infection(StrainCulture strainCulture)
		{
			_strain = strainCulture;
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Deep.Look(ref _strain, "strain");
		}
	}
}
