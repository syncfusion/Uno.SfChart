using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    internal static class NumericalAxisHelper
    {
        #region Methods

        #region Internal Static Methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void GenerateVisibleLabels(ChartAxisBase2D axis, object minimum, object maximum, object actualInterval, double smallTicksPerInterval)
        {
            DoubleRange range = axis.VisibleRange;
            double interval = axis.VisibleInterval;
            double position;

            if ((minimum != null && maximum != null && actualInterval != null) || axis.DesiredIntervalsCount != null
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible && !axis.IsZoomed))
                position = range.Start;
            else
                position = range.Start - (range.Start % interval);

            double previousPosition = double.NaN;

            for (; position <= range.End; position += interval)
            {
                if (position == previousPosition)
                    break;

                if (range.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, (axis as ChartAxis).GetActualLabelContent(position), position));
                }

                if (axis.smallTicksRequired)
                {
                    axis.AddSmallTicksPoint(position);
                }

                previousPosition = position;
            }

            if (((maximum != null && range.End.Equals(maximum))
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible && !axis.IsZoomed))
                && !range.End.Equals(position - interval))
            {
                axis.VisibleLabels.Add(new ChartAxisLabel(range.End, (axis as ChartAxis).GetActualLabelContent(range.End), range.End));
            }
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis3D
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void GenerateVisibleLabels3D(ChartAxis axis, object minimum, object maximum, object actualInterval, double smallTicksPerInterval)
        {
            DoubleRange range = axis.VisibleRange;
            double interval = axis.VisibleInterval;
            double position;

            if ((minimum != null && maximum != null && actualInterval != null) || axis.DesiredIntervalsCount != null
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible))
                position = range.Start;
            else
                position = range.Start - (range.Start % interval);

            for (; position <= range.End; position += interval)
            {
                if (range.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, axis.GetActualLabelContent(position), position));
                }

                if (axis.smallTicksRequired)
                {
                    axis.AddSmallTicksPoint(position);
                }
            }

            if (((maximum != null && range.End.Equals(maximum))
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible))
                && !range.End.Equals(position - interval))
            {
                axis.VisibleLabels.Add(new ChartAxisLabel(range.End, axis.GetActualLabelContent(range.End), range.End));
            }
        }

        /// <summary>
        /// Called when [minimum maximum changed].
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="minimum">The minimum.</param>
        internal static void OnMinMaxChanged(ChartAxis axis, object maximum, object minimum)
        {
            if (minimum != null || maximum != null)
            {
#if NETFX_CORE
                double minimumValue = minimum == null ? double.NegativeInfinity : Convert.ToDouble(minimum);
                double maximumValue = maximum == null ? double.PositiveInfinity : Convert.ToDouble(maximum);
                axis.ActualRange = new DoubleRange(minimumValue, maximumValue);
#else
                double minimumValue = minimum == null ? double.NegativeInfinity : ((double?)minimum).Value;
                double maximumValue = maximum == null ? double.PositiveInfinity : ((double?)maximum).Value;
                axis.ActualRange = new DoubleRange(minimumValue, maximumValue);
#endif
            }
            else
            {
                axis.ActualRange = DoubleRange.Empty;
            }

            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="range">The range.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="rangePadding">The range padding.</param>
        /// <returns></returns>
        internal static DoubleRange ApplyRangePadding(ChartAxis axis, DoubleRange range, double interval, NumericalPadding rangePadding)
        {
            if (rangePadding == NumericalPadding.Normal)
            {
                double minimum = 0,
                       remaining,
                       start = range.Start;

                if (range.Start < 0)
                {
                    start = 0;
                    minimum = range.Start + (range.Start / 20);

                    remaining = interval + (minimum % interval);

                    if ((0.365 * interval) >= remaining)
                    {
                        minimum -= interval;
                    }

                    if (minimum % interval < 0)
                    {
                        minimum = (minimum - interval) - (minimum % interval);
                    }
                }
                else
                {
                    minimum = range.Start < ((5.0 / 6.0) * range.End)
                                         ? 0
                                         : (range.Start - (range.End - range.Start) / 2);
                    if (minimum % interval > 0)
                    {
                        minimum -= (minimum % interval);
                    }
                }

                double maximum = (range.End + (range.End - start) / 20);

                remaining = interval - (maximum % interval);

                if ((0.365 * interval) >= remaining)
                {
                    maximum += interval;
                }

                if (maximum % interval > 0)
                {
                    maximum = (maximum + interval) - (maximum % interval);
                }

                range = new DoubleRange(minimum, maximum);

                if (minimum == 0d)
                {
                    axis.ActualInterval = axis.CalculateActualInterval(range, axis.AvailableSize);
                    return new DoubleRange(0, Math.Ceiling(maximum / axis.ActualInterval) * axis.ActualInterval);
                }
            }
            else if (rangePadding == NumericalPadding.Round
                || rangePadding == NumericalPadding.Additional)
            {
                double minimum = Math.Floor(range.Start / interval) * interval;
                double maximum = Math.Ceiling(range.End / interval) * interval;

                if (rangePadding == NumericalPadding.Additional)
                {
                    minimum -= interval;
                    maximum += interval;
                }

                return new DoubleRange(minimum, maximum);
            }

            return range;
        }

        internal static void GenerateScaleBreakVisibleLabels(NumericalAxis axis, object actualInterval, double smallTicksPerInterval)
        {
            var ranges = axis != null ? axis.AxisRanges : null;
            if (ranges == null || ranges.Count == 0)
            {
                GenerateVisibleLabels(axis, axis.Minimum, axis.Maximum, axis.Interval, axis.SmallTicksPerInterval);
            }
            else
            {
                for (int i = 0; i < ranges.Count; i++)
                {
                    var position = ranges[i].Start;
                    var interval = axis.CalculateActualInterval(
                        ranges[i], 
                        axis.Orientation == Orientation.Vertical 
                        ? new Size(axis.AvailableSize.Width, axis.AxisLength[i])
                            : new Size(axis.AxisLength[i], axis.AvailableSize.Height));

                    if (!ranges[i].Inside(position)) continue;

                    for (; position <= ranges[i].End; position += interval)
                    {
                        axis.VisibleLabels.Add(new ChartAxisLabel(position, axis.GetActualLabelContent(position), position));
                        if (ranges[i].Delta == 0) break;
                        if (axis.smallTicksRequired)
                            axis.AddSmallTicksPoint(position, interval);
                    }

                    if (ranges[i].End.Equals(position - interval)) continue;
                    var count = axis.VisibleLabels.Count;
                    var lastLabel = new ChartAxisLabel();
                    lastLabel.Position = ranges[i].End;
                    lastLabel.LabelContent = axis.GetActualLabelContent(ranges[i].End);
                    if (Convert.ToDouble(axis.VisibleLabels[count - 1].LabelContent) != ranges[i].End)
                        axis.VisibleLabels.Add(lastLabel);
                }
            }
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="avalableSize">Size of the available.</param>
        /// <param name="interval">The interval.</param>
        internal static void CalculateVisibleRange(ChartAxisBase2D axis, Size avalableSize, object interval)
        {
            if (axis.ZoomFactor < 1 || axis.ZoomPosition > 0)
            {
#if NETFX_CORE
                if (interval == null || Convert.ToDouble(interval) == 0)
#else
                if (interval == null || ((double?)interval).Value == 0)
#endif
                {
                    axis.VisibleInterval = axis.CalculateNiceInterval(axis.VisibleRange, avalableSize);
                }
                else if (interval != null)
                {
#if NETFX_CORE 
                    double actualInterval = Convert.ToDouble(interval);
#else
                    double actualInterval = ((double?)interval).Value;
#endif
                    axis.VisibleInterval = axis.EnableAutoIntervalOnZooming
                                          ? axis.CalculateNiceInterval(axis.VisibleRange, avalableSize)
                                          : actualInterval;
                }
            }
        }

        #endregion

        #endregion

    }
}
