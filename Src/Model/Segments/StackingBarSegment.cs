using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart stacking bar segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="StackingBarSeries"/>   
    public partial class StackingBarSegment:BarSegment
    {
        #region Constructor

        /// <summary>
        /// Called when instance created for StackingBarSegment
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public StackingBarSegment(double x1,double y1,double x2,double y2,StackingBarSeries series):base(x1,y1,x2,y2)
        {
            base.Series = series;
            customTemplate = series.CustomTemplate;
        }

        #endregion
    }
}
