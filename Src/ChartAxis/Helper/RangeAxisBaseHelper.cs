using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Charts
{
    internal static class RangeAxisBaseHelper
    {
        #region Methods

        #region Internal Static Methods

        /// <summary>
        /// Method implementation for Add smallTicks to axis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="position">The position.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void AddSmallTicksPoint(ChartAxis axis, double position, double interval, double smallTicksPerInterval)
        {
            var tickInterval = interval / (smallTicksPerInterval + 1);
            var tickpos = position + tickInterval;
            var end = axis.VisibleRange.End;
            var visibleRange = axis.VisibleRange;
            var linearAxis = axis as NumericalAxis;

            if (linearAxis != null && linearAxis.IsSecondaryAxis && linearAxis.BreakRangesInfo.Count > 0)
            {
                var ranges = linearAxis.AxisRanges;
                for (int i = 0; i < ranges.Count; i++)
                {
                    if (!ranges[i].Inside(position)) continue;
                    end = ranges[i].End;
                    visibleRange = ranges[i];
                    break;
                }
            }

            position += interval;

            // Precision value checked for 7 decimal points
            while (tickpos < position && (Math.Abs(position - tickpos) > 0.0000001) && tickpos <= end)
            {
                if (visibleRange.Inside(tickpos))
                {
                    axis.m_smalltickPoints.Add(tickpos);
                }

                tickpos += tickInterval;
            };
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="smallTicksPerInterval">The small ticks per interval.</param>
        internal static void GenerateVisibleLabels(ChartAxis axis, double smallTicksPerInterval)
        {
            double interval = axis.VisibleInterval;
            double position = axis.VisibleRange.Start - (axis.VisibleRange.Start % interval);

            for (; position <= axis.VisibleRange.End; position += interval)
            {
                if (axis.VisibleRange.Inside(position))
                {
                    axis.VisibleLabels.Add(new ChartAxisLabel(position, axis.GetActualLabelContent(position), position));
                }

                if (axis.smallTicksRequired)
                {
                    axis.AddSmallTicksPoint(position);
                }
            }
        }

        #endregion

        #endregion

    }
}
