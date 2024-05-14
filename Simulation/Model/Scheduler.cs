using Simulation.DataStructures;
using Simulation.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    /// <summary>
    /// Schedules events for simulation.
    /// </summary>
    public class Scheduler
    {
        /// <summary>
        /// Collection of lines.
        /// </summary>
        public IEnumerable<Line> Lines { get; set; }
        /// <summary>
        /// Reference to warehouse.
        /// </summary>
        public Warehouse Warehouse { get; set; }
        /// <summary>
        /// Current time of the simulation.
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// Simulation events saved in heaps for each line.
        /// </summary>
        public List<Heap> Events { get; set; }
        /// <summary>
        /// Minimum events for every line.
        /// </summary>
        public Heap CurrentEvents { get; set; }
        /// <summary>
        /// Generator for goods.
        /// </summary>
        public GoodsGenerator Generator { get; set; } = new GoodsGenerator();

        /// <summary>
        /// Schedules event.
        /// </summary>
        /// <param name="sender">Sending proccess.</param>
        /// <param name="argumentSender">Sender argument.</param>
        /// <param name="argumentTarget">Argument for target.</param>
        /// <param name="target">Target proccess.</param>
        /// <param name="time">Time of desired event execution.</param>
        /// <param name="eventType">Type of event.</param>
        public void Schedule(IProccess sender, object argumentSender, object argumentTarget, IProccess target, int time, EventTypes eventType)
        {
            var id = target is Warehouse ? sender.ID : target.ID;
            Events[id].Add(new Event() { Sender = sender, ArgumentSender = argumentSender, ArgumentTarget = argumentTarget, EventType = eventType, Target = target, Time = time });
        }

        /// <summary>
        /// Starts simulation.
        /// </summary>
        /// <param name="simulationDuration">Simulation duration.</param>
        public void StartSimulation(int simulationDuration)
        {
            CreateEventHeaps();
            CurrentEvents = new Heap(Lines.Count());
            Warehouse = new Warehouse(this, Lines.Count());
            Event currentEvent = null;
            Generator.Reset();
            
            if (Lines.Count() == 0)
            {
                throw new InvalidOperationException("V simulácii nie je ani jedna linka");
            }
            
            foreach (var line in Lines)
            {
                line.Start();

                Schedule(line, null, null, Warehouse, Time + line.UnloadInterval.Value, EventTypes.LineUnload);
                Schedule(line, null, null, Warehouse, Time + line.FeedInterval.Value, EventTypes.LineFeed);

                if (line.Proccesses.Count > 0)
                {
                    Schedule(line, line.Proccesses[0], line.Proccesses[0], line, Time, EventTypes.GenerateGoods);
                }
            }
            
            foreach (var heap in Events)
            {
                var ev = heap.Peak();
                if (ev != null)
                    CurrentEvents.Add(ev);
            }

            while (Time <= simulationDuration && CurrentEvents.Count > 0)
            {
                currentEvent = (Event)CurrentEvents.GetMinimum();

                if (currentEvent == null)
                {
                    return;
                }
                var id = currentEvent.Target is Warehouse ? currentEvent.Sender.ID : currentEvent.Target.ID;
                //Definately removing selected event from the heap
                currentEvent = (Event)Events[id].GetMinimum();
                Time = currentEvent.Time;
                if (Time <= simulationDuration)
                    currentEvent.Target.HandleEvent(currentEvent);
                CurrentEvents.Add(Events[id].Peak());
            }
        }

        private void CreateEventHeaps()
        {
            Events = new List<Heap>();
            foreach (var line in Lines)
            {
                Events.Add(new Heap((line.Proccesses.Count * 2) + 2));
            }
        }
    }
}
