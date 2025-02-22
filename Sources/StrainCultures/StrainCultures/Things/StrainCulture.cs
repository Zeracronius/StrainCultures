using StrainCultures.Outcomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Things
{
	public class StrainCulture : Thing
	{
		/// <summary>
		/// Stability ratio. positive is stable, negative is unstable.
		/// </summary>
		public float Stability = 0f;

		/// <summary>
		/// Time before the onset of the virus in hours.
		/// </summary>
		public int IncubationPeriodHours = 0;

		/// <summary>
		/// Time it takes for all effects to be applied and the virus to go inert.
		/// </summary>
		public int FallOffHours = 1;

		/// <summary>
		/// How likely the virus is to be resisted, and the speed effects are applied.
		/// </summary>
		public float Potency = 0;

		/// <summary>
		/// How likely the virus is to reinfect the host with itself.
		/// </summary>
		public float PropagationChance = 0;

		/// <summary>
		/// Collection of outcomes caused by this infection.
		/// </summary>
		public List<IOutcome> Outcomes = new List<IOutcome>();

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Values.Look(ref Stability, "stability");
			Scribe_Values.Look(ref IncubationPeriodHours, "incubationPeriodHours");
			Scribe_Values.Look(ref FallOffHours, "fallOffHours");
			Scribe_Values.Look(ref Potency, "potency");
			Scribe_Values.Look(ref PropagationChance, "propagationChance");
			Scribe_Collections.Look(ref Outcomes, "outcomes", LookMode.Deep);
		}
	}
}
