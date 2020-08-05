// <copyright file="StackingBar100Series3D.cs" company="Syncfusion. Inc">
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
    /// Class implementation for StackingBar100Series3D.
    /// </summary>
    public partial class StackingBar100Series3D : StackingColumn100Series3D
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StackingBar100Series3D"/> class.
        /// </summary>
        public StackingBar100Series3D()
        {
            this.IsActualTransposed = true;
            this.IsStacked100 = true;
        }

        #endregion

        #region  Methods

        #region Internal Methods 

        /// <summary>
        /// Updates the series when transpose changed.
        /// </summary>
        /// <param name="val">The Value</param>
        internal override void OnTransposeChanged(bool val)
        {
            this.IsActualTransposed = !val;
        }

        #endregion

        #region Protected Methods
        
        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingBar100Series3D());
        }

        #endregion

        #endregion
    }
}
