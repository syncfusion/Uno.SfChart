// <copyright file="StackingColumn100Series3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for StackingColumn100Series3D.
    /// </summary>
    public partial class StackingColumn100Series3D : StackingColumnSeries3D
    {
        #region Constructor

        /// <summary>
        /// Creates the segments of StackingColumn100Series3D.
        /// </summary>
        public override void CreateSegments()
        {
            base.CreateSegments();
            IsStacked100 = true;
        }

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingColumn100Series3D());
        }

        #endregion

        #endregion
    }
}
