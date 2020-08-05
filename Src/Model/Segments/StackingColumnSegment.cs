using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart stacking column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="StackingColumnSeries"/>
    public partial class StackingColumnSegment : ColumnSegment
    {
        #region Constructor

        /// <summary>
        /// Called when instance created for StackingColumnSegment
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public StackingColumnSegment(double x1,double y1,double x2,double y2, StackingColumnSeries series)
            : base(x1,y1,x2,y2)
        {
            base.Series = series;
            customTemplate = series.CustomTemplate;
        }

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
            base.Update(transformer);
        }

        #endregion

        #endregion
    }
}
