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
    /// Handles event when From property of StepperValue changes.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void FromChangedHandler(object sender);
    /// <summary>
    /// Provides stepping of values in simulation.
    /// </summary>
    public class StepperValue : ICloneable, INotifyPropertyChanged
    {
        private int _value = 0;
        /// <summary>
        /// Current value.
        /// </summary>
        public int Value 
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        private int _from = 0;
        /// <summary>
        /// Starting value of stepping.
        /// </summary>
        public int From 
        {
            get => _from;
            set
            {
                _from = value;
                To = value;
                Value = value;
                Step = 0;
                FromChanged?.Invoke(this);
                OnPropertyChanged("From");
            }
        }
        private int _to = 0;
        /// <summary>
        /// Ending value of stepping.
        /// </summary>
        public int To 
        {
            get => _to;
            set
            {
                if (value < From)
                {
                    return;
                }
                _to = value;
                OnPropertyChanged("To");
            }
        }
        private int _step = 0;
        /// <summary>
        /// Step value.
        /// </summary>
        public int Step 
        {
            get => _step;
            set
            {
                _step = value;
                OnPropertyChanged("Step");
            }
        }
        private bool _isApplied = false;
        /// <summary>
        /// True if stepping is applied, otherwise false.
        /// </summary>
        public bool IsApplied 
        {
            get => _isApplied;
            set
            {
                _isApplied = value;
                OnPropertyChanged("IsApplied");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event FromChangedHandler FromChanged;

        /// <summary>
        /// Propagates property changes to GUI.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        public void OnPropertyChanged(string sender)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }

        /// <summary>
        /// Resets the stepper.
        /// </summary>
        public void Reset()
        {
            To = From;
            Value = From;
            Step = 0;
            IsApplied = false;
        }

        /// <summary>
        /// Clones the stepper.
        /// </summary>
        /// <returns>New instance of StepperValue with the same properties as original instance.</returns>
        public object Clone()
        {
            var clone = new StepperValue();
            clone.Value = Value;
            clone.From = From;
            clone.To = To;
            clone.Step = Step;
            return clone;
        }
    }
}
