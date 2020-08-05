using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for LogarithmicAxis
    /// </summary>
    public partial class LogarithmicAxis : RangeAxisBase
    {
        #region Dependancy Property Registration

#if NETFX_CORE

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval",
                typeof(object),
                typeof(LogarithmicAxis),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(object),
                typeof(LogarithmicAxis),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(object),
                typeof(LogarithmicAxis),
                new PropertyMetadata(null, OnMaximumChanged));

#else

        /// <summary>
        ///  The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(double?),
                typeof(LogarithmicAxis),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(double?), 
                typeof(LogarithmicAxis),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(double?), 
                typeof(LogarithmicAxis),
                new PropertyMetadata(null, OnMaximumChanged));

#endif

        /// <summary>
        /// The DependencyProperty for <see cref="LogarithmicBase"/> property.
        /// </summary>
        public static readonly DependencyProperty LogarithmicBaseProperty =
            DependencyProperty.Register(
                "LogarithmicBase", 
                typeof(double), 
                typeof(ChartAxis),
                new PropertyMetadata(10d, OnLogarithmicAxisValueChanged));

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for LogarithmicAxis 
        /// </summary>
        public LogarithmicAxis()
        {
            IsLogarithmic = true;
        }

        #endregion

        #region Properties       

#region Public Properties

