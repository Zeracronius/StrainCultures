using StrainCultures.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Outcomes
{
	public interface IOutcomeWorker
	{
		bool ApplyOutcome(Pawn target, Infection infection, Mutated? mutated);
	}
}
