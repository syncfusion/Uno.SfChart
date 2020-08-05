using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class StackingArea100Series : StackingAreaSeries
    {
        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of StackingArea100Series
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
            return base.CloneSeries(new StackingArea100Series() { IsClosed = this.IsClosed });
        }

        #endregion

        #endregion
    }
}
