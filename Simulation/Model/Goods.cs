using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    /// <summary>
    /// Object used to describe goods in production in simulation.
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// True if the goods is a scrapt, otherwise false.
        /// </summary>
        public bool IsScrap { get; set; }
        /// <summary>
        /// ID of goods.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Creates new instance of Goods.
        /// </summary>
        /// <param name="id">ID of goods.</param>
        public Goods(int id)
        {
            ID = id;
        }
    }
}
