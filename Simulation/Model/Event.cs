using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    /// <summary>
    /// Simulation event.
    /// </summary>
    public class Event : IComparable
    {
        /// <summary>
        /// Sender proccess of event.
        /// </summary>
        public IProccess Sender { get; set; }
        /// <summary>
        /// Sender object of event.
        /// </summary>
        public object ArgumentSender { get; set; }
        /// <summary>
        /// Target object of event.
        /// </summary>
        public object ArgumentTarget { get; set; }
        /// <summary>
        /// Target proccess of event.
        /// </summary>
        public IProccess Target { get; set; }
        /// <summary>
        /// Time at which the event should be handled.
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// Type of event.
        /// </summary>
        public EventTypes EventType { get; set; }

        /// <summary>
        /// Compares 2 Event object.
        /// </summary>
        /// <param name="obj">Event object to compare to.</param>
        /// <returns>Value below 0 if the current instance predecesses the argument, 0 if argument and instance are identical, otherwise value above 0.</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is Event))
            {
                throw new InvalidOperationException($"Parameter obj is not of type {typeof(Event)}");
            }

            var ev = (Event)obj;
            var returnValue = 0;
            if (Time < ev.Time)
            {
                returnValue = -1;
            }
            else if (Time > ev.Time)
            {
                returnValue = 1;
            }
            return returnValue;
        }
    }
}
