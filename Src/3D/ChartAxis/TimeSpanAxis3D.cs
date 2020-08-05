// <copyright file="TimeSpanAxis3D.cs" company="Syncfusion. Inc">
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
    /// Class implementation for TimeSpanAxis
    /// </summary>
    public partial class TimeSpanAxis3D : RangeAxisBase3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(object), 
                typeof(TimeSpanAxis3D),
                new PropertyMetadata(null, OnIntervalChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(object), 
                typeof(TimeSpanAxis3D), 
                new PropertyMetadata(null, OnMinimumChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(object), 
                typeof(TimeSpanAxis3D), 
                new PropertyMetadata(null, OnMaximumChanged));

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets interval property
        /// </summary>
        public object Interval
        {
            get { return this.GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets Minimum Property
        /// </summary>
        public object Minimum
        {
            get { return this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }
    
        /// <summary>
        /// Gets or sets Maximum Property
        /// </summary>
        public object Maximum
        {
            get { return this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }
        #endregion

        #region Methods

        #region Public Methods
        
        /// <summary>
        /// Return Object from the given double value. 
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>The label content.</returns>
        public override object GetLabelContent(double position)
        {
            return TimeSpan.FromMilliseconds(position).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Calculates the actual interval.
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>The calculated actual interval.</returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (this.Interval == null)
            {
                return this.CalculateNiceInterval(range, availableSize);
            }
            var timeSpan = this.Interval is TimeSpan ? (TimeSpan)this.Interval : TimeSpan.Parse(this.Interval.ToString());
            return timeSpan.TotalMilliseconds;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            TimeSpanAxisHelper.GenerateVisibleLabels3D(this, this.Minimum, this.Maximum, this.Interval);
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
        /// Method implementation for Minimum property changed
        /// </summary>
        /// <param name="args">The Event Arguments</param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            this.OnMinMaxChanged();
        }

        /// <summary>
        /// Called when Interval property changed
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
        /// Calculates actual range.
        /// </summary>
        /// <returns>Returns the actual range.</returns>
        protected override DoubleRange CalculateActualRange()
        {
            if (this.Minimum == null && this.Maximum == null) 
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
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? TimeSpan.MaxValue.TotalMilliseconds : range.End);
                }
                else if (this.Maximum != null)
                {
                    return new DoubleRange(double.IsNaN(range.Start) ? TimeSpan.MinValue.TotalMilliseconds : range.Start, ActualRange.End);
                }

                return range;
            }
        }

        /// <summary>
        /// Apply padding based on interval.
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="interval">The Interval</param>
        /// <returns>Returns the padded range.</returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (this.Minimum == null && this.Maximum == null) 
            {
                // Executes when Minimum and Maximum aren't set.
                return base.ApplyRangePadding(range, interval);
            }
            else if (this.Minimum != null && this.Maximum != null) 
            {
                // Executes when Minimum and Maximum are set.
                return range;
            }
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = base.ApplyRangePadding(range, interval);
                return this.Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }
        
        /// <summary>
        /// Clones the <see cref="TimeSpanAxis3D"/>.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the cloned <see cref="TimeSpanAxis3D"/>.</returns>
        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var timeSpanAxis3D = new TimeSpanAxis3D();
            timeSpanAxis3D.Interval = this.Interval;
            timeSpanAxis3D.Minimum = this.Minimum;
            timeSpanAxis3D.Maximum = this.Maximum;
            obj = timeSpanAxis3D;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the <see cref="TimeSpanAxis3D"/> on minimum property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments.</param>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimeSpanAxis3D)d).OnMinimumChanged(e);
        }

        /// <summary>
        /// Updates the <see cref="TimeSpanAxis3D"/> on maximum property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimeSpanAxis3D)d).OnMaximumChanged(e);
        }
        
        /// <summary>
        /// Updates the <see cref="TimeSpanAxis3D"/> on interval value changed.
        /// </summary>
        /// <param name="d">The Dependency Property</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimeSpanAxis3D)d).OnIntervalChanged(e);
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Updates the <see cref="TimeSpanAxis3D"/> when the minimum or maximum value changed.
        /// </summary>
        private void OnMinMaxChanged()
        {
            if (this.Minimum != null || this.Maximum != null)
            {
                TimeSpan minTimeSpan = this.Minimum is TimeSpan ? (TimeSpan)this.Minimum : (this.Minimum != null ? TimeSpan.Parse(this.Minimum.ToString()) : TimeSpan.MinValue);
                TimeSpan maxTimeSpan = this.Maximum is TimeSpan ? (TimeSpan)this.Maximum : (this.Maximum != null ? TimeSpan.Parse(this.Maximum.ToString()) : TimeSpan.MaxValue);
                this.ActualRange = new DoubleRange(minTimeSpan.TotalMilliseconds, maxTimeSpan.TotalMilliseconds);
            }

            if (this.Area != null)
            {
                this.Area.ScheduleUpdate();
            }
        }
        
        #endregion

        #endregion
    }
}
