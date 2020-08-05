// <copyright file="NumericalAxis3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Class implementation for RangeAxisBase3D
    /// </summary>
    public partial class NumericalAxis3D : RangeAxisBase3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(object), 
                typeof(NumericalAxis3D),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(object), 
                typeof(NumericalAxis3D),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(object),
                typeof(NumericalAxis3D),
                new PropertyMetadata(null, OnMaximumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RangePadding"/> property.
        /// </summary>
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register(
                "RangePadding", 
                typeof(NumericalPadding), 
                typeof(NumericalAxis3D),
                new PropertyMetadata(NumericalPadding.Auto, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="StartRangeFromZero"/> property.
        /// </summary>
        public static readonly DependencyProperty StartRangeFromZeroProperty =
            DependencyProperty.Register(
                "StartRangeFromZero",
                typeof(bool),
                typeof(NumericalAxis3D),
                new PropertyMetadata(false, OnPropertyChanged));

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets a value that determines the interval between labels. Its null-able.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public object Interval
        {
            get { return this.GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets minimum property
        /// </summary>
        public object Minimum
        {
            get { return this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets Maximum property
        /// </summary>
        public object Maximum
        {
            get { return this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets property used to shift the numeric range inside or outside.
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
            set { this.SetValue(RangePaddingProperty, value); }
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
            set { this.SetValue(StartRangeFromZeroProperty, value); }
        }

        /// <summary>
        ///     Gets a NumericalPadding that describes the padding of a NumericalAxis3D.
        /// </summary>
        /// <remarks>
        ///     The NumericalPadding that is used to set the padding of the NumericalAxis3D. 
        ///     The default is <see cref="Syncfusion.UI.Xaml.Charts.NumericalPadding.Auto"/>.
        /// </remarks>
        internal NumericalPadding ActualRangePadding
        {
            get
            {
                SfChart3D area3D = Area as SfChart3D;
                if (this.RangePadding == NumericalPadding.Auto && area3D != null && area3D.Series != null && (Area as SfChart3D).Series.Count > 0)
                {
                    if ((this.Orientation == Orientation.Vertical && !area3D.Series[0].IsActualTransposed) ||
                        (this.Orientation == Orientation.Horizontal && area3D.Series[0].IsActualTransposed))
                    {
                        return NumericalPadding.Round;
                    }
                }

                return (NumericalPadding)GetValue(RangePaddingProperty);
            }           
        }

        #endregion

        #region Methods

        #region Protected Internal Methods

        /// <summary>
        /// Calculates actual interval.
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the actual interval.</returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (this.Interval == null)
            {
                return this.CalculateNiceInterval(range, availableSize);
            }
            return Convert.ToDouble(this.Interval);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            NumericalAxisHelper.GenerateVisibleLabels3D(this, this.Minimum, this.Maximum, this.Interval, this.SmallTicksPerInterval);
        }

        /// <summary>
        /// Called when Maximum property changed
        /// </summary>
        /// <param name="args">The Event Argument</param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            this.OnMinMaxChanged();
        }

        /// <summary>
        /// Called when Minimum property changed
        /// </summary>
        /// <param name="args">The Event Arguments</param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            this.OnMinMaxChanged();
        }

        /// <summary>
        /// Called when interval changed
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
        /// Calculates actual range
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
                if (this.StartRangeFromZero && range.Start > 0)
                {
                    return new DoubleRange(0, range.End);
                }
                else if (this.Minimum != null)
                {
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? ActualRange.Start + 1 : range.End);
                }
                else if (this.Maximum != null)
                {
                    return new DoubleRange(double.IsNaN(range.Start) ? ActualRange.End - 1 : range.Start, ActualRange.End);
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
                return NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, this.ActualRangePadding);
            }
            else if (this.Minimum != null && this.Maximum != null)
            {
                // Executes when Minimum and Maximum are set.
                return range;
            }
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = NumericalAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, this.ActualRangePadding);
                return this.Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        /// <summary>
        /// Clones the <see cref="NumericalAxis3D"/>.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the cloned axis.</returns>
        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var numericalAxis3D = new NumericalAxis3D();
            numericalAxis3D.Minimum = this.Minimum;
            numericalAxis3D.Maximum = this.Maximum;
            numericalAxis3D.StartRangeFromZero = this.StartRangeFromZero;
            numericalAxis3D.Interval = this.Interval;
            numericalAxis3D.RangePadding = this.RangePadding;
            obj = numericalAxis3D;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the <see cref="NumericalAxis3D"/> when anyone of it's property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericalAxis3D = d as NumericalAxis3D;
            if (numericalAxis3D != null)
            {
                numericalAxis3D.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Updates the axis when the minimum value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericalAxis3D)d).OnMinimumChanged(e);
        }

        /// <summary>
        /// Called Maximum property changed
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericalAxis3D)d).OnMaximumChanged(e);
        }

        /// <summary>
        /// Updates the axis when the intervals changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericalAxis3D)d).OnIntervalChanged(e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the axis when minimum or maximum value changed.
        /// </summary>
        private void OnMinMaxChanged()
        {
            NumericalAxisHelper.OnMinMaxChanged(this, this.Maximum, this.Minimum);
        }

        #endregion

        #endregion
    }
}
