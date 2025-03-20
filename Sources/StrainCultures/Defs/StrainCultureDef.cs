using StrainCultures.Outcomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures
{
	public class StrainCultureDef : ThingDef
	{
		public List<Type> outcomeWorkers = new List<Type>();

		public List<IOutcomeWorker> outcomes = new List<IOutcomeWorker>();

		public SimpleCurve? growthTemperatureMultiplier;

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			ParseOutcomeWorkers();
		}

		private void ParseOutcomeWorkers()
		{
			for (int i = 0; i < outcomeWorkers.Count; i++)
			{
				IOutcomeWorker? outcome = Activator.CreateInstance(outcomeWorkers[i]) as IOutcomeWorker;
				if (outcome != null)
					outcomes.Add(outcome);
			}
		}
	}
}
