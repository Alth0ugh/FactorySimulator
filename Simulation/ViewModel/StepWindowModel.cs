using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Simulation.ViewModel
{
    /// <summary>
    /// Handles event when some stepper is set.
    /// </summary>
    /// <param name="stepperValue">Stepper that is set.</param>
    public delegate void StepSetHandler(StepperValue stepperValue);
    /// <summary>
    /// Interaction logic for StepWindow.
    /// </summary>
    public class StepWindowModel
    {
        /// <summary>
        /// Internal stepper.
        /// </summary>
        private StepperValue _stepperValue;
        /// <summary>
        /// Current stepper value whci represents StepWindow.
        /// </summary>
        public StepperValue CurrentStepperValue { get; set; }
        /// <summary>
        /// Command when stepper settings are saved.
        /// </summary>
        public ICommand SaveCommand { get; set; }
        /// <summary>
        /// Command when stepper settings are canceled.
        /// </summary>
        public ICommand CancelCommand { get; set; }
        /// <summary>
        /// Handles event when stepper is set.
        /// </summary>
        public event StepSetHandler OnStepSet;
        /// <summary>
        /// Reference to parent window.
        /// </summary>
        private Window _window;
        public StepWindowModel(StepperValue stepperValue, Window window)
        {
            _stepperValue = stepperValue;
            CurrentStepperValue = (StepperValue)stepperValue.Clone();
            _window = window;
            SetCommands();
        }

        /// <summary>
        /// Saves stepper settings.
        /// </summary>
        /// <param name="o"></param>
        public void Save(object o)
        {
            _stepperValue.From = CurrentStepperValue.From;
            _stepperValue.To = CurrentStepperValue.To;
            _stepperValue.Step = CurrentStepperValue.Step;
            _stepperValue.IsApplied = true;
            OnStepSet?.Invoke(_stepperValue);
            _window?.Close();
        }

        /// <summary>
        /// Cancels stepper settings.
        /// </summary>
        /// <param name="o"></param>
        public void Cancel(object o)
        {
            _window?.Close();
        }

        /// <summary>
        /// Sets button commands.
        /// </summary>
        private void SetCommands()
        {
            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);
        }
    }
}
