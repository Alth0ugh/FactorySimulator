//#define PRINT
using Simulation.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    /// <summary>
    /// Object that describes proccess on the line in simulaation.
    /// </summary>
    public class LineProccess : INotifyPropertyChanged
    {
        private int _id;
        /// <summary>
        /// Proccess ID.
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
        /// Goods the proccess is working on.
        /// </summary>
        public Goods CurrentGoods { get; private set; }
        /// <summary>
        /// Time that it takes to work on the goods.
        /// </summary>
        public int WorkingTime { get; set; }
        /// <summary>
        /// Time that it takes to transition goods into another proccess.
        /// </summary>
        public int TransitionTime { get; set; } = 1;
        public StepperValue InitialPartCount { get; set; } = new StepperValue();
        public bool IsHalted { get; set; }
        /// <summary>
        /// Frequency of scrap goods.
        /// </summary>
        public int ErrorRate { get; set; }
        /// <summary>
        /// Total count of goods that passed through the proccess (counting scraps).
        /// </summary>
        public int ProducedGoods { get; set; }
        /// <summary>
        /// Parts count that is taken when finishing goods.
        /// </summary>
        public int IncomingPartCount { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Parts requested from warehouse when feeding the line.
        /// </summary>
        public StepperValue RequestPartCount { get; set; } = new StepperValue();
        /// <summary>
        /// Minimum part count below which when the value drops, the proccess sends NoMaterial event.
        /// </summary>
        public StepperValue MinimumPartCount { get; set; } = new StepperValue();
        /// <summary>
        /// Sum of all downtimes.
        /// </summary>
        public int Downtime { get; set; }
        private int _downTimeStart = 0;
        /// <summary>
        /// Start of the current downtime.
        /// </summary>
        public int DownTimeStart { get => _downTimeStart; set => _downTimeStart = value; }
        /// <summary>
        /// Line to which the proccess is assigned.
        /// </summary>
        public Line Line { get; private set; }
        /// <summary>
        /// GUI tile that is used for deleting the proccess from GUI.
        /// </summary>
        public UCTile Tile { get; set; }
        private Scheduler _scheduler;
        private int _currentPartCount = 0;
        private bool _feedRequested = false;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// True if the proccess finished work on current goods, otherwise false.
        /// </summary>
        public bool IsFinished { get; set; }
        /// <summary>
        /// Current count of the parts in proccess.
        /// </summary>
        public int CurrentPartCount 
        {
            get => _currentPartCount;
            set => _currentPartCount = value < 0 ? 0 : value;
        }

        /// <summary>
        /// Creates new instance of line proccess.
        /// </summary>
        /// <param name="id">Proccess ID.</param>
        /// <param name="workingTime">Time that it takes to perform work on a single goods.</param>
        /// <param name="incomingPartCount">Part count that is needed to complete work on a single goods..</param>
        /// <param name="transitionTime">Time that it takes to transition goods between proccesses.</param>
        /// <param name="line">Line to which the proccess is assigned to.</param>
        /// <param name="scheduler">Reference to scheduler.</param>
        /// <param name="errorRate">Frequency at which goods becomes scrap on current proccess. If 0, no scraps are generated in this proccess.</param>
        /// <param name="requestPartCount">Part count coming from warehouse when feed is requested.</param>
        public LineProccess(int id, int workingTime, int incomingPartCount, int transitionTime, Line line, Scheduler scheduler, int errorRate, int requestPartCount)
        {
            ID = id;
            WorkingTime = workingTime;
            IncomingPartCount = incomingPartCount;
            TransitionTime = transitionTime;
            Line = line;
            _scheduler = scheduler;
            ErrorRate = errorRate;
            RequestPartCount.Value = requestPartCount;
        }
        
        /// <summary>
        /// Prepares the procces for simulation.
        /// </summary>
        public void Initialize()
        {
            CurrentPartCount = InitialPartCount.Value;
            CurrentGoods = null;
            IsHalted = false;
            ProducedGoods = 0;
            Downtime = 0;
            _downTimeStart = 0;
            _feedRequested = false;
            IsFinished = false;
        }

        /// <summary>
        /// Ends work on current goods.
        /// </summary>
        /// <returns>Goods that the proccess worked on.</returns>
        public Goods EndWork()
        {
            var returnValue = CurrentGoods;
            CurrentGoods = null;
            IsFinished = false;
            return returnValue;
        }

        /// <summary>
        /// Feeds proccess and resumes its work.
        /// </summary>
        /// <param name="count">Part count.</param>
        public void Feed(int count)
        {
            var continueWork = IsHalted && (int)CurrentPartCount - (int)IncomingPartCount < 0;
            CurrentPartCount += count;
            _feedRequested = false;
            if (continueWork)
            {
                DoWork();
            }
        }

        /// <summary>
        /// Starts working on goods.
        /// </summary>
        /// <param name="goods">Goods to perform work on.</param>
        public void StartWork(Goods goods)
        {
            if (CurrentGoods != null)
            {
                return;
            }
            CurrentGoods = goods;

            DoWork();
        }

        /// <summary>
        /// Deletes proccess from GUI.
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            return Tile?.Delete() ?? false;
        }

        /// <summary>
        /// Performs work on goods.
        /// </summary>
        private void DoWork()
        {
            if (CurrentPartCount - IncomingPartCount < MinimumPartCount.Value && !_feedRequested)
            {
#if PRINT
                Debug.WriteLine($"{_scheduler.Time}: Request feeding on proccess {ID}");
#endif
                _scheduler.Schedule(Line, this, this, Line, _scheduler.Time, EventTypes.NoMaterial);
                _feedRequested = true;
            }
            if (CurrentPartCount - IncomingPartCount < 0)
            {
#if PRINT
                Debug.WriteLine($"{_scheduler.Time}: Not enough material found on proccess {ID}");
#endif
                //_scheduler.Schedule(Line, this, this, Line, _scheduler.Time, EventTypes.NoMaterial);
                IsHalted = true;
                _downTimeStart = _scheduler.Time;
            }
            else
            {
                if (IsHalted)
                {
                    Downtime += _scheduler.Time - _downTimeStart;
                    IsHalted = false;
                }
                _scheduler.Schedule(Line, this, this, Line, _scheduler.Time + WorkingTime, EventTypes.ProccessFinished);
                IsFinished = true;
                CurrentPartCount -= IncomingPartCount;
                if (ErrorRate > 0 && ProducedGoods % ErrorRate == 0)
                {
                    CurrentGoods.IsScrap = true;
                }
                ProducedGoods++;
            }
        }

        /// <summary>
        /// Propagates a change in property to GUI.
        /// </summary>
        /// <param name="sender"></param>
        public void OnPropertyChanged(string sender)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }
}
