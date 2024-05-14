//#define PRINT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using Simulation.UserControls;

namespace Simulation.Model
{
    /// <summary>
    /// Object used to represent production line in simulation.
    /// </summary>
    public class Line : IProccess, INotifyPropertyChanged
    {
        private int _id;
        /// <summary>
        /// ID of the line.
        /// </summary>
        public int ID 
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged("ID");
            }
        }
        /// <summary>
        /// Counter of scraps found on the line.
        /// </summary>
        public int ScrapCount { get; set; }
        /// <summary>
        /// List of proccesses on the line in chronological order.
        /// </summary>
        public List<LineProccess> Proccesses { get; set; } = new List<LineProccess>();
        /// <summary>
        /// List of proccesses that reported NoMaterial event.
        /// </summary>
        public List<LineProccess> ProccessesToFeed { get; set; } = new List<LineProccess>();
        private StepperValue _finishedCapacity = new StepperValue() { From = 1 };
        /// <summary>
        /// Capacity of finished goods.
        /// </summary>
        public StepperValue FinishedCapacity 
        {
            get => _finishedCapacity;
            set
            {
                _finishedCapacity = value;
                OnPropertyChanged("FinishedCapacity");
            }
        }
        /// <summary>
        /// Current count of finished goods.
        /// </summary>
        public int CurrentFinishedGoods { get; set; } = 0;
        private StepperValue _unloadInterval = new StepperValue() { From = 1 };
        /// <summary>
        /// Interval at which the line should be unloaded.
        /// </summary>
        public StepperValue UnloadInterval { 
            get => _unloadInterval;
            set
            {
                _unloadInterval = value;
                OnPropertyChanged("UnloadInterval");
            }
        }
        private StepperValue _feedInterval = new StepperValue() { From = 1 };
        /// <summary>
        /// Interval at which the line should be fed.
        /// </summary>
        public StepperValue FeedInterval 
        {
            get => _feedInterval;
            set
            {
                _feedInterval = value;
                OnPropertyChanged("FeedInterval");
            }
        }
        private string _name = "";
        /// <summary>
        /// Name of the line
        /// </summary>
        public string Name 
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// There is a counter on every index saying how many times the warehouse unloaded that amount of goods from line.
        /// </summary>
        public int[] UnloadValues { get; set; }
        /// <summary>
        /// Total count of finished goods from the line.
        /// </summary>
        public int TotalFinishedGoods { get; set; } = 0;
        /// <summary>
        /// Tile from GUI, needed for deleting lines and proccesses.
        /// </summary>
        public UCTile Tile { get; set; }
        private Scheduler _scheduler;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates new instance of Line.
        /// </summary>
        /// <param name="scheduler">Reference to scheduler.</param>
        public Line(Scheduler scheduler)
        {
            _scheduler = scheduler;
        }

        /// <summary>
        /// Initalizes line properties.
        /// </summary>
        public void Initialize()
        {
            ProccessesToFeed.Clear();
            CurrentFinishedGoods = 0;
            UnloadValues = null;
            TotalFinishedGoods = 0;
            ScrapCount = 0;
        }

        /// <summary>
        /// Initializes the line for work.
        /// </summary>
        public void Start()
        {
            UnloadValues = new int[FinishedCapacity.Value + 1];
        }

        /// <summary>
        /// Handles event from scheduler.
        /// </summary>
        /// <param name="ev">Event from scheduler.</param>
        public void HandleEvent(Event ev)
        {
            if (ev == null || ev.ArgumentSender == null || !(ev.ArgumentSender is LineProccess) || _scheduler == null || ev.Target == null)
            {
                return;
            }

            var proccess = (LineProccess)ev.ArgumentSender;
            var target = (LineProccess)ev.ArgumentTarget;
            switch (ev.EventType)
            {
                case EventTypes.GoodsTransition:
                    {
                        //If the last procces on the line wants to transition goods
                        if (proccess.ID + 1 >= Proccesses.Count)
                        {
                            if (CurrentFinishedGoods + 1 > FinishedCapacity.Value)
                            {
                                proccess.IsHalted = true;
                                proccess.DownTimeStart = _scheduler.Time;
#if PRINT
                                Debug.WriteLine($"{_scheduler.Time}: halting proccess {proccess.ID}");
#endif
                            }
                            else
                            {
                                TotalFinishedGoods++;
                                CurrentFinishedGoods++;
                                proccess.EndWork();
#if PRINT
                                Debug.WriteLine($"{_scheduler.Time}: Proccess {proccess.ID} finished work ({TotalFinishedGoods})");
#endif
                            }
                        }
                        else
                        {
                            if (Proccesses[proccess.ID + 1].CurrentGoods == null)
                            {
#if PRINT
                                Debug.WriteLine($"{_scheduler.Time}: Transition from {proccess.ID} to {Proccesses[proccess.ID + 1].ID}");
#endif
                                Proccesses[proccess.ID + 1].StartWork(proccess.EndWork());
                                if (proccess.ID == 0)
                                {
                                    _scheduler.Schedule(this, proccess, proccess, this, _scheduler.Time, EventTypes.GenerateGoods);
                                }
                            }
                            else
                            {
#if PRINT
                                Debug.WriteLine($"{_scheduler.Time}: halting proccess {proccess.ID}");
#endif
                                proccess.IsHalted = true;
                                proccess.DownTimeStart = _scheduler.Time;
                            }
                        }

                        if (proccess.ID > 0 && proccess.CurrentGoods == null)
                        {
                            _scheduler.Schedule(this, proccess, Proccesses[proccess.ID - 1], this, _scheduler.Time, EventTypes.ProccessReady);
                        }
                    }
                    break;
                case EventTypes.ProccessFinished:
                    {
                        //Scrap goods do not continue the production proccess
                        if (proccess.CurrentGoods.IsScrap)
                        {
#if PRINT
                            Debug.WriteLine($"{_scheduler.Time}: Scrap on proccess {proccess.ID} detected");
#endif
                            ScrapCount++;
                            proccess.EndWork();
                            //Signaling to previous proccesses that the current proccess is empty and ready for work
                            if (proccess.ID - 1 >= 0)
                            {
                                _scheduler.Schedule(this, proccess, Proccesses[proccess.ID - 1], this, _scheduler.Time, EventTypes.ProccessReady);
                            }
                            else
                            {
                                _scheduler.Schedule(this, proccess, proccess, this, _scheduler.Time, EventTypes.GenerateGoods);
                            }
                        }
                        else
                        {
                            _scheduler.Schedule(this, proccess, proccess, this, _scheduler.Time + proccess.TransitionTime,EventTypes.GoodsTransition);
                        }
                    }
                    break;
                case EventTypes.ProccessReady:
                    {
                        if (target.IsHalted && target.IsFinished)
                        {
#if PRINT
                            Debug.WriteLine($"{_scheduler.Time}: Transition from {target.ID} to {proccess.ID}");
#endif
                            var goods = target.EndWork();
                            proccess.StartWork(goods);
                            target.IsHalted = false;
                            target.Downtime += _scheduler.Time - target.DownTimeStart;
                            if (target.ID > 0)
                            {
                                _scheduler.Schedule(this, target, Proccesses[target.ID - 1], this, _scheduler.Time, EventTypes.ProccessReady);
                            }
                            else
                            {
                                _scheduler.Schedule(this, Proccesses[0], Proccesses[0], this, _scheduler.Time, EventTypes.GenerateGoods);
                            }
                        }
                    }
                    break;
                case EventTypes.GenerateGoods:
                    {
#if PRINT
                        Debug.WriteLine($"{_scheduler.Time}: Generating new goods on proccess {target.ID}");
#endif
                        var goods = _scheduler.Generator.GenerateGoods();
                        target.StartWork(goods);
                    }
                    break;
                case EventTypes.NoMaterial:
                    ProccessesToFeed.Add(target);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Unloads a line and resumes work on the line.
        /// </summary>
        public void UnloadLine()
        {
            UnloadValues[CurrentFinishedGoods]++;
            CurrentFinishedGoods = 0;
            if (Proccesses.Count > 1 && Proccesses[Proccesses.Count - 1].IsHalted && Proccesses[Proccesses.Count - 1].IsFinished)
            {
                Proccesses[Proccesses.Count - 1].IsHalted = false;
                Proccesses[Proccesses.Count - 1].Downtime += _scheduler.Time - Proccesses[Proccesses.Count - 1].DownTimeStart;
                Proccesses[Proccesses.Count - 1].EndWork();
                TotalFinishedGoods++;
                CurrentFinishedGoods++;
#if PRINT
                Debug.WriteLine($"{_scheduler.Time}: Proccess {Proccesses[Proccesses.Count - 1].ID} finished work ({TotalFinishedGoods})");
#endif
                _scheduler.Schedule(this, Proccesses[Proccesses.Count - 1], Proccesses[Proccesses.Count - 2], this, _scheduler.Time, EventTypes.ProccessReady);
            }
        }

        /// <summary>
        /// Deletes line proccesses and the line.
        /// </summary>
        /// <returns>True if the line or any of its proccesses had an active stepper, otherwise false.</returns>
        public bool Delete()
        {
            var proccessDelete = false;
            foreach (var proccess in Proccesses)
            {
                proccessDelete |= proccess.Delete();
            }
            return Tile?.Delete() | proccessDelete ?? proccessDelete;
        }

        public void OnPropertyChanged(string sender)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }
}
