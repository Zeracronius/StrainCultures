using StrainCultures.Defs;
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
	public class Infection : Hediff, Scheduling.IEventHandler
	{
		public enum InfectionState : byte
		{
			None = 0,
			Incubating = 1,
			Active = 2,
			Inert = 3
		}


		private StrainCulture _strain;
		private InfectionDef _infectionDef;

		private InfectionState _state = InfectionState.None;
		private string _stateLabel = "";
		private int _incubatingTicks = 0;

#pragma warning disable CS8618
		[Obsolete("Use ApplyInfection static method instead.", true)]
		public Infection()
		{
		}
#pragma warning restore CS8618

		public static Infection ApplyInfection(StrainCulture strainCulture, Pawn pawn, BodyPartRecord? partRecord = null)
		{
			Infection hediff = (Infection)HediffMaker.MakeHediff(DefOfStrains.SC_Infection, pawn, partRecord);
			hediff._strain = strainCulture;
			pawn.health.AddHediff(hediff);
			return hediff;
		}

		// Infection has been applied to a pawn.
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			_infectionDef = (InfectionDef)def;
			_incubatingTicks = _strain.IncubationPeriodHours * Utilities.TimeMetrics.TICKS_PER_HOUR;
			
			// If incubation period is actually 0, then skip directly to active.
			InfectionState newState = _strain.IncubationPeriodHours == 0 ? InfectionState.Active : InfectionState.Incubating;
			ChangeState(newState);
			Scheduling.EventScheduler.QueueEvent(GetTicksToNextEvent(), this);
		}

		/// <summary>
		/// Returns the number of ticks until the next event based on infection state.
		/// Incubating will return the incubation period in ticks.
		/// Active will return the minimum ticks between outcomes.
		/// Inert will return 0 to disable scheduling.
		/// </summary>
		/// <returns></returns>
		private int GetTicksToNextEvent()
		{
			switch (_state)
			{
				case InfectionState.Incubating:
					return _incubatingTicks;

				case InfectionState.Active:
					return (int)_infectionDef.minimumTicksBetweenOutcomeCurve.Evaluate(_strain.Potency);

				default:
					return 0;
			}
		}

		public override void Tick()
		{
			ageTicks++;
		}

		private void ChangeState(InfectionState newState)
		{
			_state = newState;
			_stateLabel = " (" + _state.ToString() + ")";
			OnInfectionStateChange();
		}

		protected virtual void OnInfectionStateChange()
		{
		}

		public override string Label => base.Label + _stateLabel;

		public override bool TryMergeWith(Hediff other)
		{
			// Don't merge infection hediffs for now.
			return false;
		}

		private void TryEvolveStrain()
		{
			//TODO Mutate the strain based on the stability of the strain.
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Deep.Look(ref _strain, true, "strain");
		}

		public void HandleEvent(string? signal)
		{
			Mod.Logging.Message("Health event");
			if (_state == InfectionState.Inert)
				return;

			// First event that happens will always be when infection is done incubating.
			if (_state == InfectionState.Incubating)
				ChangeState(InfectionState.Active);

			// Get falloff ticks every time since this can shift as strain evolves.
			int falloffTicks = _strain.FallOffHours * Utilities.TimeMetrics.TICKS_PER_HOUR;
			if (ageTicks > falloffTicks + _incubatingTicks)
			{
				// Change state to inert and disable scheduling.
				ChangeState(InfectionState.Inert);
				return;
			}

			int ticksToNextEvent = GetTicksToNextEvent();

			// If new or infection is younger than the total period of incubation + falloff, then trigger outcome.
			bool result = _strain.TriggerOutcome(pawn, this, pawn.health.hediffSet.GetFirstHediffOfDef(DefOfStrains.SC_Mutated) as Mutated);

			// If outcome was triggered, try to evolve the strain.
			if (result)
				TryEvolveStrain();

			// Schedule next event.
			Scheduling.EventScheduler.QueueEvent(GetTicksToNextEvent(), this);
		}
	}
}
