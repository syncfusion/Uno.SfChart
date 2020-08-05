using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    internal static class TimeSpanAxisHelper
    {
        #region Methods

        #region Static Methods

        /// <summary>
        /// Generates the visible labels.
        /// </summary>
        /// <param name="axis">The axis.</param>
        internal static void GenerateVisibleLabels(ChartAxisBase2D axis, object minimum, object maximum, object actualInterval)
        {
            double interval = axis.VisibleInterval;
            var range = axis.VisibleRange;
            double position;
            var timeSpanAxis = axis as TimeSpanAxis;

            if ((minimum != null && maximum != null && actualInterval != null)
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible && !axis.IsZoomed))
                position = range.Start;
            else
                position = range.Start - (range.Start % interval);

            double previousPosition = double.NaN;

            for (; position <= axis.VisibleRange.End; position += interval)
            {
                if (position == previousPosition)
                    break;
                if (axis.VisibleRange.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, timeSpanAxis.GetActualLabelContent(position), position));
                }

                if (axis.smallTicksRequired)
                {
                    axis.AddSmallTicksPoint(position);
                }

                previousPosition = position;
            }
#if NETFX_CORE

            if (((maximum != null && range.End.Equals((TimeSpan.Parse(maximum.ToString())).TotalMilliseconds))
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible && !axis.IsZoomed))
                && !range.End.Equals(position - interval))
#else
            if (((maximum != null && range.End.Equals(((TimeSpan)maximum).TotalMilliseconds))
               || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
               || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible && !axis.IsZoomed))
               && !range.End.Equals(position - interval))
#endif
            {
                axis.VisibleLabels.Add(new ChartAxisLabel(range.End, timeSpanAxis.GetActualLabelContent(range.End), range.End));
            }
        }

        /// <summary>
        /// Generates the visible labels.
        /// </summary>
        /// <param name="axis">The axis.</param>
        internal static void GenerateVisibleLabels3D(ChartAxis axis, object minimum, object maximum, object actualInterval)
        {
            double interval = axis.VisibleInterval;
            var range = axis.VisibleRange;
            double position;
            if ((minimum != null && maximum != null && actualInterval != null)
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible))
                position = range.Start;
            else
                position = range.Start - (range.Start % interval);

            for (; position <= axis.VisibleRange.End; position += interval)
            {
                if (axis.VisibleRange.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, axis.GetLabelContent(position), position));
                }

                if (axis.smallTicksRequired)
                {
                    axis.AddSmallTicksPoint(position);
                }
            }
#if NETFX_CORE 

            if (((maximum != null && range.End.Equals((TimeSpan.Parse(maximum.ToString())).TotalMilliseconds))
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible))
                && !range.End.Equals(position - interval))
#else
            if (((maximum != null && range.End.Equals(((TimeSpan)maximum).TotalMilliseconds))
                || axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                || (axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible))
                && !range.End.Equals(position - interval))
#endif
            {
                axis.VisibleLabels.Add(new ChartAxisLabel(range.End, axis.GetLabelContent(range.End), range.End));
            }
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="avalableSize">Size of the available.</param>
        internal static void CalculateVisibleRange(ChartAxisBase2D axis, object interval, Size avalableSize)
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
