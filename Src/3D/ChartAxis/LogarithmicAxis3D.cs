// <copyright file="LogarithmicAxis3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Windows.Foundation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for LogarithmicAxis3D
    /// </summary>
    public partial class LogarithmicAxis3D : RangeAxisBase3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(object), 
                typeof(LogarithmicAxis3D),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(object), 
                typeof(LogarithmicAxis3D),
                new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(object),
                typeof(LogarithmicAxis3D),
                new PropertyMetadata(null, OnMaximumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LogarithmicBase"/> property.
        /// </summary>
        public static readonly DependencyProperty LogarithmicBaseProperty =
            DependencyProperty.Register(
                "LogarithmicBase", 
                typeof(double), 
                typeof(LogarithmicAxis3D),
                new PropertyMetadata(10d, OnLogarithmicAxisValueChanged));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogarithmicAxis3D"/> class.
        /// </summary>
        public LogarithmicAxis3D()
        {
            this.IsLogarithmic = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value that determines the interval between labels.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public object Interval
        {
            get { return (object)GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value for the axis range.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the minimum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Minimum
        {
            get { return (object)GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value for the axis range.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the maximum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Maximum
        {
            get { return (object)GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the base for the <c>LogarithmicAxis3D</c>.
        /// </summary>
        /// <value>
        /// <c>2</c> for binary logarithm,
        /// <c>10</c> for common logarithm.
        /// </value>
        public double LogarithmicBase
        {
            get { return (double)GetValue(LogarithmicBaseProperty); }
            set { this.SetValue(LogarithmicBaseProperty, value); }
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.ValueToCoefficientCalc(double)"/>
        public override double CoefficientToValue(double value)
        {
            return Math.Pow(this.LogarithmicBase, base.CoefficientToValue(value));
        }

        /// <summary>
        /// Return the object Value from the given double value.
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>Returns the label content.</returns>
        public override object GetLabelContent(double position)
        {
            return Math.Round(Math.Pow(this.LogarithmicBase, Math.Log(position, this.LogarithmicBase)), CRoundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Calculates nice interval.
        /// </summary>
        /// <param name="actualRange">The Actual Range</param>
        /// <param name="availableSize">The Available Range</param>
        /// <returns>Returns the calculated nice interval.</returns>
        protected internal override double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {
            return LogarithmicAxisHelper.CalculateNiceInterval(this, actualRange, availableSize);
        }

        /// <summary>
        /// Method implementation for Add SmallTicks for axis
        /// </summary>
        /// <param name="position">The Position</param>
        /// <param name="logarithmicBase">The Logarithmic Base</param>
        protected internal override void AddSmallTicksPoint(double position, double logarithmicBase)
        {
            LogarithmicAxisHelper.AddSmallTicksPoint(this, position, logarithmicBase, this.SmallTicksPerInterval);
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the calculated actual range.</returns>
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
            LogarithmicAxisHelper.GenerateVisibleLabels3D(this, this.Minimum, this.Maximum, this.Interval, this.LogarithmicBase);
        }

        /// <summary>
        /// Called when maximum changed
        /// </summary>
        /// <param name="args">The Event Arguments</param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            this.OnMinMaxChanged();
        }

        /// <summary>
        /// Called when minimum property changed
        /// </summary>
        /// <param name="args">The Event Arguments</param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            this.OnMinMaxChanged();
        }

        /// <summary>
        /// Called when Interval changed
        /// </summary>
        /// <param name="e">The Event Argument</param>
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
        /// <returns>Returns the calculated range.</returns>
        protected override DoubleRange CalculateActualRange()
        {
            if (this.Minimum == null && this.Maximum == null) 
            {
                // Executes when Minimum and Maximum aren't set.
                return LogarithmicAxisHelper.CalculateActualRange(this, base.CalculateActualRange(), this.LogarithmicBase);
            }
            else if (this.Minimum != null && this.Maximum != null)
            {
                // Executes when Minimum and Maximum aren set.
                return this.ActualRange;
            }
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange range = this.CalculateBaseActualRange();
                range = LogarithmicAxisHelper.CalculateActualRange(this, range, this.LogarithmicBase);
                if (this.Minimum != null)
                {
                    return new DoubleRange(ActualRange.Start, range.End);
                }
                else if (this.Maximum != null)
                {
                    return new DoubleRange(range.Start, ActualRange.End);
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
                return base.ApplyRangePadding(range, interval);
            }
            else if (this.Minimum != null && this.Maximum != null) 
            {
                // Executes when Minimum and Maximum are set.
                return range;
            }
            else
            {
                // Executes when either Minimum and Maximum is set.
                DoubleRange baseRange = base.ApplyRangePadding(range, interval);
                return this.Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        /// <summary>
        /// Clones the logarithmic axis.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the cloned axis.</returns>
        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var logarithmicAxis3D = new LogarithmicAxis3D();
            logarithmicAxis3D.Interval = this.Interval;
            logarithmicAxis3D.Minimum = this.Minimum;
            logarithmicAxis3D.Maximum = this.Maximum;
            logarithmicAxis3D.LogarithmicBase = this.LogarithmicBase;
            obj = logarithmicAxis3D;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Get the range if date values contain 0 or double.NaN
        /// </summary>
        /// <param name="values">The Values</param>
        /// <param name="rangeEnd">The Range End</param>
        /// <returns>Returns the range.</returns>
        private static DoubleRange GetRange(List<double> values, double rangeEnd)
        {
            if (values.All(value => double.IsNaN(value) || value <= 0))
            {
                return DoubleRange.Empty;
            }
            else
            {
                var valueCollection = (from value in values where value > 0 select value);
                if (valueCollection.Any())
                {
                    var minimum = valueCollection.Min();
                    if (minimum > 0 && minimum < 1)
                    {
                        return new DoubleRange(minimum, rangeEnd);
                    }
                    else
                    {
                        return new DoubleRange(1, rangeEnd);
                    }
                }

                return DoubleRange.Empty;
            }
        }

        /// <summary>
        /// Updates the axis when the logarithmic base changed. 
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnLogarithmicAxisValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Updates the axis on minimum value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LogarithmicAxis3D)d).OnMinimumChanged(e);
        }

        /// <summary>
        /// Updates the axis when the maximum value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LogarithmicAxis3D)d).OnMaximumChanged(e);
        }

        /// <summary>
        /// Updates the axis when the intervals changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LogarithmicAxis3D)d).OnIntervalChanged(e);
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Updates the axis when minimum and maximum value changed.
        /// </summary>
        private void OnMinMaxChanged()
        {
            LogarithmicAxisHelper.OnMinMaxChanged(this, this.Minimum, this.Maximum, this.LogarithmicBase);
        }

        /// <summary>
        /// Calculates the base actual range.
        /// </summary>
        /// <returns>Returns the base actual range.</returns>
        private DoubleRange CalculateBaseActualRange()
        {
            if (this.Area != null)
            {
                foreach (var series in Area.VisibleSeries.OfType<XyDataSeries>())
                {
                    if (series.ActualYAxis == this && series.YValues.Count > 0 && (double.IsNaN(series.YValues.Min()) || series.YValues.Min() <= 0))
                    {
                        series.YRange = LogarithmicAxis3D.GetRange(series.YValues as List<double>, series.YRange.End);
                    }

                    var xValues = series.ActualXValues as List<double>;

                    if (xValues != null && (series.ActualXAxis == this && xValues.Count > 0 && ((double.IsNaN(xValues.Min())) || xValues.Min() <= 0)))
                    {
                        series.XRange = LogarithmicAxis3D.GetRange(xValues, series.XRange.End);
                    }
                }

                return Area.VisibleSeries.OfType<ISupportAxes>()
                    .Select(
                    series =>
                    {
                        if (series.ActualXAxis == this)
                        {
                            return series.XRange;
                        }

                        if (series.ActualYAxis == this)
                        {
                            return series.YRange;
                        }

                        return DoubleRange.Empty;
                    }).Sum();
            }

            return DoubleRange.Empty;
        }

        #endregion
        
        #endregion
    }
}
