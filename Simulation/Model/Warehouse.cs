//#define PRINT
using Simulation.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    public class Warehouse : IProccess
    {
        private Scheduler _scheduler;
        public int ID { get; set; } = 0;

        public Warehouse(Scheduler scheduler, int id)
        {
            _scheduler = scheduler;
            ID = id;
        }
        public void HandleEvent(Event ev)
        {
            switch (ev.EventType)
            {
                case EventTypes.LineFeed:
                    {
                        var line = (Line)ev.Sender;
#if PRINT
                        Debug.WriteLine($"{_scheduler.Time}: Feeding line {line.Name}");
#endif
                        foreach (var proccess in line.ProccessesToFeed)
                        {
                            proccess.Feed(proccess.RequestPartCount.Value);
                        }
                        line.ProccessesToFeed.Clear();
                        _scheduler.Schedule(line, null, null, this, _scheduler.Time + line.FeedInterval.Value, EventTypes.LineFeed);
                    }
                    break;
                case EventTypes.LineUnload:
                    {
                        var line = (Line)ev.Sender;
#if PRINT
                        Debug.WriteLine($"{_scheduler.Time}: Unloading line {line.Name}");
#endif
                        line.UnloadLine();
                        _scheduler.Schedule(line, null, line, this, _scheduler.Time + line.UnloadInterval.Value, EventTypes.LineUnload); ;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
