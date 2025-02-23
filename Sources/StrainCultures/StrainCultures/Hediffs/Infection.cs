using StrainCultures.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Hediffs
{
	/// <summary>
	/// Infection caused by being injected with strain culture.
	/// </summary>
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

		// Infection has been applied to a pawn.
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			MutateStrain();
		}

		private void MutateStrain()
		{
			//TODO Mutate the strain based on the stability of the strain.
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Deep.Look(ref _strain, "strain");
		}
	}
}
