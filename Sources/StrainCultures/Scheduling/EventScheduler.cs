using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Scheduling
{
	internal class EventScheduler : GameComponent
	{
		private static EventScheduler? _instance;
		private static TickManager? _tickManager;

		public class Event : IExposable
		{
			public void Assign(IEventHandler handler, string? signal)
			{
				Handler = handler;
				Signal = signal;
			}

			public void Clear()
			{
				Handler = null!;
				Signal = null;
			}

			public IEventHandler? Handler;
			public string? Signal;

			public void ExposeData()
			{
				Scribe_References.Look(ref Handler, "handler");
				Scribe_Values.Look(ref Signal, "signal");
			}
		}

		private Stack<Event> _recycleEvents = new Stack<Event>(100);
		private Stack<Stack<Event>> _recycleBin = new Stack<Stack<Event>>(100);
		private Dictionary<int, Stack<Event>> _scheduledEvents = new Dictionary<int, Stack<Event>>();
		private int _ticksToNextEvent = -1;
		private int _lastTick = -1;

		public Game game;

		public EventScheduler(Game game)
		{
			this.game = game;
			_instance = this;
			_tickManager = game.tickManager;
		}

		public override void GameComponentTick()
		{
			base.GameComponentTick();

			int currentTick = _tickManager!.TicksGame;

			if (_lastTick == -1)
				_lastTick = currentTick;

			// Run the current tick and the ticks that have passed since the last tick.
			for (int i = _lastTick + 1; i <= currentTick; i++)
				TickSchedule(i);

			_lastTick = currentTick;
		}

		private void TickSchedule(int currentTick)
		{
			// Negative means no events scheduled.
			if (_ticksToNextEvent < 0)
				return;

			_ticksToNextEvent--;

			if (_ticksToNextEvent == 0)
			{
				Mod.Logging.Message($"[{currentTick}] Raising events!");
				if (_scheduledEvents.TryGetValue(currentTick, out Stack<Event> events))
				{
					while (events.TryPop(out Event currentEvent))
					{
						currentEvent.Handler?.HandleEvent(currentEvent.Signal);
						currentEvent.Clear();
						_recycleEvents.Push(currentEvent);
					}

					_scheduledEvents.Remove(currentTick);
					_recycleBin.Push(events);
				}

				ScheduleNextEvent(currentTick);
			}
		}

		/// <summary>
		/// Set ticks to event to the number of ticks from now to the next event.
		/// </summary>
		/// <param name="currentTick"></param>
		private void ScheduleNextEvent(int currentTick)
		{
			if (_scheduledEvents.Count == 0)
				_ticksToNextEvent = -1; // No future events scheduled.
			else
			{
				int nextEventTick = _scheduledEvents.Keys.Min();
				int ticksToEvent = nextEventTick - currentTick;
				if (ticksToEvent > 0)
					_ticksToNextEvent = ticksToEvent;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Dictionary<int, List<Event>> keyValuePairs = _scheduledEvents.ToDictionary(x => x.Key, x => x.Value.ToList());
			Scribe_Collections.Look(ref keyValuePairs, "scheduledEvents", LookMode.Value, LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				_scheduledEvents = keyValuePairs.ToDictionary(x => x.Key, x => new Stack<Event>(x.Value));
				ScheduleNextEvent(Find.TickManager!.TicksGame + 1);
				Mod.Logging.Message($"[{Find.TickManager!.TicksGame}]: Scheduler loaded events for ticks: {String.Join(",", _scheduledEvents.Keys)}. Time until next event: {_ticksToNextEvent}");
			}

		}

		public static void QueueEvent(int ticksFromNow, IEventHandler handler, string? signal = null)
		{
			if (_instance == null || _tickManager == null)
			{
				Mod.Logging.Error("EventScheduler has null");
				return;
			}

			if (_instance._recycleEvents.TryPop(out Event newEvent) == false)
				newEvent = new Event();

			newEvent.Assign(handler, signal);
			_instance.QueueEvent(ticksFromNow, newEvent);
		}

		private void QueueEvent(int ticksFromNow, Event eventEntry)
		{
			int currentTick = _tickManager!.TicksGame;
			int targetTick = currentTick + ticksFromNow;
			if (_scheduledEvents.TryGetValue(targetTick, out Stack<Event> events) == false)
			{
				if (_recycleBin.Count > 0)
					events = _recycleBin.Pop();
				else
					events = new Stack<Event>();

				_scheduledEvents.Add(targetTick, events);
			}
			events.Push(eventEntry);

			// Update countdown until next tick if needed.
			if (_ticksToNextEvent < 0 || ticksFromNow < _ticksToNextEvent)
				_ticksToNextEvent = ticksFromNow;

			Mod.Logging.Message("Event scheduled in " + ticksFromNow + " ticks");
		}
	}
}
