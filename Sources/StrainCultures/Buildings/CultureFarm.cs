using StrainCultures.Defs;
using StrainCultures.Mod;
using StrainCultures.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace StrainCultures.Buildings
{
	public abstract class CultureFarm : Building
	{
		protected StrainCulture? _culture = null;

		public bool AllowManualExtracting = true;
		public float ExtractAtFilled = 0.8f; // Extract at 80% filled.

		private Gizmo? _selectCultureGizmo;
		private Gizmo? _allowManualExtracting;
		public Job? InsertJob;

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			_selectCultureGizmo = new Command_Action()
			{
				defaultLabel = "Select culture",
				defaultDesc = "Select culture",
				icon = RimWorld.TexCommand.Replant,
				action = OnSelectCultureGizmo
			};

			_allowManualExtracting = new Command_Toggle()
			{
				defaultLabel = "Allow extracting",
				defaultDesc = "Allow extracting",
				icon = RimWorld.TexCommand.ForbidOff,
				isActive = () => AllowManualExtracting,
				toggleAction = () => AllowManualExtracting = !AllowManualExtracting
			};
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetInspectString());
			stringBuilder.AppendLine("Culture: " + (_culture == null ? "Empty" : _culture.LabelCapNoCount));
			stringBuilder.AppendLine("Filled: " + Filled.ToStringPercent());
			return stringBuilder.ToString().TrimEndNewlines();
		}


		/// <summary>
		/// Inserts a culture sample into the farm if currently empty.
		/// </summary>
		/// <param name="strainCulture"></param>
		public void InsertCulture(StrainCulture strainCulture)
		{
			if (_culture != null)
				return;

			_culture = strainCulture;
			_culture.Destroy(DestroyMode.Vanish);
			_culture.ForceSetStateToUnspawned();
		}

		/// <summary>
		/// Extracts all but 1 culture from the farm and drops it near the provided pawn.
		/// </summary>
		/// <param name="pawn"></param>
		public void ExtractCulture(Pawn pawn)
		{
			if (_culture == null)
				return;

			StrainCulture newCulture = (StrainCulture)_culture.SplitOff(_culture.stackCount - 1);
			GenPlace.TryPlaceThing(newCulture, pawn.Position, pawn.Map, ThingPlaceMode.Near);
		}

		/// <summary>
		/// Extracts all culture completely, leaving it empty and ready for different culture.
		/// </summary>
		/// <param name="pawn"></param>
		public void EmptyCulture(Pawn pawn)
		{
			if (_culture == null)
				return;

			GenPlace.TryPlaceThing(_culture, pawn.Position, pawn.Map, ThingPlaceMode.Near);
			_culture = null;
		}

		public bool HasCulture
		{
			get
			{
				return _culture != null;
			}
		}

		//TODO optimise
		public float Filled
		{
			get
			{
				if (_culture == null)
					return 0;

				return (float)_culture.stackCount / _culture.def.stackLimit;
			}
		}

		//TODO optimise
		public bool ShouldExtract
		{
			get
			{
				return Filled >= ExtractAtFilled;
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var item in base.GetGizmos())
			{
				yield return item;
			}
			yield return _selectCultureGizmo!;
			yield return _allowManualExtracting!;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref _culture, "_culture");
			Scribe_Values.Look(ref AllowManualExtracting, "allowManualExtracting");
			Scribe_Values.Look(ref ExtractAtFilled, "extractAtFilled");
		}

		protected virtual void OnSelectCultureGizmo()
		{
			IEnumerable<StrainCulture> cultureThingsOnMap = Map.listerThings.GetThingsOfType<StrainCulture>();

			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (StrainCulture culture in cultureThingsOnMap)
			{
				// Don't include option for inserting the strain already in the farm.
				if (_culture != null && culture.CanStackWith(_culture))
					continue;

				FloatMenuOption option = new FloatMenuOption(culture.LabelCapNoCount, () =>
				{
					InsertJob = JobMaker.MakeJob(DefOfStrains.SC_InsertCultureJob, culture, this);
				});
				options.Add(option);
			}

			if (options.Count == 0)
				options.Add(new FloatMenuOption("No other cultures available", null));

			Find.WindowStack.Add(new FloatMenu(options));
		}
	}
}
