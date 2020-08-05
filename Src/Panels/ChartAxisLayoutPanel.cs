// <copyright file="ChartAxisLayoutPanel.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents <see cref="ChartAxisLayoutPanel"/> class. 
    /// </summary>
    public partial class ChartAxisLayoutPanel : Panel
    {
        #region Properties

        /// <summary>
        /// Gets or sets AxisLayout property
        /// </summary>

        public ILayoutCalculator AxisLayout
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = ChartLayoutUtils.CheckSize(availableSize);
            if (AxisLayout != null)
            {
                AxisLayout.Measure(availableSize);
            }
            return availableSize;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (AxisLayout != null)
            {
                AxisLayout.Arrange(finalSize);
            }
            return finalSize;
        }

        #endregion

        #endregion
    }
}
