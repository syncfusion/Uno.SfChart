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
    /// Class implementation for TimeSpanAxis
    /// </summary>
    public partial class TimeSpanAxis : RangeAxisBase
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
                typeof(TimeSpanAxis),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(object),
                typeof(TimeSpanAxis),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(object),
                typeof(TimeSpanAxis),
                new PropertyMetadata(null, OnMaximumChanged));

#else
        
        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval",
                typeof(TimeSpan?),
                typeof(TimeSpanAxis),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(TimeSpan?), 
                typeof(TimeSpanAxis),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(TimeSpan?), 
                typeof(TimeSpanAxis),
                new PropertyMetadata(null, OnMaximumChanged));

#endif

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
        /// Gets or sets the minimum value for the timespan axis range. This is nullable property.
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
        public TimeSpan? Interval
        {
            get { return (TimeSpan?)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value for the timespan axis range. This is nullable property.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the minimum value, it will be calculate from the underlying collection.
        /// </remarks>
        public TimeSpan? Minimum
        {
            get { return (TimeSpan?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value for the axis range. This is nullable property. 
        /// </summary>
        /// <remarks>
        ///     If we didn't set the maximum value, it will be calculate from the underlying collection.
        /// </remarks>
        public TimeSpan? Maximum
        {
            get { return (TimeSpan?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

#endif

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Return Object from the given double value 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            if (CustomLabels.Count > 0 || LabelsSource != null)
            {
                return GetCustomLabelContent(position) ?? GetActualLabelContent(position);
            }
            else
                return GetActualLabelContent(position);
        }

        #endregion

        #region Internal Override Methods
        internal override object GetActualLabelContent(double position)
        {
            return TimeSpan.FromMilliseconds(position).ToString(LabelFormat, CultureInfo.CurrentCulture);
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size avalableSize)
        {
            base.CalculateVisibleRange(avalableSize);
            TimeSpanAxisHelper.CalculateVisibleRange(this, Interval, avalableSize);
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
                return base.CalculateNiceInterval(range, availableSize);
#if NETFX_CORE 
            TimeSpan timeSpan = Interval is TimeSpan ? (TimeSpan)Interval : TimeSpan.Parse(Interval.ToString());
            return timeSpan.TotalMilliseconds;
#else
            return Interval.Value.TotalMilliseconds;
#endif
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
        /// Method implementation for Minimum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        /// <summary>
        /// Called when Interval property changed
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
            TimeSpanAxisHelper.GenerateVisibleLabels(this, Minimum, Maximum, Interval);
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected override DoubleRange CalculateActualRange()
        {
            if (Minimum == null && Maximum == null) // Executes when Minimum and Maximum aren't set.
            {
                DoubleRange range = base.CalculateActualRange();

                if (Orientation == Orientation.Horizontal &&
                    IncludeAnnotationRange && Area != null)
                {
                    foreach (var annotation in (Area as SfChart).Annotations)
                    {
                        if (annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.XAxis == this)
                            range += new DoubleRange(
                                (Annotation.ConvertData(annotation.X1, this) == 0d 
                                ? range.Start : Annotation.ConvertData(annotation.X1, this)),
                                (annotation is TextAnnotation 
                                ? (Annotation.ConvertData(annotation.X1, this) == 0d 
                                ? range.Start : Annotation.ConvertData(annotation.X1, this)) 
                                : annotation is ImageAnnotation 
                                ? (Annotation.ConvertData((annotation as ImageAnnotation).X2, this) == 0d
                                ? range.Start : Annotation.ConvertData((annotation as ImageAnnotation).X2, this)) 
                                : (Annotation.ConvertData((annotation as ShapeAnnotation).X2, this) == 0d 
                                ? range.Start : Annotation.ConvertData((annotation as ShapeAnnotation).X2, this))));
                    }
                }

                return range;
            }
            else if (Minimum != null && Maximum != null) // Executes when Minimum and Maximum are set.
                return ActualRange;
            else
            {
                // Executes when either Minimum or Maximum.
                DoubleRange range = base.CalculateActualRange();
                if (Minimum != null)
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? ActualRange.Start + 1 : range.End);
                else if (Maximum != null)
                    return new DoubleRange(double.IsNaN(range.Start) ? ActualRange.End - 1 : range.Start, ActualRange.End);
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
            var timeSpanAxis = new TimeSpanAxis();
            timeSpanAxis.Interval = this.Interval;
            timeSpanAxis.Minimum = this.Minimum;
            timeSpanAxis.Maximum = this.Maximum;

            obj = timeSpanAxis;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimeSpanAxis).OnMinimumChanged(e);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimeSpanAxis).OnMaximumChanged(e);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimeSpanAxis).OnIntervalChanged(e);
        }

        #endregion

        #region Private Methods

        private void OnMinMaxChanged()
        {
            if (Minimum != null || Maximum != null)
            {
#if NETFX_CORE
                TimeSpan minTimeSpan = Minimum is TimeSpan ? (TimeSpan)Minimum : (Minimum != null ? TimeSpan.Parse(Minimum.ToString()) : TimeSpan.MinValue);
                TimeSpan maxTimeSpan = Maximum is TimeSpan ? (TimeSpan)Maximum : (Maximum != null ? TimeSpan.Parse(Maximum.ToString()) : TimeSpan.MaxValue);
                ActualRange = new DoubleRange(minTimeSpan.TotalMilliseconds, maxTimeSpan.TotalMilliseconds);
#else
                double minTimeSpan = Minimum == null ? TimeSpan.MinValue.TotalMilliseconds : Minimum.Value.TotalMilliseconds;
                double maxTimeSpan = Maximum == null ? TimeSpan.MaxValue.TotalMilliseconds : Maximum.Value.TotalMilliseconds;
                ActualRange = new DoubleRange(minTimeSpan, maxTimeSpan);
#endif
            }

            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        #endregion

        #endregion
    }
}
