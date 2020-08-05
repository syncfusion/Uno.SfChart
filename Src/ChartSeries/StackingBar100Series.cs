using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// StackingBar100Series resembles <see cref="StackingBarSeries"/>, but the cumulative portion of each stacked element always totals to 100%.
    /// </summary>
    public partial class StackingBar100Series : StackingBarSeries
    {
        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of StackingBar100Series.
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
            return base.CloneSeries(new StackingBar100Series());
        }

        #endregion

        #endregion
    }
}
