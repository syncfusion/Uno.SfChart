using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for RangeAxisBase
    /// </summary>
    public partial class NumericalAxis : RangeAxisBase
    {
        #region Dependency Property Registration

#if NETFX_CORE

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval",
                typeof(object),
                typeof(NumericalAxis),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(object),
                typeof(NumericalAxis),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(object), 
                typeof(NumericalAxis),
                new PropertyMetadata(null, OnMaximumChanged));

#else

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(double?), 
                typeof(NumericalAxis),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(double?),
                typeof(NumericalAxis),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(double?),
                typeof(NumericalAxis),
                new PropertyMetadata(null, OnMaximumChanged));

#endif

        /// <summary>
        /// The DependencyProperty for <see cref="RangePadding"/> property.
        /// </summary>
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register(
                "RangePadding", 
                typeof(NumericalPadding),
                typeof(NumericalAxis),
                new PropertyMetadata(NumericalPadding.Auto, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="StartRangeFromZero"/> property.
        /// </summary>
        public static readonly DependencyProperty StartRangeFromZeroProperty =
            DependencyProperty.Register(
                "StartRangeFromZero",
                typeof(bool),
                typeof(NumericalAxis),
                new PropertyMetadata(false, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="AxisScaleBreaks"/> property.
        /// </summary>
        public static readonly DependencyProperty AxisScaleBreaksProperty =
            DependencyProperty.Register("AxisScaleBreaks", typeof(ChartAxisScaleBreaks), typeof(NumericalAxis), new PropertyMetadata(null, OnScaleBreakChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="BreakPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty BreakPositionProperty =
            DependencyProperty.Register(
                "BreakPosition", 
                typeof(ScaleBreakPosition),
                typeof(NumericalAxis),
                new PropertyMetadata(ScaleBreakPosition.DataCount, OnBreakPositionChanged));

        #endregion

        #region Fields

        #region Internal Fields

        internal double DataRangeDelta;

        internal List<FrameworkElement> BreakShapes;

        internal List<DoubleRange> BreakRanges, AxisRanges;

        internal Dictionary<DoubleRange, ChartAxisScaleBreak> BreakRangesInfo;

        internal List<double> AxisLength;

        #endregion

        #region Private Fields

        List<DoubleRange> dataRanges;

        List<double> dataPoints;

        #endregion

        #endregion

        #region Constructor

        public NumericalAxis()
        {
            Breaks = new ChartAxisScaleBreak();
            AxisScaleBreaks = new ChartAxisScaleBreaks();
            BreakShapes = new List<FrameworkElement>();
            BreakRangesInfo = new Dictionary<DoubleRange, ChartAxisScaleBreak>();
        }

        #endregion

        #region Properties

        #region Public Properties

#if NETFX_CORE

        /// <summary>
        /// Gets or sets a value that determines the interval between labels.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public object Interval
        {
            get { return (object)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value for the axis range. This is nullable property.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the minimum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Minimum
        {
            get { return (object)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the maximum value for the axis range. This is nullable property. 
        /// </summary>
        /// <remarks>
        ///     If we didn't set the maximum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Maximum
        {
            get { return (object)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

#else

        /// <summary>
        /// Gets or sets a value that determines the interval between labels. Its nullable.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public double? Interval
        {
            get { return (double?)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value for the axis range. This is nullable property.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the minimum value, it will be calculate from the underlying collection.
        /// </remarks>
        public double? Minimum
        {
            get { return (double?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value for the axis range. This is nullable property. 
        /// </summary>
        /// <remarks>
        ///     If we didn't set the maximum value, it will be calculate from the underlying collection.
        /// </remarks>
        public double? Maximum
        {
            get { return (double?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

#endif

        /// <summary>
        /// Gets or sets the padding used to shift the numeric range inside or outside.
        /// </summary>
        /// <value>
        ///     <c>Additional</c>, to extend the range,
        ///     <c>Round</c>, to round-off the range,
        ///     <c>None</c>, do nothing,
        ///     <c>Auto</c>, auto range based on type of series.
        /// </value>
        public NumericalPadding RangePadding
        {
            get { return (NumericalPadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to start range from zero when range calculated automatically.
        /// </summary>
        /// <value>
        ///     <c>True</c> will reset the range starting from zero.
        /// </value>
        public bool StartRangeFromZero
        {
            get { return (bool)GetValue(StartRangeFromZeroProperty); }
            set { SetValue(StartRangeFromZeroProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scale break collection for the axis.
        /// </summary>
        public ChartAxisScaleBreaks AxisScaleBreaks
        {
            get { return (ChartAxisScaleBreaks)GetValue(AxisScaleBreaksProperty); }
            set { SetValue(AxisScaleBreaksProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property used to position the breaks.
        /// </summary>
        /// <value>          
        ///     <c>DataCount</c>, Break will be placed based on the data count,
        ///     <c>Percent</c>, Break will be placed based on the given BreakPercent, 
        ///     <c>Scale</c>, Break will be placed based on the delta of the range         
        /// </value>
        public ScaleBreakPosition BreakPosition
        {
            get { return (ScaleBreakPosition)GetValue(BreakPositionProperty); }
            set { SetValue(BreakPositionProperty, value); }
        }

        #endregion

        #region Internal Properties
        
        // Summary:
        //     Gets or sets a NumericalPadding that describes the padding of a NumericalAxis.
        //
        // Returns:
        //     The NumericalPadding that is used to set the padding of the NumericalAxis. 
        //     The default is Syncfusion.UI.Xaml.Charts.NumericalPadding.Auto.

        internal NumericalPadding ActualRangePadding
        {
            get
            {
                SfChart sfArea = Area as SfChart;
                if (RangePadding == NumericalPadding.Auto && sfArea != null && sfArea.Series != null && sfArea.Series.Count > 0)
                {
                    if ((Orientation == Orientation.Vertical && !sfArea.Series[0].IsActualTransposed) ||
                        (Orientation == Orientation.Horizontal && sfArea.Series[0].IsActualTransposed))
                        return NumericalPadding.Round;
                }

                return (NumericalPadding)GetValue(RangePaddingProperty);
            }
        }

        internal ChartAxisScaleBreak Breaks
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods
        
        public override double CoefficientToValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            if (AxisRanges != null && AxisRanges.Count > 0)
            {
                result = CalculateCoefficientToValue(value);
            }
            else
                result = VisibleRange.Start + VisibleRange.Delta * value;
            return result;
        }
        
        public override double ValueToCoefficient(double value)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            if (AxisRanges != null && AxisRanges.Count > 0)
            {
                result = CalculateValueToCoefficient(value);
            }
            else
            {
                if (delta == 0)
                    return -1;
                result = (value - start) / delta;
            }

            return this.isInversed ? 1d - result : result;
        }

        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        public override double ValueToPolarCoefficient(double value)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            result = (value - start) / delta;

            return ValueBasedOnAngle(result);
        }

        #endregion

        #region Internal Override Methods

        internal override void Dispose()
        {
            if (AxisScaleBreaks != null)
            {
                AxisScaleBreaks.CollectionChanged -= OnScaleBreakCollectionChanged;
                AxisScaleBreaks.Clear();
                AxisScaleBreaks = null;
            }           

            base.Dispose();
        }

        #endregion

        #region Internal Methods

        internal bool BreakExistence()
        {
            if (AxisScaleBreaks.Count > 0 && (BreakRanges != null && this.BreakRanges.Count > 0))
                return true;
            else
                return false;
        }

        internal void OnScaleBreakChanged(ChartAxisScaleBreaks newValue, ChartAxisScaleBreaks oldValue)
        {
            if (newValue != null)
            {
                newValue.CollectionChanged += OnScaleBreakCollectionChanged;
                foreach (var scalebreak in newValue)
                {
                    scalebreak.PropertyChanged += Scalebreak_PropertyChanged;
                    if (axisPanel != null && !axisPanel.Children.Contains(scalebreak))
                    {
                        axisPanel.Children.Add(scalebreak);
                    }
                }
            }

            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnScaleBreakCollectionChanged;
                foreach (var scalebreak in oldValue)
                {
                    scalebreak.PropertyChanged -= Scalebreak_PropertyChanged;
                    if (axisPanel != null && axisPanel.Children.Contains(scalebreak))
                    {
                        axisPanel.Children.Remove(scalebreak);
                    }
                }
            }

            UpdateArea();
        }

        internal void DrawScaleBreakLines(ChartAxis axis)
        {
            if (Area == null || Area.AreaType != ChartAreaType.CartesianAxes)
            {
                ClearBreakElements();
                return;
            }

            ChartAxisScaleBreak.DrawPath(this);
        }

        internal void ClearBreakElements()
        {
            if (Area == null) return;
            if (AxisRanges != null)
                AxisRanges.Clear();
            if (BreakRanges != null)
                BreakRanges.Clear();
            if (dataRanges != null)
                dataRanges.Clear();
            BreakRangesInfo.Clear();
            if (Area.AdorningCanvas == null) return;

            foreach (var element in BreakShapes)
            {
                Area.AdorningCanvas.Children.Remove(element);
            }

            BreakShapes.Clear();
        }

        internal void CalculateScaleBreaksRanges(ObservableCollection<ISupportAxes> series)
        {
            if (Area == null || AxisScaleBreaks.Count == 0) return;
            DataRangeDelta = 0;
            BreakRanges = new List<DoubleRange>();
            BreakRangesInfo = new Dictionary<DoubleRange, ChartAxisScaleBreak>();
            AxisRanges = new List<DoubleRange>();
            dataRanges = new List<DoubleRange>();
            for (int i = 0; i < AxisScaleBreaks.Count; i++)
            {
                if (double.IsNaN(AxisScaleBreaks[i].Start) || (double.IsNaN(AxisScaleBreaks[i].End))) continue;
                var range = new DoubleRange(AxisScaleBreaks[i].Start, AxisScaleBreaks[i].End);
                if (VisibleRange.Inside(range) && range.Delta > 0)
                {
                    ComputeBreakRange(range, i);
                }
            }

            AddDataRange();
            foreach (var range in dataRanges)
            {
                DataRangeDelta += range.Delta;
            }

            ComputeAxisHeight(dataRanges, DataRangeDelta);
        }

        internal void ComputeScaleBreaks()
        {
            if (Area != null)
            {
                if (Area.AreaType != ChartAreaType.CartesianAxes || ZoomFactor != 1 || ZoomPosition != 0 || IsDefaultRange)
                {
                    GenerateDefaultLabel();
                    return;
                }

                var registeredSeries = RegisteredSeries;

                if (registeredSeries.Count == 0 || registeredSeries.Any(series => ((!(series is CartesianSeries)) || series is StackingSeriesBase)))
                {
                    GenerateDefaultLabel();
                    return;
                }

                GetSeriesYValues(registeredSeries);
                CalculateScaleBreaksRanges(registeredSeries);
                NumericalAxisHelper.GenerateScaleBreakVisibleLabels(this, Interval, SmallTicksPerInterval);
            }
            else
            {
                ClearBreakElements();
                GenerateDefaultLabel();
            }
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (Interval == null || Convert.ToDouble(Interval) == 0 || (AxisRanges != null && AxisRanges.Count > 0))
                return base.CalculateNiceInterval(range, availableSize);
#if NETFX_CORE
            return Convert.ToDouble(Interval);
#else
            return Interval.Value;
#endif
        }
        
        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size avalableSize)
        {
            base.CalculateVisibleRange(avalableSize);
            NumericalAxisHelper.CalculateVisibleRange(this, avalableSize, Interval);
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called when Maximum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        /// <summary>
        /// called when Minimum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        /// <summary>
        /// Called when interval changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            SetRangeForAxisStyle();
            ClearBreakElements();
            if (AxisScaleBreaks.Count > 0 && this.IsSecondaryAxis && CustomLabels.Count == 0 && LabelsSource == null)
            {
                ComputeScaleBreaks();
            }
            else
                NumericalAxisHelper.GenerateVisibleLabels(this, Minimum, Maximum, Interval, SmallTicksPerInterval);
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected override DoubleRange CalculateActualRange()
        {
            if (ActualRange.IsEmpty) // Executes when Minimum and Maximum aren't set.
            {
                DoubleRange range = base.CalculateActualRange();

                // Execute when include the annotion range for Auto Range
                if (IncludeAnnotationRange)
                {
                    foreach (var annotation in (Area as SfChart).Annotations)
                    {
                        if (Orientation == Orientation.Vertical && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.YAxis == this)
                            range += new DoubleRange(
                                Annotation.ConvertData(annotation.Y1, this),
                                     annotation is TextAnnotation ?
                                        Annotation.ConvertData(annotation.Y1, this) : annotation is ImageAnnotation ?
                                        Annotation.ConvertData((annotation as ImageAnnotation).Y2, this) :
                                        Annotation.ConvertData((annotation as ShapeAnnotation).Y2, this));
                        else if (Orientation == Orientation.Horizontal && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.XAxis == this)
                            range += new DoubleRange(
                                Annotation.ConvertData(annotation.X1, this), 
                                annotation is TextAnnotation ?
                                Annotation.ConvertData(annotation.X1, this) : annotation is ImageAnnotation ?
                                Annotation.ConvertData((annotation as ImageAnnotation).X2, this) :
                                Annotation.ConvertData((annotation as ShapeAnnotation).X2, this));
                    }
                }

                return range;
            }
            else if (Minimum != null && Maximum != null) // Executes when Minimum and Maximum are set.
                return ActualRange;
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange range = base.CalculateActualRange();
                if (StartRangeFromZero && range.Start > 0)
                    return new DoubleRange(0, range.End);
                else if (Minimum != null)
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? ActualRange.Start + 1 : range.End);
                else if (Maximum != null)
                    return new DoubleRange(double.IsNaN(range.Start) ? ActualRange.End - 1 : range.Start, ActualRange.End);
                else
                {
                    if (IncludeAnnotationRange)
                    {
                        foreach (var annotation in (Area as SfChart).Annotations)
                        {
                            if (Orientation == Orientation.Vertical && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.YAxis == this)
                                range += new DoubleRange(
                                    Annotation.ConvertData(annotation.Y1, this), annotation is TextAnnotation ?
                                    Annotation.ConvertData(annotation.Y1, this) : annotation is ImageAnnotation ?
                                    Annotation.ConvertData((annotation as ImageAnnotation).Y2, this) :
                                    Annotation.ConvertData((annotation as ShapeAnnotation).Y2, this));
                            else if (Orientation == Orientation.Horizontal && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.XAxis == this)
                                    range += new DoubleRange(
                                    Annotation.ConvertData(annotation.X1, this), annotation is TextAnnotation ?
                                    Annotation.ConvertData(annotation.X1, this) : annotation is ImageAnnotation ?
                                    Annotation.ConvertData((annotation as ImageAnnotation).X2, this) :
                                    Annotation.ConvertData((annotation as ShapeAnnotation).X2, this));
                        }
                    }

                    return range;
                }
            }
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (Minimum == null && Maximum == null)  // Executes when Minimum and Maximum aren't set.
                return NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, ActualRangePadding);
            else if (Minimum != null && Maximum != null)  // Executes when Minimum and Maximum are set.
                return range;
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, ActualRangePadding);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var numericalAxis = new NumericalAxis();
            numericalAxis.Minimum = this.Minimum;
            numericalAxis.Maximum = this.Maximum;
            numericalAxis.StartRangeFromZero = this.StartRangeFromZero;
            numericalAxis.Interval = this.Interval;
            numericalAxis.RangePadding = this.RangePadding;
            foreach (ChartAxisScaleBreak scaleBreak in AxisScaleBreaks)
            {
                numericalAxis.AxisScaleBreaks.Add((ChartAxisScaleBreak)scaleBreak.Clone());
            }

            obj = numericalAxis;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericalAxis).OnPropertyChanged();
        }

        private static void OnScaleBreakChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericalAxis).OnScaleBreakChanged(e.NewValue as ChartAxisScaleBreaks, e.OldValue as ChartAxisScaleBreaks);
        }


        private static void OnBreakPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as NumericalAxis;
            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericalAxis).OnMinimumChanged(e);
        }

        /// <summary>
        /// Called Maximum property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericalAxis).OnMaximumChanged(e);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericalAxis).OnIntervalChanged(e);
        }

        #endregion

        #region Private Methods
        
        private void Scalebreak_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateArea();
        }

        private void OnScaleBreakCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ChartAxisScaleBreak item in e.NewItems)
                {
                    item.PropertyChanged += Scalebreak_PropertyChanged;
                    if (axisPanel != null && !axisPanel.Children.Contains(item))
                    {
                        axisPanel.Children.Add(item);
                    }
                }

                if (Area != null)
                    UpdateArea();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ChartAxisScaleBreak item in e.OldItems)
                {
                    item.PropertyChanged -= Scalebreak_PropertyChanged;
                    if (axisPanel != null && axisPanel.Children.Contains(item))
                    {
                        axisPanel.Children.Remove(item);
                    }
                }

                UpdateArea();
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (Area.AdorningCanvas != null)
                {
                    UIElement[] axisPanelCollection = new UIElement[axisPanel.Children.Count];
                    axisPanel.Children.CopyTo(axisPanelCollection, 0);
                    foreach (ChartAxisScaleBreak item in axisPanelCollection.Where(it => (it is ChartAxisScaleBreak)))
                    {
                        if (axisPanel.Children.Contains(item))
                        {
                            item.PropertyChanged -= Scalebreak_PropertyChanged;
                            axisPanel.Children.Remove(item);
                        }
                    }

                    UpdateArea();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var item = e.OldItems[0] as ChartAxisScaleBreak;
                if (item != null && axisPanel != null && axisPanel.Children.Contains(item))
                {
                    item.PropertyChanged -= Scalebreak_PropertyChanged;
                    axisPanel.Children.Remove(item);
                }

                item = e.NewItems[0] as ChartAxisScaleBreak;
                if (item != null && axisPanel != null && !axisPanel.Children.Contains(item))
                {
                    item.PropertyChanged += Scalebreak_PropertyChanged;
                    axisPanel.Children.Add(item);
                }

                UpdateArea();
            }
        }

        private void UpdateArea()
        {
            if (Area != null)
                Area.ScheduleUpdate();
        }

        private void OnMinMaxChanged()
        {
            NumericalAxisHelper.OnMinMaxChanged(this, Maximum, Minimum);
        }
        
        private void ComputeBreakRange(DoubleRange currentRange, int index)
        {
            bool canAdd = true;
            for (int i = 0; i < this.BreakRanges.Count;)
            {
                DoubleRange previous = this.BreakRanges[i];
                if (currentRange.Inside(previous))
                {
                    BreakRangesInfo.Remove(BreakRanges[i]);
                    this.BreakRanges.RemoveAt(i);
                }
                else if (previous.Inside(currentRange))
                {
                    canAdd = false;
                    break;
                }
                else if (previous.Intersects(currentRange))
                {
                    currentRange += previous;
                    BreakRangesInfo.Remove(BreakRanges[i]);
                    this.BreakRanges.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (!canAdd) return;
            if (currentRange.Delta == 0) return;
            this.BreakRanges.Add(currentRange);
            BreakRangesInfo.Add(currentRange, AxisScaleBreaks[index]);
        }
        
        private double CalculateCoefficientToValue(double value)
        {
            double length;
            double result = double.NaN;
            double coeffValue = 0;
            double spacing = 5;
            length = this.Orientation == Orientation.Horizontal ? AvailableSize.Width :
                    AvailableSize.Height;

            for (int i = 0; i < AxisRanges.Count; i++)
            {
                var brkCoeff = DoubleRange.Empty;
                if (BreakRanges != null && i < BreakRanges.Count)
                {
                    spacing = this.BreakRangesInfo[BreakRanges[i]].BreakSpacing;
                    brkCoeff = new DoubleRange(ValueToCoefficient(BreakRanges[i].Start), ValueToCoefficient(BreakRanges[i].End));
                }

                var coeffRange = new DoubleRange(ValueToCoefficient(AxisRanges[i].Start), ValueToCoefficient(AxisRanges[i].End));

                if (coeffRange.Inside(value))
                {
                    result = AxisRanges[i].Start + (AxisRanges[i].Delta * ((value - coeffValue) / (AxisLength[i] / length)));
                    break;
                }

                if (value > 1)
                {
                    result = AxisRanges[AxisRanges.Count - 1].Start + (AxisRanges[AxisRanges.Count - 1].Delta * ((value - coeffValue) / (AxisLength.Last() / length)));
                    break;
                }

                if (value < 0)
                {
                    result = AxisRanges[0].Start + (AxisRanges[0].Delta * ((value - coeffValue) / (AxisLength.First() / length)));
                    break;
                }

                if (i < BreakRanges.Count && value > brkCoeff.Start && value < brkCoeff.End)
                {
                    result = BreakRanges[i].Start + (BreakRanges[i].Delta * ((value - coeffValue) / BreakRangesInfo[BreakRanges[i]].BreakSpacing / length));
                    break;
                }

                coeffValue += ((AxisLength[i] + spacing) / length);
            }

            return result;
        }

        internal double CalculateValueToCoefficient(double value)
        {
            double result = 0;
            double length;
            double spacing = 5;
            DoubleRange visibleActualRange = DoubleRange.Empty;

            length = this.Orientation == Orientation.Horizontal ? AvailableSize.Width
                      : AvailableSize.Height;

            foreach (var range in AxisRanges)
            {
                visibleActualRange += range;
            }

            for (int i = 0; i < AxisRanges.Count; i++)
            {
                if (BreakRanges != null && i < BreakRanges.Count)
                    spacing = this.BreakRangesInfo[BreakRanges[i]].BreakSpacing;

                if (visibleActualRange.Inside(value))
                {
                    if (AxisRanges[i].Inside(value))
                    {
                        var start = AxisRanges[i].Start;
                        var delta = AxisRanges[i].Delta;
                        result += (AxisLength[i] / length)
                                     * (value - start) / delta;
                        break;
                    }

                    if (BreakRanges != null && i < BreakRanges.Count && value > BreakRanges[i].Start && value < BreakRanges[i].End)
                    {
                        result += ((AxisLength[i]) / length);
                        result += (((value - BreakRanges[i].Start)) / BreakRanges[i].Delta) * (spacing / length);
                        break;
                    }

                    result += ((AxisLength[i] + spacing) / length);
                }
                else
                {
                    result = (value - visibleActualRange.Start) / visibleActualRange.Delta;
                }
            }

            return result;
        }

        private void GenerateDefaultLabel()
        {
            VisibleLabels.Clear();
            SmallTickPoints.Clear();
            NumericalAxisHelper.GenerateVisibleLabels(this, Minimum, Maximum, Interval, SmallTicksPerInterval);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void GetSeriesYValues(ObservableCollection<ISupportAxes> registeredSeries)
        {
            foreach (var series in registeredSeries)
            {
                dataPoints = new List<double>();
                if (series.ActualYAxis == this)
                {
                    if (series is BoxAndWhiskerSeries)
                    {
                        var boxSeriesYVal = (series as BoxAndWhiskerSeries).YCollection;
                        if (boxSeriesYVal != null)
                        {
                            for (int k = 0; k < boxSeriesYVal.Count(); k++)
                            {
                                foreach (var value in boxSeriesYVal[k])
                                {
                                    dataPoints.Add(value);
                                }
                            }
                        }
                    }
                    else if (series is WaterfallSeries || series is ErrorBarSeries)
                    {
                        var ser = series as WaterfallSeries;
                        var segments = ser == null ? (series as ErrorBarSeries).Segments :
                                     (series as WaterfallSeries).Segments;
                        foreach (var seg in segments)
                        {
                            if (!(dataPoints.Contains(seg.YRange.Start)))
                                dataPoints.Add(seg.YRange.Start);
                            if (!(dataPoints.Contains(seg.YRange.End)))
                                dataPoints.Add(seg.YRange.End);
                        }
                    }
                    else
                    {
                        var cartesianSeries = series as CartesianSeries;
                        if (cartesianSeries != null && cartesianSeries.ActualSeriesYValues != null)
                        {
                            for (int k = 0; k < cartesianSeries.ActualSeriesYValues.Count(); k++)
                            {
                                foreach (var value in cartesianSeries.ActualSeriesYValues[k])
                                {
                                    dataPoints.Add(value);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ComputeAxisHeight(List<DoubleRange> axisSegment, double delta)
        {
            if (axisSegment != null && axisSegment.Count == 0) return;
            AxisLength = new List<double>();
            double length = 0, availableLength = 0, overallDataCount = 0;
            double spacing = 5;
            double breakPercent = 50;

            availableLength = this.Orientation == Orientation.Horizontal ? AvailableSize.Width
                        : AvailableSize.Height;

            foreach (var range in axisSegment)
            {
                foreach (var item in dataPoints)
                {
                    if (range.Inside(item))
                        overallDataCount++;
                }
            }

            for (int i = 0; i < axisSegment.Count; i++)
            {
                if (i < BreakRanges.Count)
                {
                    spacing = this.BreakRangesInfo[BreakRanges[i]].BreakSpacing;
                    breakPercent = this.BreakRangesInfo[BreakRanges[i]].BreakPercent;
                    if (breakPercent < 0 || breakPercent > 100)
                        breakPercent = 50;
                }

                int currentCount = dataPoints.Count(item => axisSegment[i].Inside(item));
                switch (BreakPosition)
                {
                    case ScaleBreakPosition.Scale:
                        length = (availableLength * axisSegment[i].Delta) / delta;
                        break;

                    case ScaleBreakPosition.Percent:
                        if (BreakRanges.Count > i)
                        {
                            length = (availableLength * breakPercent) / 100;
                        }
                        else
                        {
                            length = availableLength;
                        }

                        availableLength = availableLength - length;
                        break;

                    default:
                        length = axisSegment.Count == 1 ? availableLength :
                            (((0.5 / axisSegment.Count) + ((currentCount / overallDataCount) * 0.5))) * availableLength;
                        break;
                }

                if (BreakRanges.Count > 0)
                {
                    if (i == 0 || i == axisSegment.Count - 1)
                    {
                        length = length - spacing / 2;
                    }
                    else
                    {
                        length = length - (BreakRangesInfo[BreakRanges[i - 1]].BreakSpacing / 2 + BreakRangesInfo[BreakRanges[i]].BreakSpacing / 2);
                    }
                }

                length = length < 0 ? 0 : length;
                if (AxisLength != null)
                    AxisLength.Add(length);
            }

            AxisRanges = axisSegment;
        }

        private void AddDataRange()
        {
            dataRanges = new List<DoubleRange>();
            var sortedRange = BreakRanges.OrderBy(i => i.Start).ToList();
            BreakRanges.Clear();
            BreakRanges = sortedRange.ToList();

            for (int i = 0; i < sortedRange.Count; i++)
            {
                if (i == 0)
                {
                    if (!(sortedRange[i].Start <= VisibleRange.Start))
                        dataRanges.Add(new DoubleRange(VisibleRange.Start, sortedRange[i].Start));
                    else
                    {
                        BreakRanges.RemoveAt(i);
                    }
                }
                else
                    dataRanges.Add(new DoubleRange(sortedRange[i - 1].End, sortedRange[i].Start));
            }

            if (sortedRange.Count > 0)
            {
                if (!(sortedRange[sortedRange.Count - 1].End >= VisibleRange.End))
                    dataRanges.Add(new DoubleRange(sortedRange[sortedRange.Count - 1].End, VisibleRange.End));
                else
                {
                    BreakRanges.Remove(sortedRange[sortedRange.Count - 1]);
                }
            }
        }

        #endregion
        
        #endregion
    }
}
