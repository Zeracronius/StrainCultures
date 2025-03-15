using RimWorld;
using StrainCultures.Hediffs;
using StrainCultures.Mutations;
using StrainCultures.Outcomes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Things
{
	public class StrainCulture : ThingWithComps
	{
		private static int _nextID = 0;
		private int _id = 0;

		private string? _label;

		public StrainCulture()
			: base()
		{
		}


		/// <summary>
		/// Create a new variant of a given strain.
		/// </summary>
		/// <param name="strain"></param>
		public StrainCulture(StrainCulture strain)
			: base()
		{
			def = strain.def;
			_id = ++_nextID;
			Saturation = strain.Saturation;
			Mallability = strain.Mallability / 2f;
			Stability = strain.Stability;
			IncubationPeriodHours = strain.IncubationPeriodHours;
			FallOffHours = strain.FallOffHours;
			Potency = strain.Potency;
			PropagationChance = strain.PropagationChance;
			Influences = new Dictionary<string, float>(strain.Influences);

			PostMake();
			PostPostMake();
		}

		public SimpleCurve? growthTemperatureMultiplier => (def as StrainCultureDef)?.growthTemperatureMultiplier;

		public float Saturation = 200;

		/// <summary>
		/// value used to interpolate resulting influences when extracted from a pawn.
		/// </summary>
		public float Mallability = 2f;

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
		public int FallOffHours = 12;

		/// <summary>
		/// How likely the virus is to be resisted, and the speed effects are applied.
		/// </summary>
		public float Potency = 1;

		/// <summary>
		/// How likely the virus is to reinfect the host with itself.
		/// </summary>
		public float PropagationChance = 0;

		/// <summary>
		/// Weight values for the association to a given tag.
		/// </summary>
		public Dictionary<string, float> Influences = new Dictionary<string, float>();


		public void ApplyInfluences(Thing thing)
		{
			string defName = thing.def.defName;
			float value = 0;
			int influencesCount = Influences.Count;
			if (Influences.TryGetValue(defName, out value))
			{
				// If current influence already exist, exclude from count.
				influencesCount -= 1;
			}

			float newValue = UnityEngine.Mathf.Lerp(value, 1f, Mallability);
			float delta = newValue - value;
			float averagedDelta = delta / influencesCount;

			foreach (var influence in Influences)
			{
				if (influence.Key == defName)
				{
					Influences[defName] = newValue;
				}
				else
				{
					Influences[influence.Key] -= averagedDelta;
				}
			}

			// Sanity check only run in debug mode.
			Debug.Assert(Influences.Values.Sum() == 1f);
		}

		public void ApplyInfluences(Mutation mutation)
		{
			//TODO apply mutation influences logic here.
		}


		public override bool CanStackWith(Thing other)
		{
			// Prevent stacking with items of a different strain.
			if (_id != (other as StrainCulture)?._id)
				return false;

			return base.CanStackWith(other);
		}

		public override string LabelNoCount
		{
			get
			{
				if (_label == null)
				{
					_label = GenLabel.ThingLabel(this, 1);
					if (_id > 0)
						_label += " (Strain " + _id + ")";
				}
				return _label;
			}
		}

		public override TipSignal GetTooltip()
		{
			return base.GetTooltip();

			//TODO Add descriptive tooltip for strain culture details.
		}

		public override void ExposeData()
		{
			base.ExposeData(); 
			// While this is static it should be identical for all instances and not matter.
			Scribe_Values.Look(ref _nextID, "_nextID");


			Scribe_Values.Look(ref _id, "_id");
			Scribe_Values.Look(ref Stability, "stability");
			Scribe_Values.Look(ref IncubationPeriodHours, "incubationPeriodHours");
			Scribe_Values.Look(ref FallOffHours, "fallOffHours");
			Scribe_Values.Look(ref Potency, "potency");
			Scribe_Values.Look(ref PropagationChance, "propagationChance");
			Scribe_Collections.Look(ref Influences, "tagAssociations", LookMode.Value, LookMode.Value);
		}

		public bool TriggerOutcome(Pawn target, Infection infection, Mutated? mutated)
		{
			var strainDef = def as StrainCultureDef;
			if (strainDef != null)
			{
				int count = strainDef.outcomeWorkers.Count;
				List<IOutcomeWorker> outcomes = strainDef.outcomes;
				for (int i = 0; i < count; i++)
				{
					if (outcomes[i].ApplyOutcome(target, infection, mutated))
						return true;
				}
			}
			return false;
		}

	}
}
