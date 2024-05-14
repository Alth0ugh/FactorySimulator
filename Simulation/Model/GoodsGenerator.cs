using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    /// <summary>
    /// Generates new goods with unique IDs.
    /// </summary>
    public class GoodsGenerator
    {
        private int _counter = 0;

        /// <summary>
        /// Generates new goods.
        /// </summary>
        /// <returns>Generated goods.</returns>
        public Goods GenerateGoods()
        {
            return new Goods(_counter++);
        }

        /// <summary>
        /// Resets generating counter.
        /// </summary>
        public void Reset()
        {
            _counter = 0;
        }
    }
}
