// <copyright file="BarSeries3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
#if WINDOWS_UAP
    using System.Collections.Generic;
#endif
    using System.ComponentModel;
    using System.Linq;

    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// <see cref="BarSeries3D"/> displays its data points using a set of horizontal bars.
    /// </summary>
    public partial class BarSeries3D : ColumnSeries3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarSeries3D"/> class.
        /// </summary>
        public BarSeries3D()
        {
            this.IsActualTransposed = true;
        }

        /// <summary>
        /// Updates the series when transpose changed.
        /// </summary>
        /// <param name="val">The Value</param>
        internal override void OnTransposeChanged(bool val)
        {
            this.IsActualTransposed = !val;
        }

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new BarSeries3D());
        }
    }
}
