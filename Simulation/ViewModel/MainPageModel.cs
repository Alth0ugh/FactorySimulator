//#define PRINT
using Simulation.HelperClasses;
using Simulation.Model;
using Simulation.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Simulation.ViewModel
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public class MainPageModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Command that handles start simulation event.
        /// </summary>
        public ICommand StartSimulationCommand { get; set; }
        /// <summary>
        /// Command that handles new line event.
        /// </summary>
        public ICommand AddNewLineCommand { get; set; }
        /// <summary>
        /// Command that handles new proccess event.
        /// </summary>
        public ICommand AddProccessCommand { get; set; }
        /// <summary>
        /// Command that handles the deletion of line or proccess.
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        /// <summary>
        /// Command that handles setting up a stepper.
        /// </summary>
        public ICommand StepCommand { get; set; }
        /// <summary>
        /// Command that handles canceling stepper.
        /// </summary>
        public ICommand CancelStepperCommand { get; set; }
        /// <summary>
        /// Selected desired output from simulation.
        /// </summary>
        public OutputParameterEnum SelectedOutputParameter { get; set; }
        private bool _stepSet = false;
        /// <summary>
        /// True if stepper is set in the simulation, otherwise false.
        /// </summary>
        public bool StepSet 
        {
            get => _stepSet;
            set
            {
                _stepSet = value;
                OnPropertyChanged("StepSet");
            }
        }

        /// <summary>
        /// Duration of the simulation.
        /// </summary>
        public int SimulationDuration { get; set; } = 0;
        private Line _selectedLine;
        /// <summary>
        /// Currently selected line.
        /// </summary>
        public Line SelectedLine 
        {
            get => _selectedLine;
            set
            {
                _selectedLine = value;
                OnPropertyChanged("SelectedLine");
                RenderProccesses();
            }
        }

        private bool _isFirstTimeResult = true;

        private int _counter = 0;
        private Scheduler _scheduler;
        private StepperValue _selectedStepper;

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Collection of lines in the simulation.
        /// </summary>
        public ObservableCollection<Line> Lines { get; set; } = new ObservableCollection<Line>();
        /// <summary>
        /// Collection of proccesses in the simulation.
        /// </summary>
        public ObservableCollection<LineProccess> Proccesses { get; set; } = new ObservableCollection<LineProccess>();
        public MainPageModel()
        {
            SetCommands();
            _scheduler = new Scheduler();
        }
        /// <summary>
        /// Sets commands for buttons.
        /// </summary>
        public void SetCommands()
        {
            StartSimulationCommand = new Command(StartSimulation);
            AddNewLineCommand = new Command(AddNewLine);
            AddProccessCommand = new Command(AddProccess);
            DeleteCommand = new Command(DeleteProccessOrLine);
            StepCommand = new Command(Step);
            CancelStepperCommand = new Command(CancelStepper);
        }

        /// <summary>
        /// Cancels currently active stepper.
        /// </summary>
        /// <param name="o"></param>
        public void CancelStepper(object o)
        {
            if (_selectedStepper == null)
            {
                return;
            }

            _selectedStepper.FromChanged -= CancelStepper;
            _selectedStepper.To = _selectedStepper.From;
            _selectedStepper.Step = 0;
            _selectedStepper.Value = _selectedStepper.To;
            _selectedStepper.IsApplied = false;
            _selectedStepper = null;
            StepSet = false;
        }
        /// <summary>
        /// Sets new stepper.
        /// </summary>
        /// <param name="o">Stepper to be set.</param>
        public void Step(object o)
        {
            if (o is StepperValue)
            {
                var stepWindow = new StepWindow((StepperValue)o);
                if (stepWindow.DataContext is StepWindowModel)
                {
                    ((StepWindowModel)stepWindow.DataContext).OnStepSet += OnStepSet;
                }
                stepWindow.ShowDialog();
            }
        }
        /// <summary>
        /// Handles setting up new stepper.
        /// </summary>
        /// <param name="stepperValue">Stepper that was set.</param>
        public void OnStepSet(StepperValue stepperValue)
        {
            StepSet = true;
            _selectedStepper = stepperValue;
            _selectedStepper.FromChanged += CancelStepper;
        }
        /// <summary>
        /// Deletes proccess or line.
        /// </summary>
        /// <param name="o">Proccess or line to be deleted.</param>
        public void DeleteProccessOrLine(object o)
        {
            var wasDeletedStepper = false;
            if (o is LineProccess)
            {
                var proccess = (LineProccess)o;
                wasDeletedStepper = proccess.Delete();
                Proccesses.Remove(proccess);
                SelectedLine.Proccesses.Remove(proccess);
                for (int i = 0; i < SelectedLine.Proccesses.Count; i++)
                {
                    SelectedLine.Proccesses[i].ID = i;
                }
            }
            else if (o is Line)
            {
                var line = (Line)o;
                wasDeletedStepper = line.Delete();
                Lines.Remove(line);
                for (int i = 0; i < Lines.Count; i++)
                {
                    Lines[i].ID = i;
                }
            }

            if (wasDeletedStepper)
            {
                StepSet = false;
                _selectedStepper = null;
            }
        }
        /// <summary>
        /// Adds new line.
        /// </summary>
        /// <param name="o"></param>
        public void AddNewLine(object o)
        {
            Lines.Add(new Line(_scheduler) { ID = Lines.Count });
            _counter++;
        }
        /// <summary>
        /// Starts current simualtion.
        /// </summary>
        /// <param name="o"></param>
        public void StartSimulation(object o)
        {
            FileLogger.Initialize($@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\Desktop\output.txt", System.IO.FileMode.Append);

            _scheduler.Lines = Lines;
            var simCounter = 1;
            _isFirstTimeResult = true;
            if (_selectedStepper != null)
            {
                for (int i = _selectedStepper.From; i <= _selectedStepper.To; i += _selectedStepper.Step)
                {
#if PRINT
                    Debug.WriteLine($"-------- Starting simulation number {simCounter++} --------");
#endif
                    ResetSimulation();
                    _selectedStepper.Value = i;
                    try
                    {
                        _scheduler.StartSimulation(SimulationDuration);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Chyba pri spúšťaní simulácie");
                        FileLogger.Close();
                        return;
                    }
                    WriteResults();
                }
            }
            else 
            {
                ResetSimulation();
                try
                {
                    _scheduler.StartSimulation(SimulationDuration);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Chyba pri spúšťaní simulácie");
                    FileLogger.Close();
                    return;
                }
                WriteResults();
            }
            FileLogger.Close();
            MessageBox.Show("Simulácia úspešne skončila.", "Koniec simulácie");
        }

        /// <summary>
        /// Adds new proccess to currently selected line.
        /// </summary>
        /// <param name="o"></param>
        public void AddProccess(object o)
        {
            var newProccess = new LineProccess(Proccesses.Count, 1, 0, 1, SelectedLine, _scheduler, 0, 0);
            Proccesses.Add(newProccess);
            SelectedLine.Proccesses.Add(newProccess);
        }

        /// <summary>
        /// Propagates property changes to GUI.
        /// </summary>
        /// <param name="sender">Name of the sender.</param>
        public void OnPropertyChanged(string sender)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }

        /// <summary>
        /// Prints out the result of the simulation to a text file.
        /// </summary>
        private void WriteResults()
        {
            FileLogger.Write(GetHeaderMessage());
            switch (SelectedOutputParameter)
            {
                case OutputParameterEnum.TotalFG:
                    WriteTotalFinishedGoods(_selectedStepper);
                    break;
                case OutputParameterEnum.UnloadValues:
                    WriteUnloadValues(_selectedStepper);
                    break;
                case OutputParameterEnum.Downtime:
                    WriteDowntime(_selectedStepper);
                    break;
                case OutputParameterEnum.PartCount:
                    WriteTotalPartsUsed(_selectedStepper);
                    break;
                default:
                    return;
            }
        }

        private string GetHeaderMessage()
        {
            var result = string.Empty;
            switch (SelectedOutputParameter)
            {
                case OutputParameterEnum.TotalFG:
                    result = _selectedStepper != null ? "Line:Step:FinishedGoods:Scrap:TotalGoods\n" : "Line:FinishedGoods:Scrap:TotalGoods\n";
                    break;
                case OutputParameterEnum.UnloadValues:
                    result = _selectedStepper != null ? "Line:Step:UnloadValue:Count\n" : "Line:UnloadValue:Count\n";
                    break;
                case OutputParameterEnum.Downtime:
                    result = _selectedStepper != null ? "Line:Step:Proccess:Downtime\n" : "Line:Proccess:Downtime\n";
                    break;
                case OutputParameterEnum.PartCount:
                    result = _selectedStepper != null ? "Line:Step:Count\n" : "Line:Count\n";
                    break;
                default:
                    break;
            }

            if (!_isFirstTimeResult)
            {
                result = string.Empty;
            }
            _isFirstTimeResult = false;
            return result;
        }

        private void ResetSimulation()
        {
            foreach (var line in Lines)
            {
                line.Initialize();
                foreach (var proccess in line.Proccesses)
                {
                    proccess.Initialize();
                }
            }
            //_scheduler.Events.Clear();
            _scheduler.Time = 0;
        }

        /// <summary>
        /// Renders proccesses for currently selected line.
        /// </summary>
        private void RenderProccesses()
        {
            Proccesses.Clear();
            if (SelectedLine == null)
            {
                return;
            }
            foreach (var proccess in SelectedLine.Proccesses)
            {
                Proccesses.Add(proccess);
            }
        }

        private void WriteTotalPartsUsed(StepperValue value)
        {
            foreach (var line in Lines)
            {
                if (value == null)
                {
                    FileLogger.WriteLine($"{line.Name}:{GetUsedParts(line)}");
                }
                else
                {
                    FileLogger.WriteLine($"{line.Name}:{value.Value}:{GetUsedParts(line)}");
                }
            }
        }

        public void WriteTotalFinishedGoods(StepperValue value)
        {
            foreach (var line in Lines)
            {
                if (line.Proccesses.Count == 0)
                {
                    continue;
                }
                if (value == null)
                {
                    FileLogger.WriteLine($"{line.Name}:{line.TotalFinishedGoods}:{line.ScrapCount}:{line.Proccesses[0].ProducedGoods}");
                }
                else
                {
                    FileLogger.WriteLine($"{line.Name}:{value.Value}:{line.TotalFinishedGoods}:{line.ScrapCount}:{line.Proccesses[0].ProducedGoods}");
                }
            }
        }

        private int GetUsedParts(Line line)
        {
            var totalUsedParts = 0;
            foreach (var proccess in line.Proccesses)
            {
                totalUsedParts += proccess.ProducedGoods * proccess.IncomingPartCount;
            }
            return totalUsedParts;
        }

        private void WriteUnloadValues(StepperValue value)
        {
            foreach (var line in Lines)
            {
                for (int i = 0; i < line.UnloadValues.Length; i++)
                {
                    if (value == null)
                    {
                        FileLogger.WriteLine($"{line.Name}:{i}:{line.UnloadValues[i]}");
                    }
                    else
                    {
                        FileLogger.WriteLine($"{line.Name}:{value.Value}:{i}:{line.UnloadValues[i]}");
                    }
                }
            }
        }

        private void WriteDowntime(StepperValue value)
        {
            foreach (var line in Lines)
            {
                foreach (var proccess in line.Proccesses)
                {
                    if (value == null)
                    {
                        FileLogger.WriteLine($"{line.Name}:{proccess.Name}:{proccess.Downtime}");
                    }
                    else
                    {
                        FileLogger.WriteLine($"{line.Name}:{value.Value}:{proccess.Name}:{proccess.Downtime}");
                    }
                }
            }
        }
    }
}
