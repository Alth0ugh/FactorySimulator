using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Simulation.UserControls
{
    /// <summary>
    /// Interaction logic for UCTile.xaml
    /// </summary>
    public partial class UCTile : UserControl
    {
        public int TransitionTime { get => (int)GetValue(TransitionTimeDependency); set => SetValue(TransitionTimeDependency, value); }
        public int ErrorRate { get => (int)GetValue(ErrorRateDependency); set => SetValue(ErrorRateDependency, value); }
        public int PartsCount { get => (int)GetValue(PartsCountDependency); set => SetValue(PartsCountDependency, value); }
        public StepperValue RequestParts { get => (StepperValue)GetValue(RequestPartsDependency); set => SetValue(RequestPartsDependency, value); }
        public StepperValue FeedInterval { get => (StepperValue)GetValue(FeedIntervalDependency); set => SetValue(FeedIntervalDependency, value); }
        public StepperValue UnloadInterval { get => (StepperValue)GetValue(UnloadIntervalDependency); set => SetValue(UnloadIntervalDependency, value); }
        public StepperValue FinishedCapacity { get => (StepperValue)GetValue(FinishedCapacityDependency); set => SetValue(FinishedCapacityDependency, value); }
        public int ID { get => (int)GetValue(IDDependency); set => SetValue(IDDependency, value); }
        public int WorkingTime { get => (int)GetValue(WorkingTimeDependency); set => SetValue(WorkingTimeDependency, value); }
        public StepperValue CurrentPartCount { get => (StepperValue)GetValue(CurrentPartCountProperty); set => SetValue(CurrentPartCountProperty, value); }
        public StepperValue MinimumPartCount { get => (StepperValue)GetValue(MinimumPartCountProperty); set => SetValue(MinimumPartCountProperty, value); }
        public Visibility TransitionTimeVisibility { get => (Visibility)GetValue(TransitionTimeVisibilityProperty); set => SetValue(TransitionTimeVisibilityProperty, value); }
        public Visibility ErrorRateVisibility { get => (Visibility)GetValue(ErrorRateVisibilityProperty); set => SetValue(ErrorRateVisibilityProperty, value); }
        public Visibility PartsCountVisibility { get => (Visibility)GetValue(PartsCountVisibilityProperty); set => SetValue(PartsCountVisibilityProperty, value); }
        public Visibility RequestPartsVisibility { get => (Visibility)GetValue(RequestPartsVisibilityProperty); set => SetValue(RequestPartsVisibilityProperty, value); }
        public Visibility FeedIntervalVisibility { get => (Visibility)GetValue(FeedIntervalVisibilityProperty); set => SetValue(FeedIntervalVisibilityProperty, value); }
        public Visibility UnloadIntervalVisibility { get => (Visibility)GetValue(UnloadIntervalVisibilityProperty); set => SetValue(UnloadIntervalVisibilityProperty, value); }
        public Visibility FinishedCapacityVisibility { get => (Visibility)GetValue(FinishedCapacityVisibilityProperty); set => SetValue(FinishedCapacityVisibilityProperty, value); }
        public Visibility WorkingTimeVisibility { get => (Visibility)GetValue(WorkingTimeVisibilityProperty); set => SetValue(WorkingTimeVisibilityProperty, value); }
        public Visibility CurrentPartCountVisibility { get => (Visibility)GetValue(CurrentPartCountVisibilityProperty); set => SetValue(CurrentPartCountVisibilityProperty, value); }
        public Visibility MinimumPartCountVisibility { get => (Visibility)GetValue(CurrentPartCountVisibilityProperty); set => SetValue(CurrentPartCountVisibilityProperty, value); }
        public ICommand DeleteCommand { get => (ICommand)GetValue(DeleteCommandProperty); set => SetValue(DeleteCommandProperty, value); }
        public object DeleteParameter { get => GetValue(DeleteParameterProperty); set => SetValue(DeleteParameterProperty, value); }
        public ICommand StepCommand { get => (ICommand)GetValue(StepCommandProperty); set => SetValue(StepCommandProperty, value); }
        public bool StepSet { get => (bool)GetValue(StepSetProperty); set => SetValue(StepSetProperty, value); }
        public ICommand CancelStepperCommand { get => (ICommand)GetValue(CancelStepperCommandProperty); set => SetValue(CancelStepperCommandProperty, value); }
        public string ModelName { get => (string)GetValue(ModelNameProperty); set => SetValue(ModelNameProperty, value); }
        public static readonly DependencyProperty ModelNameProperty = DependencyProperty.Register("ModelName", typeof(string), typeof(UCTile), new PropertyMetadata(""));
        public static readonly DependencyProperty CancelStepperCommandProperty = DependencyProperty.Register("CancelStepperCommand", typeof(ICommand), typeof(UCTile));
        public static readonly DependencyProperty StepSetProperty = DependencyProperty.Register("StepSet", typeof(bool), typeof(UCTile), new PropertyMetadata(false));
        public static readonly DependencyProperty StepCommandProperty = DependencyProperty.Register("StepCommand", typeof(ICommand), typeof(UCTile));
        public static readonly DependencyProperty DeleteParameterProperty = DependencyProperty.Register("DeleteParameter", typeof(object), typeof(UCTile));
        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(UCTile));
        public static readonly DependencyProperty CurrentPartCountProperty = DependencyProperty.Register("CurrentPartCount", typeof(StepperValue), typeof(UCTile), new PropertyMetadata(new StepperValue()));
        public static readonly DependencyProperty MinimumPartCountProperty = DependencyProperty.Register("MinimumPartCount", typeof(StepperValue), typeof(UCTile), new PropertyMetadata(new StepperValue()));
        public static readonly DependencyProperty CurrentPartCountVisibilityProperty = DependencyProperty.Register("CurrentPartCountVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty MinimumPartCountVisibilityProperty = DependencyProperty.Register("MinimumPartCountVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty WorkingTimeVisibilityProperty = DependencyProperty.Register("WorkingTimeVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty FinishedCapacityVisibilityProperty = DependencyProperty.Register("FinishedCapacityVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty UnloadIntervalVisibilityProperty = DependencyProperty.Register("UnloadIntervalVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty FeedIntervalVisibilityProperty = DependencyProperty.Register("FeedIntervalVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty RequestPartsVisibilityProperty = DependencyProperty.Register("RequestPartsVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty PartsCountVisibilityProperty = DependencyProperty.Register("PartsCountVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty ErrorRateVisibilityProperty = DependencyProperty.Register("ErrorRateVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TransitionTimeVisibilityProperty = DependencyProperty.Register("TransitionTimeVisibility", typeof(Visibility), typeof(UCTile), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TransitionTimeDependency = DependencyProperty.Register("TransitionTime", typeof(int), typeof(UCTile), new PropertyMetadata() { DefaultValue = (int)0 });
        public static readonly DependencyProperty ErrorRateDependency = DependencyProperty.Register("ErrorRate", typeof(int), typeof(UCTile), new PropertyMetadata() { DefaultValue = 0 });
        public static readonly DependencyProperty PartsCountDependency = DependencyProperty.Register("PartsCount", typeof(int), typeof(UCTile), new PropertyMetadata() { DefaultValue = 0 });
        public static readonly DependencyProperty RequestPartsDependency = DependencyProperty.Register("RequestParts", typeof(StepperValue), typeof(UCTile), new PropertyMetadata(new StepperValue()));
        public static readonly DependencyProperty FeedIntervalDependency = DependencyProperty.Register("FeedInterval", typeof(StepperValue), typeof(UCTile), new PropertyMetadata(new StepperValue()));
        public static readonly DependencyProperty UnloadIntervalDependency = DependencyProperty.Register("UnloadInterval", typeof(StepperValue), typeof(UCTile), new PropertyMetadata(new StepperValue()));
        public static readonly DependencyProperty IDDependency = DependencyProperty.Register("ID", typeof(int), typeof(UCTile), new PropertyMetadata() { DefaultValue = 0 });
        public static readonly DependencyProperty FinishedCapacityDependency = DependencyProperty.Register("FinishedCapacity", typeof(StepperValue), typeof(UCTile), new PropertyMetadata(new StepperValue()));
        public static readonly DependencyProperty WorkingTimeDependency = DependencyProperty.Register("WorkingTime", typeof(int), typeof(UCTile), new PropertyMetadata() { DefaultValue = (int)0 });

        public UCTile()
        {
            InitializeComponent();
            DataContextChanged += ContextChanged;
        }

        public void ContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is Line)
            {
                var line = (Line)DataContext;
                line.Tile = this;
            }
            if (DataContext is LineProccess)
            {
                var proccess = (LineProccess)DataContext;
                proccess.Tile = this;
            }
        }

        public bool Delete()
        {
            var returnValue = false;
            if (RequestParts.IsApplied ||
                FeedInterval.IsApplied ||
                UnloadInterval.IsApplied ||
                FinishedCapacity.IsApplied ||
                CurrentPartCount.IsApplied || 
                MinimumPartCount.IsApplied)
            {
                returnValue = true;
            }
            RequestParts.Reset();
            FeedInterval.Reset();
            UnloadInterval.Reset();
            FinishedCapacity.Reset();
            CurrentPartCount.Reset();
            MinimumPartCount.Reset();
            return returnValue;
        }
    }
}
