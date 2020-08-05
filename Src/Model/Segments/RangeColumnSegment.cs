using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart range column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="RangeColumnSeries"/>  
    public partial class RangeColumnSegment : ColumnSegment
    {
        #region Constructor

        /// <summary>
        /// Called when instance created for RangeColumnSegment
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public RangeColumnSegment(double x1, double y1, double x2, double y2, RangeColumnSeries series, object item)
            : base(x1, y1, x2, y2)
        {
            base.Series = series;
            base.customTemplate = series.CustomTemplate;
            base.Item = item;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the high(top) value bind with this segment.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the low(bottom) value bind with this segment.
        /// </summary>
        public double Low { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            if (!this.Series.IsMultipleYPathRequired)
            {
                var axisYRange = Series.ActualYAxis.VisibleRange;
                var median = (axisYRange.End - Math.Abs(axisYRange.Start)) / 2;
                var segmentMiddle = Top / 2;

                Top = median + segmentMiddle;
                Bottom = median - segmentMiddle;

               var index =  Series.Segments.IndexOf(this);
               YData = (Series as RangeSeriesBase).High == null ? (Series as RangeSeriesBase).LowValues[index] : (Series as RangeSeriesBase).HighValues[index]; 
            }
            else
            {
                High = Top;
                Low = Bottom;
            }

            base.Update(transformer);
        }

        #endregion

        #endregion       
    }
}
