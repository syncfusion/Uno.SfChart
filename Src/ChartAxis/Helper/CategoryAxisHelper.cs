using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    internal partial class CategoryAxisHelper
    {
        #region Methods

        #region Internal Static Methods

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <param name="labelPlacement"></param>
        /// <returns></returns>
        internal static DoubleRange ApplyRangePadding(ChartAxis axis, DoubleRange range, double interval, LabelPlacement labelPlacement)
        {
            var actualSeries =
                axis.Area.VisibleSeries
                .Where(series => series.ActualXAxis == axis)
                .Max(filteredSeries => filteredSeries.DataCount);
            if (!(actualSeries is PolarRadarSeriesBase) && labelPlacement == LabelPlacement.BetweenTicks)
            {
                return new DoubleRange(-0.5, (int)range.End + 0.5);
            }

            return range;
        }

        /// <summary>
        /// Method implementation for Get LabelContent for given position
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal static object GetLabelContent(ChartAxis axis, double position)
        {
            ChartSeriesBase actualSeries =
                axis.Area.VisibleSeries
                .Where(series => series.ActualXAxis == axis)
                .Max(filteredSeries => filteredSeries.DataCount);

            if (actualSeries != null)
            {
                if (axis.CustomLabels.Count > 0 || axis.LabelsSource != null)
                {
                    return axis.GetCustomLabelContent(position) ?? GetLabelContent(axis, (int)Math.Round(position), actualSeries) ?? string.Empty;
                }
                else
                    return GetLabelContent(axis, (int)Math.Round(position), actualSeries) ?? string.Empty;
            }

            return position;
        }

        internal static object GetLabelContent(ChartAxis axis, int pos, ChartSeriesBase actualSeries)
        {
            var isIndexed = (actualSeries is WaterfallSeries || actualSeries is HistogramSeries ||
                actualSeries is ErrorBarSeries || actualSeries is PolarRadarSeriesBase) ?
                true : (axis as CategoryAxis).IsIndexed;
            if (actualSeries != null)
            {
                IEnumerable pointValues;
                ChartValueType valueType;


                pointValues = actualSeries.ActualXValues;
                valueType = actualSeries.XAxisValueType;


                var values = pointValues as List<double>;
                if (values != null && pos < values.Count && pos >= 0)
                {
                    switch (valueType)
                    {
                        case ChartValueType.DateTime:
                            {
                                DateTime xDateTime = values[pos].FromOADate();
                                return xDateTime.ToString(axis.LabelFormat, CultureInfo.CurrentCulture);
                            }

                        case ChartValueType.TimeSpan:
                            {
                                TimeSpan xTimeSpanValue = TimeSpan.FromMilliseconds(values[pos]);
                                return xTimeSpanValue.ToString(axis.LabelFormat, CultureInfo.CurrentCulture);
                            }

                        case ChartValueType.Double:
                        case ChartValueType.Logarithmic:
                            {
                                return values[pos].ToString(axis.LabelFormat, CultureInfo.CurrentCulture);
                            }
                    }
                }
                else
                {
                    List<string> StrValues = new List<string>();
                    StrValues = !isIndexed
                        ? actualSeries.GroupedXValues
                        : pointValues as List<string>;
                    if (StrValues != null && pos < StrValues.Count && pos >= 0)
                    {
                        if (!String.IsNullOrEmpty(axis.LabelFormat))
                            return String.Format(axis.LabelFormat, StrValues[pos]);
                        return StrValues[pos];
                    }
                }
            }

            return pos;
        }

        /// <summary>
        /// Method implementation for Generate Visiblie labels for CategoryAxis
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static void GenerateVisibleLabels(ChartAxis axis, LabelPlacement labelPlacement)
        {

            {
                var actualSeries =
                    axis.Area.VisibleSeries
                        .Where(series => series.ActualXAxis == axis)
                        .Max(filteredSeries => filteredSeries.DataCount);

                actualSeries = actualSeries != null ? actualSeries : axis.Area is SfChart ? (axis.Area as SfChart).TechnicalIndicators.Where(series => series.ActualXAxis == axis)
                        .Max(filteredSeries => filteredSeries.DataCount) : null; 

                if (actualSeries == null) return;
                var visibleRange = axis.VisibleRange;
                double actualInterval = axis.ActualInterval;
                double interval = axis.VisibleInterval;
                double position = visibleRange.Start - (visibleRange.Start % actualInterval);
                int count = 0;
                var isPolarRadarSeries = actualSeries is PolarRadarSeriesBase;
                var isIndexed = (actualSeries is WaterfallSeries || actualSeries is HistogramSeries
                    || actualSeries is ErrorBarSeries || isPolarRadarSeries) ?
                    true : (axis as CategoryAxis).IsIndexed;
                count = isIndexed ? actualSeries.DataCount : actualSeries.DistinctValuesIndexes.Count;
                for (; position <= visibleRange.End; position += interval)
                {
                    int pos = ((int)Math.Round(position));
                    if (visibleRange.Inside(pos) && pos < count && pos > -1)
                    {
                        object obj = null;
                        obj = GetLabelContent(axis, pos, actualSeries);
                        axis.VisibleLabels.Add(new ChartAxisLabel(pos, obj, pos));
                    }
                }

                position = visibleRange.Start - (visibleRange.Start % actualInterval);
                if (isPolarRadarSeries) return;
                for (; position <= visibleRange.End; position += 1)
                {
                    if (labelPlacement != LabelPlacement.BetweenTicks) continue;
                    if (position == 0 && (axis.VisibleRange.Inside(-0.5)))
                    {
                        axis.m_smalltickPoints.Add(-0.5);
                    }

                    AddBetweenTicks(axis, position, 1d);
                }
            }
        }

        internal static void AddBetweenTicks(ChartAxis axis, double position, double interval)
        {
            var tickInterval = interval / 2;
            var tickpos = position + tickInterval;
            var end = axis.VisibleRange.End;
            position += 1;
            while (tickpos < position && tickpos <= end)
            {
                if (axis.VisibleRange.Inside(tickpos))
                {
                    axis.m_smalltickPoints.Add(tickpos);
                }

                tickpos += tickInterval;
            }
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        internal static double CalculateActualInterval(ChartAxis axis, DoubleRange range, Size availableSize, object interval)
        {
            if (interval == null)
                return Math.Max(1d, Math.Floor(range.Delta / axis.GetActualDesiredIntervalsCount(availableSize)));
#if NETFX_CORE
            return Convert.ToDouble(interval);
#else
            return ((double?)interval).Value;
#endif
        }

        internal static void GroupData(ChartVisibleSeriesCollection visibleSeries)
        {
            List<string> groupingValues = new List<string>();

            // Get the x values from the series.
            foreach (var series in visibleSeries)
            {
                if (series is WaterfallSeries || series is ErrorBarSeries
                    || series is HistogramSeries || series is PolarRadarSeriesBase) return;
                if (series.ActualXValues is List<string>)
                    groupingValues.AddRange(series.ActualXValues as List<string>);
                else
                    groupingValues.AddRange(from val in (series.ActualXValues as List<double>)
                                            select val.ToString());
            }

            var distinctXValues = groupingValues.Distinct().ToList();
            foreach (var series in visibleSeries)
            {
                if (series.ActualXValues is List<string>)
                    series.GroupedXValuesIndexes = (from val in (List<string>)series.ActualXValues
                                                    select (double)distinctXValues.IndexOf(val)).ToList();
                else
                    series.GroupedXValuesIndexes = (from val in series.ActualXValues as List<double>
                                                    select (double)distinctXValues.IndexOf(val.ToString())).ToList();
                series.GroupedXValues = distinctXValues;
            }

            foreach (var series in visibleSeries)
            {
                series.DistinctValuesIndexes.Clear();
                var aggregateValues = new Dictionary<int, List<double>>[series.ActualSeriesYValues.Length];
                var yValues = new List<double>[series.ActualSeriesYValues.Length];
                series.GroupedSeriesYValues = new IList<double>[series.ActualSeriesYValues.Length];
                bool isRangeColumnSingleValue = series is RangeColumnSeries && !series.IsMultipleYPathRequired;

                for (int i = 0; i < series.ActualSeriesYValues.Length; i++)
                {
                    series.GroupedSeriesYValues[i] = new List<double>();
                    aggregateValues[i] = new Dictionary<int, List<double>>();
                    ((List<double>)series.GroupedSeriesYValues[i]).AddRange(series.ActualSeriesYValues[i]);
                    
                    if (isRangeColumnSingleValue)
                    {
                        break;
                    }
                }

                var actualXValues = series.ActualXValues is List<string>
                    ? series.ActualXValues as List<string>
                    : (from val in (series.ActualXValues as List<double>) select val.ToString()).ToList();

                for (int i = 0; i < distinctXValues.Count; i++)
                {
                    int count = 0;
                    var indexes = new List<int>();
                    for (int j = 0; j < series.ActualSeriesYValues.Length; j++)
                    {
                        yValues[j] = new List<double>();
                        
                        if (isRangeColumnSingleValue)
                        {
                            break;
                        }
                    }

                    (from xValues in actualXValues
                     let index = count++
                     where distinctXValues[i] == xValues
                     select index).Select(t1 =>
                     {
                         for (int j = 0; j < series.ActualSeriesYValues.Length; j++)
                         {
                            yValues[j].Add(series.ActualSeriesYValues[j][count - 1]);
                            if (j == 0)
                                indexes.Add(count - 1);
                                
                             if (isRangeColumnSingleValue)
                             {
                                 break;
                             }
                         }

                         return 0;
                     }).ToList();

                    for (int j = 0; j < series.ActualSeriesYValues.Length; j++)
                    {
                        aggregateValues[j].Add(i, yValues[j]);
                        if (isRangeColumnSingleValue)
                        {
                            break;
                        }
                    }

                    series.DistinctValuesIndexes.Add(i, indexes);
                }

                var aggregateValue = series.ActualXAxis is CategoryAxis
                    ? (series.ActualXAxis as CategoryAxis).AggregateFunctions
                    :  AggregateFunctions.None;

                if (aggregateValue != AggregateFunctions.None)
                {
                    series.DistinctValuesIndexes.Clear();
                    series.GroupedXValuesIndexes.Clear();
                    for (int i = 0; i < series.ActualSeriesYValues.Length; i++)
                    {
                        series.GroupedSeriesYValues[i].Clear();
                        
                        if (isRangeColumnSingleValue)
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < distinctXValues.Count; i++)
                    {
                        series.GroupedXValuesIndexes.Add(i);
                        for (int j = 0; j < series.ActualSeriesYValues.Length; j++)
                        {
                            if (aggregateValues[j][i].Count > 0)
                                    switch (aggregateValue)
                                    {
                                        case AggregateFunctions.Average:
                                            series.GroupedSeriesYValues[j].Add(aggregateValues[j][i].Average());
                                            break;
                                        case AggregateFunctions.Count:
                                            series.GroupedSeriesYValues[j].Add(aggregateValues[j][i].Count());
                                            break;
                                        case AggregateFunctions.Max:
                                            series.GroupedSeriesYValues[j].Add(aggregateValues[j][i].Max());
                                            break;
                                        case AggregateFunctions.Min:
                                            series.GroupedSeriesYValues[j].Add(aggregateValues[j][i].Min());
                                            break;
                                        case AggregateFunctions.Sum:
                                            series.GroupedSeriesYValues[j].Add(aggregateValues[j][i].Sum());
                                            break;
                                    }
                                    
                            if (isRangeColumnSingleValue)
                            {
                                break;
                            }
                        }

                        var trackballIndex = new List<int> { i };
                        series.DistinctValuesIndexes.Add(i, trackballIndex);
                    }
                }
            }
        }
        
        #endregion

        #endregion
    }
}
