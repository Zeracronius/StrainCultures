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

		public struct Event
		{
			public Event(IEventHandler handler, string? signal)
			{
				Handler = handler;
				Signal = signal;
			}

			public IEventHandler Handler;
			public string? Signal;
		}

		private Stack<Stack<Event>> _recycleBin = new Stack<Stack<Event>>(100);
		private Dictionary<int, Stack<Event>> _scheduledEvents = new Dictionary<int, Stack<Event>>();
		private int _ticksToNextEvent = -1;
		private int _lastTick = -1;

		public Game game;

		public EventScheduler(Game game)
		{
			this.game = game;
			_instance = this;
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
				if (_scheduledEvents.TryGetValue(currentTick, out Stack<Event> events))
				{
					while (events.TryPop(out Event currentEvent))
						currentEvent.Handler.HandleEvent(currentEvent.Signal);

					_scheduledEvents.Remove(currentTick);
					_recycleBin.Push(events);
				}

				// Set ticks to event to the number of ticks from now to the next event.

				if (_scheduledEvents.Count > 0)
				{
					int nextEventTick = _scheduledEvents.Keys.Min();
					int ticksToEvent = nextEventTick - currentTick;
					if (ticksToEvent > 0)
						_ticksToNextEvent = ticksToEvent;
				}
				else
					_ticksToNextEvent = -1; // No future events scheduled.
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref _scheduledEvents, "scheduledEvents", LookMode.Value, LookMode.Value);
		}

		public override void LoadedGame()
		{
			_tickManager = Find.TickManager;
		}

		public static void QueueEvent(int ticksFromNow, IEventHandler handler, string? signal = null)
		{
			if (_instance == null || _tickManager == null)
			{
				Mod.Logging.Error("EventScheduler has null");
				return;
			}

			_instance.QueueEvent(ticksFromNow, new Event(handler, signal));
		}

		public void QueueEvent(int ticksFromNow, Event eventEntry)
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