#if NETFX_CORE
        /// <summary>
        /// Gets or sets a value that determines the interval between labels. Its nullable.
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
        /// Gets or sets the base for the <c>LogarithmicAxis</c>.
        /// </summary>
        /// <value>
        /// <c>2</c> for binary logarithm,
        /// <c>10</c> for common logarithm.
        /// </value>
        public double LogarithmicBase
        {
            get { return (double)GetValue(LogarithmicBaseProperty); }
            set { SetValue(LogarithmicBaseProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

                /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        public override double CoefficientToValue(double value)
        {
            return Math.Pow(LogarithmicBase, base.CoefficientToValue(value));
        }

        /// <summary>
        /// Return the object Value from the given double value
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            // The value is converter to its corresponding log value before passing through this method.
            if (VisibleLabels != null && (CustomLabels.Count > 0 || LabelsSource != null))
            {
                position = Math.Log(position, LogarithmicBase);
                return GetCustomLabelContent(position) ?? GetActualLabelContent(position);
            }
            else
                return GetActualLabelContent(position);
        }

        #endregion

        #region Internal Override Methods

        internal override void UpdateAutoScrollingDelta(double autoScrollingDelta)
        {
            var visibleRange = new DoubleRange(ActualRange.End - (double)AutoScrollingDelta, VisibleRange.End);
            ZoomFactor = (float)(visibleRange.Delta / VisibleRange.Delta);
            ZoomPosition = 1 - ZoomFactor;
        }

        internal override object GetActualLabelContent(double position)
        {
            return Math.Round(
                Math.Pow(
                    this.LogarithmicBase, 
                    Math.Log(position, LogarithmicBase)),
               CRoundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Calculates nice interval
        /// </summary>
        /// <param name="actualRange"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal override double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {
            return LogarithmicAxisHelper.CalculateNiceInterval(this, actualRange, availableSize);
        }

        /// <summary>
        /// Method implementation for Add SmallTicks for axis
        /// </summary>
        /// <param name="position"></param>
        /// <param name="logarithmicBase"></param>
        protected internal override void AddSmallTicksPoint(double position, double logarithmicBase)
        {
            LogarithmicAxisHelper.AddSmallTicksPoint(this, position, logarithmicBase, SmallTicksPerInterval);
        }
        
        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (Interval == null)
                return CalculateNiceInterval(range, availableSize);
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
            LogarithmicAxisHelper.CalculateVisibleRange(this, avalableSize, Interval);
        }

        #endregion

        #region Protected Override Methods
        
        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            SetRangeForAxisStyle();
            LogarithmicAxisHelper.GenerateVisibleLabels(this, Minimum, Maximum, Interval, LogarithmicBase);
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected override DoubleRange CalculateActualRange()
        {
            if (Minimum == null && Maximum == null) // Executes when  Minimum and Maximum aren't set.
            {
                DoubleRange range = LogarithmicAxisHelper.CalculateActualRange(this, CalculateBaseActualRange(), LogarithmicBase);

                // Execute when include the annotion range for Auto Range
                if (IncludeAnnotationRange)
                {
                    foreach (var annotation in (Area as SfChart).Annotations)
                    {
                        if (Orientation == Orientation.Vertical && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.YAxis == this)
                            range += new DoubleRange(
                                     Annotation.ConvertData(annotation.Y1, this),
                                     annotation is TextAnnotation 
                                     ? Annotation.ConvertData(annotation.Y1, this) 
                                     : annotation is ImageAnnotation 
                                        ? Annotation.ConvertData((annotation as ImageAnnotation).Y2, this) 
                                        : Annotation.ConvertData((annotation as ShapeAnnotation).Y2, this));
                        else if (Orientation == Orientation.Horizontal && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.XAxis == this)
                            range += new DoubleRange(
                                     Annotation.ConvertData(annotation.X1, this),
                                     annotation is TextAnnotation 
                                     ? Annotation.ConvertData(annotation.X1, this)
                                     : annotation is ImageAnnotation 
                                       ? Annotation.ConvertData((annotation as ImageAnnotation).X2, this) 
                                       : Annotation.ConvertData((annotation as ShapeAnnotation).X2, this));
                    }
                }

                return range;
            }
            else if (Minimum != null && Maximum != null) // Executes when Minimum and Maximum is set.
                return ActualRange;
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange range = CalculateBaseActualRange();
                range = LogarithmicAxisHelper.CalculateActualRange(this, range, LogarithmicBase);
                if (Minimum != null)
                    return new DoubleRange(ActualRange.Start, range.End);
                if (Maximum != null)
                    return new DoubleRange(range.Start, ActualRange.End);
                return range;
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
            if (Minimum == null && Maximum == null) // Executes when Minimum and Maximum aren't set.
                return base.ApplyRangePadding(range, interval);
            else if (Minimum != null && Maximum != null) // Executes when Minimum and Maximum are set.
                return range;
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = base.ApplyRangePadding(range, interval);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var logarithmicAxis = new LogarithmicAxis();
            logarithmicAxis.Minimum = this.Minimum;
            logarithmicAxis.Maximum = this.Maximum;
            logarithmicAxis.Interval = this.Interval;
            logarithmicAxis.LogarithmicBase = this.LogarithmicBase;

            obj = logarithmicAxis;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called when maximum changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        /// <summary>
        /// Called when minimum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        /// <summary>
        /// Called when Interval changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        #endregion

        #region Private Static Methods

        private static void OnLogarithmicAxisValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LogarithmicAxis).OnLogBaseChanged();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LogarithmicAxis).OnMinimumChanged(e);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LogarithmicAxis).OnMaximumChanged(e);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LogarithmicAxis).OnIntervalChanged(e);
        }

        /// <summary>
        /// Get the Actual YValues from chart series.
        /// </summary>
        /// <param name="chartSeries"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static IList<double> GetYValues(ChartSeriesBase chartSeries)
        {
            return chartSeries is XyDataSeries
                ? (chartSeries as XyDataSeries).YValues
                : chartSeries is RangeSeriesBase
                    ? (chartSeries as RangeSeriesBase).HighValues.Union((chartSeries as RangeSeriesBase).LowValues)
                        .ToList()
                    : chartSeries is FinancialSeriesBase
                        ? (chartSeries as FinancialSeriesBase).HighValues.Union(
                            (chartSeries as FinancialSeriesBase).LowValues).ToList()
                        : new List<double>();
        }

        /// <summary>
        /// Get the range if date values contain 0 or double.NaN
        /// </summary>
        /// <param name="values"></param>
        /// <param name="rangeEnd"></param>
        /// <returns></returns>
        private static DoubleRange GetRange(List<double> values, double rangeEnd)
        {
            if (values.All(value => double.IsNaN(value) || value <= 0))
                return DoubleRange.Empty;
            else
            {
                var valueCollection = (from value in values where value > 0 select value);
                if (valueCollection.Any())
                {
                    var minimum = valueCollection.Min();
                    if (minimum > 0 && minimum < 1)
                        return new DoubleRange(minimum, rangeEnd);
                    else
                        return new DoubleRange(1, rangeEnd);
                }

                return DoubleRange.Empty;
            }
        }

        #endregion

        #region Private Methods

        private void OnLogBaseChanged()
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        private void OnMinMaxChanged()
        {
            LogarithmicAxisHelper.OnMinMaxChanged(this, Minimum, Maximum, LogarithmicBase);
        }

        /// <summary>
        /// Calculate base actual range.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private DoubleRange CalculateBaseActualRange()
        {
            if (Area != null)
            {
                var technicalIndicators = new List<ChartSeriesBase>();

                if (Area is SfChart)
                {
                    foreach (ChartSeries indicator in (Area as SfChart).TechnicalIndicators)
                    {
                        technicalIndicators.Add(indicator as ChartSeriesBase);
                    }
                }

                foreach (var chartSeries in (Area.VisibleSeries.Union(technicalIndicators)).Where(
                        series => series.ActualXAxis == this || series.ActualYAxis == this))
                {
                    var yValues = GetYValues(chartSeries);
                    if (chartSeries.ActualYAxis == this && yValues.Count > 0 && (double.IsNaN(yValues.Min()) || yValues.Min() <= 0))
                    {
                        if (chartSeries is PolarRadarSeriesBase)
                            (chartSeries as PolarRadarSeriesBase).YRange = GetRange(yValues as List<double>, (chartSeries as PolarRadarSeriesBase).YRange.End);
                        else
                            (chartSeries as CartesianSeries).YRange = GetRange(yValues as List<double>, (chartSeries as CartesianSeries).YRange.End);
                    }

                    var xValues = chartSeries.ActualXValues as List<double>;
                    if (xValues != null && (chartSeries.ActualXAxis == this && xValues.Count > 0 && ((double.IsNaN(xValues.Min())) || xValues.Min() <= 0)))
                    {
                        if (chartSeries is PolarRadarSeriesBase)
                            (chartSeries as PolarRadarSeriesBase).XRange = GetRange(xValues, (chartSeries as PolarRadarSeriesBase).XRange.End);
                        else
                            (chartSeries as CartesianSeries).XRange = GetRange(xValues, (chartSeries as CartesianSeries).XRange.End);
                    }
                }

                return Area.VisibleSeries.OfType<ISupportAxes>()
                    .Select
                    (series =>
                    {
                        if (series.ActualXAxis == this)
                            return series.XRange;
                        if (series.ActualYAxis == this)
                            return series.YRange;
                        return DoubleRange.Empty;
                    }).Sum();
            }

            return DoubleRange.Empty;
        }

        #endregion
              
        #endregion
    }
}
