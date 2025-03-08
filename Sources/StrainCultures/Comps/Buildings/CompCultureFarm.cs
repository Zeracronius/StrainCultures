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

namespace StrainCultures.Comps
{
	public abstract class CompProperties_CultureFarm : CompProperties
	{
		public CompProperties_CultureFarm()
		{
			compClass = typeof(CompCultureFarm);
		}

		public bool affectedByTemperature = false;
		public int culturePerGrowth = 5;
	}

	public abstract class CompCultureFarm : ThingComp
	{
		protected StrainCulture? _culture = null;

		public bool AllowManualExtracting = true;
		public float ExtractAtFilled = 0.8f; // Extract at 80% filled.
		public Job? InsertJob;

		private float _progressToCulture = 0;

		private Gizmo? _selectCultureGizmo;
		private Gizmo? _allowManualExtracting;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

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

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Culture: " + (_culture == null ? "Empty" : _culture.LabelCapNoCount));
			stringBuilder.AppendLine("Filled: " + Filled.ToStringPercent());
			stringBuilder.AppendLine("Temperature growth multiplier: " + GetTemperatureGrowthMultiplier().ToStringPercent());
			return stringBuilder.ToString().TrimEndNewlines();
		}

		protected float GetTemperatureGrowthMultiplier()
		{
			return _culture?.growthTemperatureMultiplier.Evaluate(parent.AmbientTemperature) ?? 0;
		}

		protected virtual bool CanGrow()
		{
			// No culture
			if (_culture == null)
				return false;

			// Full
			if (_culture.stackCount == _culture.def.stackLimit)
				return false;

			CompProperties_PrimitiveCultureFarm properties = (CompProperties_PrimitiveCultureFarm)props;
			if (properties.affectedByTemperature)
			{
				if (GetTemperatureGrowthMultiplier() <= 0)
					return false;
			}

			return true;
		}

		protected virtual void OnCultureGrown()
		{
		}

		/// <summary>
		/// Triggered every 250 ticks. See <see cref="Utilities.TimeMetrics.TICKS_RARE"/>.
		/// </summary>
		public override void CompTickRare()
		{
			base.CompTickRare();
			if (CanGrow() == false)
				return;

			if (_culture != null)
			{
				_progressToCulture += (int)(((CompProperties_PrimitiveCultureFarm)props).culturePerGrowth * GetTemperatureGrowthMultiplier());

				if (_progressToCulture > 1)
				{
					int newCulture = (int)_progressToCulture;
					_progressToCulture -= newCulture;
					_culture.stackCount += newCulture;
					OnCultureGrown();
				}
			}
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

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			yield return _selectCultureGizmo!;
			yield return _allowManualExtracting!;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Deep.Look(ref _culture, "_culture");
			Scribe_Values.Look(ref AllowManualExtracting, "allowManualExtracting");
			Scribe_Values.Look(ref ExtractAtFilled, "extractAtFilled");
			Scribe_Values.Look(ref _progressToCulture, "progressToCulture");
		}

		protected virtual void OnSelectCultureGizmo()
		{
			IEnumerable<StrainCulture> cultureThingsOnMap = parent.Map.listerThings.GetThingsOfType<StrainCulture>();

			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (StrainCulture culture in cultureThingsOnMap)
			{
				// Don't include option for inserting the strain already in the farm.
				if (_culture != null && culture.CanStackWith(_culture))
					continue;

				FloatMenuOption option = new FloatMenuOption(culture.LabelCapNoCount, () =>
				{
					InsertJob = JobMaker.MakeJob(DefOfStrains.SC_InsertCultureJob, culture, parent);
				});
				options.Add(option);
			}

			if (options.Count == 0)
				options.Add(new FloatMenuOption("No other cultures available", null));

			Find.WindowStack.Add(new FloatMenu(options));
		}
	}
}
