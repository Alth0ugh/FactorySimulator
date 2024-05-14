using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    public enum EventTypes
    {
        ProccessReady,
        ProccessFinished,
        NoMaterial,
        GoodsTransition,
        LineFeed,
        LineUnload,
        GenerateGoods
    }
}
