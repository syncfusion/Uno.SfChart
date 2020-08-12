using System;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    internal static class LogarithmicAxisHelper
    {

        #region Methods

        #region Internal Static Methods

        /// <summary>
        /// Calculates nice interval
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="actualRange"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal static double CalculateNiceInterval(ChartAxis axis, DoubleRange actualRange, Size availableSize)
        {
            double delta = actualRange.Delta;

            double actualDesiredIntervalsCount = axis.GetActualDesiredIntervalsCount(availableSize);

            double niceInterval = delta;

            double minInterval = Math.Pow(10, Math.Floor(Math.Log10(niceInterval)));

            foreach (int mul in ChartAxis.c_intervalDivs)
            {
                double currentInterval = minInterval * mul;

                if (actualDesiredIntervalsCount < (delta / currentInterval))
                {
                    break;
                }

                niceInterval = currentInterval;
            }

            return niceInterval >= 1 ? niceInterval : 1;
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="actualInterval">The actual interval.</param>
        internal static void GenerateVisibleLabels(ChartAxisBase2D axis, object minimum, object maximum, object actualInterval, double logBase)
        {
            double interval = axis.VisibleInterval;
            var logarithmicAxis = axis as LogarithmicAxis;

            double position;
            if ((minimum != null && maximum != null && actualInterval != null)
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible && !axis.IsZoomed))
                position = axis.VisibleRange.Start;
            else
                position = axis.VisibleRange.Start - (axis.VisibleRange.Start % axis.ActualInterval);

            for (; position <= axis.VisibleRange.End; position += interval)
            {
                if (axis.VisibleRange.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, logarithmicAxis.GetActualLabelContent(Math.Pow(logBase, position)), position));
                }

                if (axis.smallTicksRequired)
                {
                    axis.AddSmallTicksPoint(position, logarithmicAxis.LogarithmicBase);
                }
            }

            if (((maximum != null && axis.VisibleRange.End.Equals(maximum))
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible && !axis.IsZoomed))
                    && !axis.VisibleRange.End.Equals(position - interval))
            {
                axis.VisibleLabels.Add(new ChartAxisLabel(axis.VisibleRange.End, logarithmicAxis.GetActualLabelContent(Math.Pow(logBase, axis.VisibleRange.End)), axis.VisibleRange.End));
            }
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis3D
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="actualInterval">The actual interval.</param>
        internal static void GenerateVisibleLabels3D(ChartAxis axis, object minimum, object maximum, object actualInterval, double logBase)
        {
        }

        /// <summary>
        /// Method implementation for Add SmallTicks for axis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="position">The position.</param>
        /// <param name="logarithmicBase">The logarithmic base.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void AddSmallTicksPoint(ChartAxis axis, double position, double logarithmicBase, double smallTicksPerInterval)
        {
            double logtickstart = Math.Pow(logarithmicBase, position - axis.VisibleInterval);
            double logtickend = Math.Pow(logarithmicBase, position);
            double logtickInterval = (logtickend - logtickstart) / (smallTicksPerInterval + 1);
            double logtickPos = logtickstart + logtickInterval;
            double logSmalltick = Math.Log(logtickPos, logarithmicBase);
            while (logtickPos < logtickend)
            {
                if (axis.VisibleRange.Inside(logSmalltick))
                {
                    axis.m_smalltickPoints.Add(logSmalltick);
                }

                logtickPos += logtickInterval;
                logSmalltick = Math.Log(logtickPos, logarithmicBase);
            }
        }

        /// <summary>
        /// Called when [minimum maximum changed].
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="logarithmicBase">The logarithmic base.</param>
        internal static void OnMinMaxChanged(ChartAxis axis, object minimum, object maximum, double logarithmicBase)
        {
            if (minimum != null || maximum != null)
            {
#if NETFX_CORE
                double minimumValue = minimum == null ? double.NegativeInfinity : Convert.ToDouble(minimum) > 0 ? Convert.ToDouble(minimum) : 1;
                double maximumValue = maximum == null ? double.PositiveInfinity : Convert.ToDouble(maximum);
                axis.ActualRange = new DoubleRange(Math.Log(minimumValue, logarithmicBase), Math.Log(maximumValue, logarithmicBase));
#else
                double minimumValue = minimum == null ? double.NegativeInfinity : ((double?)minimum > 0 ? (double?)minimum : 1).Value;
                double maximumValue = maximum == null ? double.PositiveInfinity : ((double?)maximum).Value;
                axis.ActualRange = (logarithmicBase == 10) ? new DoubleRange(Math.Log10(minimumValue), Math.Log10(maximumValue)) : new DoubleRange(Math.Log(minimumValue, logarithmicBase), Math.Log(maximumValue, logarithmicBase));
#endif
            }

            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="range">The range.</param>
        /// <param name="logarithmicBase">The logarithmic base.</param>
        /// <returns></returns>
        internal static DoubleRange CalculateActualRange(ChartAxis axis, DoubleRange range, double logarithmicBase)
        {
            if (range.IsEmpty) return range;
            double logStart = Math.Log(range.Start, logarithmicBase);
            logStart = double.IsInfinity(logStart) || double.IsNaN(logStart) ? range.Start : logStart;
            double logEnd = Math.Log(range.End, logarithmicBase);
            logEnd = double.IsInfinity(logEnd) || double.IsNaN(logEnd) ? logarithmicBase : logEnd;

            double mulS = ChartMath.Round(logStart, 1, false);
            double mulE = ChartMath.Round(logEnd, 1, true);
            range = new DoubleRange(mulS, mulE);

            return range;
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
                if (interval != null)
                {
                    axis.VisibleInterval = axis.EnableAutoIntervalOnZooming
                                            ? axis.CalculateNiceInterval(axis.VisibleRange, avalableSize)
                                            : axis.ActualInterval;
                }
                else
                {
                    axis.VisibleInterval = axis.CalculateNiceInterval(axis.VisibleRange, avalableSize);
                }
            }
        }

        #endregion

        #endregion
    }
}
