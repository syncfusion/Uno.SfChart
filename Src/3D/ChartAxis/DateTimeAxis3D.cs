// <copyright file="DateTimeAxis3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Globalization;
    using Windows.Foundation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for DateTimeAxis3D
    /// </summary>
    public partial class DateTimeAxis3D : RangeAxisBase3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(object), 
                typeof(DateTimeAxis3D),
                new PropertyMetadata(null, OnMinimumChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(object), 
                typeof(DateTimeAxis3D), 
                new PropertyMetadata(null, OnMaximumChanged));
        

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(double),
                typeof(DateTimeAxis3D),
                new PropertyMetadata(0d, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RangePadding"/> property.
        /// </summary>
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register(
                "RangePadding", 
                typeof(DateTimeRangePadding), 
                typeof(DateTimeAxis3D),
                new PropertyMetadata(DateTimeRangePadding.Auto, OnRangePaddingChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="IntervalType"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalTypeProperty =
            DependencyProperty.Register(
                "IntervalType", 
                typeof(DateTimeIntervalType), 
                typeof(DateTimeAxis3D),
                new PropertyMetadata(DateTimeIntervalType.Auto, OnIntervalTypeChanged));
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the minimum value for the axis range. This is null-able property.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the minimum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Minimum
        {
            get { return this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value for the axis range. This is null-able property. 
        /// </summary>
        /// <remarks>
        ///     If we didn't set the maximum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Maximum
        {
            get { return this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that determines the interval between labels.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }      

        /// <summary>
        /// Gets or sets a value indicating shift to the DateTimeAxis range inside or outside.
        /// </summary>
        /// <value>
        ///     <c>Additional</c>, to extend the range,
        ///     <c>Round</c>, to round-off the range,
        ///     <c>None</c>, do nothing,
        ///     <c>Auto</c>, auto range based on type of series.
        /// </value>
        public DateTimeRangePadding RangePadding
        {
            get { return (DateTimeRangePadding)GetValue(RangePaddingProperty); }
            set { this.SetValue(RangePaddingProperty, value); }
        }
     
        /// <summary>
        /// Gets or sets the type of interval to be displayed in axis.
        /// </summary>
        /// <remarks>
        ///     This property hold the values ranges from Year, Months to Milliseconds.
        /// </remarks>
        public DateTimeIntervalType IntervalType
        {
            get { return (DateTimeIntervalType)GetValue(IntervalTypeProperty); }
            set { this.SetValue(IntervalTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the actual interval type for the axis.
        /// /// </summary>
        internal DateTimeIntervalType ActualIntervalType3D { get; set; }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Return object value from the given double value
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>Return object value from the given double value.</returns>
        public override object GetLabelContent(double position)
        {           
            return position.FromOADate().ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Calculates actual interval.
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>The Actual Interval.</returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (this.Interval == 0 || double.IsNaN(this.Interval))
            {
                return this.CalculateNiceInterval(range, availableSize);
            }
            else if (this.IntervalType == DateTimeIntervalType.Auto)
            {
                this.CalculateNiceInterval(range, availableSize);
            }

            return this.Interval;
        }

        /// <summary>
        /// Calculates the nice interval.
        /// </summary>
        /// <param name="actualRange">The Actual Range</param>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the calculated nice interval.</returns>
        protected internal override double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {                      
            var dateTimeMin = actualRange.Start.FromOADate();
            var dateTimeMax = actualRange.End.FromOADate();
            var timeSpan = dateTimeMax.Subtract(dateTimeMin);

            var range = new DoubleRange(0, timeSpan.TotalDays / 365);

            var interval = base.CalculateNiceInterval(range, availableSize);

            if (interval >= 1)
            {
                this.ActualIntervalType3D = DateTimeIntervalType.Years;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);

            if (interval >= 1)
            {
                this.ActualIntervalType3D = DateTimeIntervalType.Months;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);

            if (interval >= 1)
            {
                this.ActualIntervalType3D = DateTimeIntervalType.Days;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);

            if (interval >= 1)
            {
                this.ActualIntervalType3D = DateTimeIntervalType.Hours;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);

            if (interval >= 1)
            {
                this.ActualIntervalType3D = DateTimeIntervalType.Minutes;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);

            if (interval >= 1)
            {
                this.ActualIntervalType3D = DateTimeIntervalType.Seconds;
                return interval;
            }

            interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);

            this.ActualIntervalType3D = DateTimeIntervalType.Milliseconds;
            return interval;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when Interval property changed.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
            {
                Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Calculates the actual range for the axis.
        /// </summary>
        /// <returns>Returns the calculated actual range.</returns>
        protected override DoubleRange CalculateActualRange()
        {
            if (ActualRange.IsEmpty)
            {
                // Executes when Minimum and Maximum aren't set.
                return base.CalculateActualRange();
            }
            else if (this.Minimum != null && this.Maximum != null) 
            {
                // Executes when Minimum and Maximum are set.
                return this.ActualRange;
            }
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange range = base.CalculateActualRange();
                if (this.Minimum != null)
                {
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? DateTime.MaxValue.ToOADate() : range.End);
                }
                else if (this.Maximum != null)
                {
                    return new DoubleRange(double.IsNaN(range.Start) ? DateTime.MinValue.ToOADate() : range.Start, ActualRange.End);
                }

                return range;
            }
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="interval">The Interval</param>
        /// <returns>Returns the padded range.</returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (this.Minimum == null && this.Maximum == null) 
            {
                // Executes when Minimum and Maximum aren't set.
                return DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, this.RangePadding, this.ActualIntervalType3D);
            }
            else if (this.Minimum != null && this.Maximum != null) 
            {
                // Executes when Minimum and Maximum are set.
                return range;
            }
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, this.RangePadding, this.ActualIntervalType3D);
                return this.Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        /// <summary>
        /// Clones the axis.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the cloned axis.</returns>
        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var dateTimeAxis = new DateTimeAxis3D();
            dateTimeAxis.Interval = this.Interval;
            dateTimeAxis.Minimum = this.Minimum;
            dateTimeAxis.Maximum = this.Maximum;
            dateTimeAxis.RangePadding = this.RangePadding;
            dateTimeAxis.IntervalType = this.IntervalType;
            obj = dateTimeAxis;             
            return base.CloneAxis(obj);
        }
        
        /// <summary>
        /// Called when Maximum property changed
        /// </summary>
        /// <param name="args">The Event Arguments</param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            this.OnMinMaxChanged();
        }

        /// <summary>
        /// Called when minimum property Changed
        /// </summary>
        /// <param name="args">The Event Arguments</param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            this.OnMinMaxChanged();
        }
        
        /// <summary>
        /// Method implementation to Create VisibleLabels for DateTime axis.
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            DateTimeAxisHelper.GenerateVisibleLabels3D(this, this.Minimum, this.Maximum, this.ActualIntervalType3D);
        }

        #endregion
        
        #region Private Static Methods

        /// <summary>
        /// Updates the axis when the intervals changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis3D)d).OnIntervalChanged(e);
        }

        /// <summary>
        /// Updates the axis when the maximum value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis3D)d).OnMaximumChanged(e);
        }

        /// <summary>
        /// Updates the axis when the minimum value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis3D)d).OnMinimumChanged(e);
        }

        /// <summary>
        /// Updates the axis when the range padding changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnRangePaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((DateTimeAxis3D)d).OnPropertyChanged();
            }
        }

        /// <summary>
        /// Updates the axis when the interval type changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIntervalTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeAxis3D = d as DateTimeAxis3D;
            if (dateTimeAxis3D != null && e.NewValue != null)
            {
                dateTimeAxis3D.ActualIntervalType3D = dateTimeAxis3D.IntervalType;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the axis when the minimum or maximum value is changed.
        /// </summary>
        private void OnMinMaxChanged()
        {
            DateTimeAxisHelper.OnMinMaxChanged(this, this.Minimum, this.Maximum);
        }

        #endregion

        #endregion
    }
}
