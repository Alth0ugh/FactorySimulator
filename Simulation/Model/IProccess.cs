using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    /// <summary>
    /// Interface for simulation proccesses.
    /// </summary>
    public interface IProccess
    {
        /// <summary>
        /// Handles simulation events.
        /// </summary>
        /// <param name="ev">Simulation event.</param>
        void HandleEvent(Event ev);
        /// <summary>
        /// ID of the proccess.
        /// </summary>
        int ID { get; set; }
    }
}
