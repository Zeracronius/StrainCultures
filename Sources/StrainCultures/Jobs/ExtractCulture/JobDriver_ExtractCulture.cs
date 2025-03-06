﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace StrainCultures.Jobs
{
	internal class JobDriver_ExtractCulture : JobDriver
	{
		private static int EXTRACT_DURATION = 400;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (TargetThingA is not Buildings.CultureFarm)
				return false;

			if (((Buildings.CultureFarm)TargetThingA).HasCulture == false)
				return false;

			// Reserve culture farm building
			if (!pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed))
				return false;

			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnIncapable(PawnCapacityDefOf.Manipulation);

			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
				.FailOnDespawnedOrNull(TargetIndex.A);

			yield return Toils_General.Wait(EXTRACT_DURATION, TargetIndex.A)
				.WithProgressBarToilDelay(TargetIndex.A)
				.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
				.FailOnDespawnedOrNull(TargetIndex.A);

			Toil extractToil = ToilMaker.MakeToil();
			extractToil.initAction = () => ((Buildings.CultureFarm)TargetThingA).ExtractCulture(pawn);
			yield return extractToil;
		}
	}
}
