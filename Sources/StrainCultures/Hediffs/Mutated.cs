using RimWorld;
using StrainCultures.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Hediffs
{
	/// <summary>
	/// Contains and handles all the mutations applied to a pawn, and is responsible for drawing them.
	/// </summary>
	public class Mutated : Hediff
	{
		private List<Mutation> _mutations = new();

		public Mutated()
		{
			
		}

		

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Collections.Look(ref _mutations, "mutations", LookMode.Deep);
		}

		internal float GetStatFactor(StatDef stat)
		{
			throw new NotImplementedException();
		}

		internal float GetStatOffset(StatDef stat)
		{
			throw new NotImplementedException();
		}
	}
}
