using RimWorld;
using StrainCultures.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace StrainCultures.Jobs
{
	internal class JobDriver_InsertCulture : JobDriver
	{
		private static int INSERT_DURATION = 200;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (TargetThingA is not Things.StrainCulture)
				return false;

			CompCultureFarm farm = TargetThingB.TryGetComp<CompCultureFarm>();
			if (farm == null)
				return false;

			// Reserve 1 strain culture sample
			if (!pawn.Reserve(TargetA, job, 1, 1, null, errorOnFailed))
				return false;

			job.count = 1;

			// Reserve culture farm building
			if (!pawn.Reserve(TargetB, job, 1, -1, null, errorOnFailed))
				return false;

			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A);

			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch)
				.FailOnDespawnedNullOrForbidden(TargetIndex.B);



			yield return Toils_General.Wait(INSERT_DURATION, TargetIndex.B)
				.WithProgressBarToilDelay(TargetIndex.B)
				.FailOnDespawnedNullOrForbidden(TargetIndex.B)
				.FailOnCannotTouch(TargetIndex.B, PathEndMode.Touch);

			Toil insertToil = ToilMaker.MakeToil();
			insertToil.initAction = () =>
			{
				CompCultureFarm farm = TargetThingB.TryGetComp<CompCultureFarm>();
				farm.EmptyCulture(pawn);
				farm.InsertCulture((Things.StrainCulture)pawn.carryTracker.CarriedThing);
			};

			yield return insertToil;
		}
	}
}
