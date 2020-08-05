// <copyright file="StackingSeriesBase3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Foundation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for StackingSeriesBase3D
    /// </summary>
    public abstract partial class StackingSeriesBase3D : XyzDataSeries3D
    {     
        #region Dependency Propertye Registration

        /// <summary>
        /// The DependencyProperty for <see cref="GroupingLabel"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupingLabelProperty =
            DependencyProperty.Register(
                "GroupingLabel", 
                typeof(string), 
                typeof(StackingSeriesBase3D),
                new PropertyMetadata(null, OnGroupingLabelChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the label to categorize the stacking series as a single unit.
        /// </summary>
        /// <remarks>
        /// We can group one or more series into a single group by specifying this property.
        /// The series coming under same group will stack with other series in group.
        /// </remarks>
        public string GroupingLabel
        {
            get { return (string)GetValue(GroupingLabelProperty); }
            set { this.SetValue(GroupingLabelProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets a value indicating whether the stack value is calculated.
        /// </summary>
        internal bool StackValueCalculated { get; set; }
        
        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets the starting y values collection.
        /// </summary>
        protected internal IList<double> YRangeStartValues { get; set; }

        /// <summary>
        /// Gets or sets the ending y values collection.
        /// </summary>
        protected internal IList<double> YRangeEndValues { get; set; }
        
        #endregion
        
        #endregion

        #region Methods

        #region Public Methods
        
        /// <summary>
        /// Finds the nearest point in ChartSeries relative to the mouse point/touch position.
        /// </summary>
        /// <param name="point">The co-ordinate point representing the current mouse point /touch position.</param>
        /// <param name="x">x-value of the nearest point.</param>
        /// <param name="y">y-value of the nearest point.</param>
        /// <param name="stackedYValue">The Stacked Y Value.</param>
        public override void FindNearestChartPoint(Point point, out double x, out double y, out double stackedYValue)
        {
            base.FindNearestChartPoint(point, out x, out y, out stackedYValue);
            if (double.IsNaN(x) || double.IsNaN(y))
            {
                return;
            }

            if (ActualXValues is IList<double> && !this.IsIndexed)
            {
                var xValues = ActualXValues as IList<double>;
                stackedYValue = this.GetStackedYValue(xValues.IndexOf(x));
            }
            else
            {
                stackedYValue = this.GetStackedYValue((int)x);
            }
        }

        /// <summary>
        /// Returns the stacked value of the series.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        /// <returns>StackedYValues class instance.</returns>
        public StackingValues GetCumulativeStackValues(ChartSeriesBase series)
        {
            if (Area.StackedValues == null || !Area.StackedValues.Keys.Contains(series))
            {
                this.CalculateStackingValues();
            }

            return Area.StackedValues != null && Area.StackedValues.Keys.Contains(series) ? Area.StackedValues[series] : null;
        }

        #endregion

        #region Protected Methods
        
        /// <summary>
        /// Return double value from the given index.
        /// </summary>
        /// <param name="index">The Index</param>
        /// <returns>Returns the stacked y values.</returns>
        protected double GetStackedYValue(int index)
        {
            return this.YRangeEndValues[index];
        }
        
        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned chart.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the series when grouping label changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnGroupingLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StackingSeriesBase3D series = (d as StackingSeriesBase3D);
            if (series != null && series.Area != null && series.ActualArea != null)
            {
                series.ActualArea.SBSInfoCalculated = false;
                series.Area.ScheduleUpdate();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates stacking values.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Linq Statements")]
        private void CalculateStackingValues()
        {
            Area.StackedValues = new Dictionary<object, StackingValues>();
            var stackingSeries = from series in Area.VisibleSeries
                where series is StackingSeriesBase3D
                select series;

            var enumerable = stackingSeries as ChartSeriesBase[] ?? stackingSeries.ToArray();
            var i = 0;
            foreach (var series in enumerable)
            {
                // To split the series into groups according to their labels.
                var seriesGroups =
                     from seriesGroup in series.ActualYAxis.RegisteredSeries
                     where seriesGroup is StackingSeriesBase3D && (seriesGroup as StackingSeriesBase3D).IsSeriesVisible
                     group seriesGroup by (seriesGroup as StackingSeriesBase3D).GroupingLabel into groups
                     select new { GroupingPath = groups.Key, Series = groups };
                foreach (var label in seriesGroups)
                {
                    bool reCalculation = true;
                    var lastValPosition = new List<double>();
                    var lastValNeg = new List<double>();
                    bool isFirstSeries = true;
                    foreach (ChartSeriesBase chartSeries in label.Series)
                    {
                        if (!(chartSeries is StackingSeriesBase3D))
                        {
                            continue;
                        }

                        if (!chartSeries.IsSeriesVisible)
                        {
                            continue;
                        }

                        if (((StackingSeriesBase3D)chartSeries).StackValueCalculated)
                        {
                            break;
                        }

                        IList<double> yValues;
                        var values = new StackingValues { StartValues = new List<double>(), EndValues = new List<double>() };
                        yValues = ((XyDataSeries3D)chartSeries).YValues;
                        if (chartSeries.ActualXAxis is CategoryAxis3D && !(chartSeries.ActualXAxis as CategoryAxis3D).IsIndexed)
                        {
                            if (!(chartSeries is StackingColumn100Series3D) && !(chartSeries is StackingBar100Series3D))
                            {
                                chartSeries.GroupedActualData.Clear();
                                List<double> seriesYValues = new List<double>();

                                for (int m = 0; m < chartSeries.DistinctValuesIndexes.Count; m++)
                                {
                                    var list = (from index in chartSeries.DistinctValuesIndexes[m]
                                                select new List<double> { chartSeries.GroupedSeriesYValues[0][index], index }).ToList();

                                    for (int n = 0; n < chartSeries.DistinctValuesIndexes[m].Count; n++)
                                    {
                                        var yValue = list[n][0];
                                        seriesYValues.Add(yValue);
                                        chartSeries.GroupedActualData.Add(this.ActualData[(int)list[n][1]]);
                                    }
                                }

                                yValues = seriesYValues;
                            }
                            else
                            {
                                yValues = chartSeries.GroupedSeriesYValues[0];
                                chartSeries.GroupedActualData.AddRange(chartSeries.ActualData);
                            }
                        }

                        double origin = ActualXAxis != null ? ActualXAxis.Origin : 0; // setting origin value for stacking segment

                        var j = 0;
                        if (this.ActualXAxis != null && ActualXAxis.Origin == 0 && ActualYAxis is LogarithmicAxis
                            && (ActualYAxis as LogarithmicAxis).Minimum != null)
                        {
                            origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;
                        }

                        foreach (var yValue in yValues)
                        {
                            double lastValue;
                            var currentValue = yValue;

                            if (lastValPosition.Count <= j)
                            {
                                lastValPosition.Add(0);
                            }

                            if (lastValNeg.Count <= j)
                            {
                                lastValNeg.Add(0);
                            }

                            if (values.StartValues.Count <= j)
                            {
                                values.StartValues.Add(0);
                                values.EndValues.Add(0);
                            }

                            if (currentValue >= 0)
                            {
                                lastValue = lastValPosition[j];
                                if (chartSeries.GetType().Name.Contains("100Series"))
                                {
                                    currentValue =
                                        Area.GetPercentByIndex(
                                            (label.Series as IList<ISupportAxes>).OfType<StackingSeriesBase3D>().ToList(),
                                            j,
                                            currentValue,
                                            reCalculation);
                                }

                                lastValPosition[j] += currentValue;
                            }
                            else
                            {
                                lastValue = lastValNeg[j];
                                if (chartSeries.GetType().Name.Contains("100Series"))
                                {
                                    currentValue =
                                        Area.GetPercentByIndex(
                                            (label.Series as IList<ISupportAxes>).OfType<StackingSeriesBase3D>().ToList(),
                                            j,
                                            currentValue,
                                            reCalculation);
                                }

                                lastValNeg[j] += currentValue;
                            }

                            values.StartValues[j] = isFirstSeries ? origin : lastValue;   // Applying first series starting point from the origin
                            values.EndValues[j] = currentValue + (double.IsNaN(lastValue) ? origin : lastValue); // Included condition for Empty point support
                            j++;
                        }

                        isFirstSeries = false;
                        i++;
                        Area.StackedValues.Add(chartSeries, values);
                        ((StackingSeriesBase3D)chartSeries).StackValueCalculated = true;
                        reCalculation = false;
                    }
                }
            }

            foreach (var chartSeries in enumerable.OfType<StackingSeriesBase3D>())
            {
                (chartSeries).StackValueCalculated = false;
            }
        }

        #endregion
        
        #endregion
    }
}
