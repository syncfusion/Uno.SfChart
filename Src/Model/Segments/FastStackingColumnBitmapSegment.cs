using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    ///  Represents a control that use a WriteableBitmap to define their appearance.
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.FastColumnBitmapSegment" />
    public partial class FastStackingColumnSegment : FastColumnBitmapSegment
    {
       #region Constructor

        /// <summary>
        /// Called when instance created for FastStackingColumnSegment with following arguments
        /// </summary>
        /// <param name="x1Values"></param>
        /// <param name="y1Values"></param>
        /// <param name="x2Values"></param>
        /// <param name="y2Values"></param>
        /// <param name="series"></param>
        public FastStackingColumnSegment(IList<double> x1Values, IList<double> y1Values, IList<double> x2Values, IList<double> y2Values, ChartSeries series)
            : base(x1Values, y1Values, x2Values, y2Values, series)
        {
            base.Series = series;
            base.Item = series.ActualData;
        }

        #endregion
    }
}

