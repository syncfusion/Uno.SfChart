using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// StackingColumn100Series resembles <see cref="StackingColumnSeries"/>, but the cumulative portion of each stacked element always totals to 100%.
    /// </summary>
    public partial class StackingColumn100Series : StackingColumnSeries
    {
        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of StackingColumn100Series
        /// </summary>
        public override void CreateSegments()
        {
            base.CreateSegments();
            IsStacked100 = true;
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingColumn100Series());
        }

        #endregion
        
        #endregion
    }
}
