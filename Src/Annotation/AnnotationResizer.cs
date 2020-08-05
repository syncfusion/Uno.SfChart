// <copyright file="AnnotationResizer.cs" company="Syncfusion. Inc">
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
    /// Represents the <see cref="AnnotationResizer"/> class.
    /// </summary>
    internal partial class AnnotationResizer : SolidShapeAnnotation
    {
        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets the rendered <see cref="Resizer"/> control.
        /// </summary>
        /// <returns>Returns the <see cref="Resizer"/> control.</returns>
        public override UIElement GetRenderedAnnotation()
        {
            return ResizerControl;
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Creates the <see cref="Resizer"/> control.
        /// </summary> 
        /// <returns>Returns the created <see cref="Resizer"/> control.</returns>
        internal override UIElement CreateAnnotation()
        {
            ResizerControl = new Resizer();
            ResizerControl.AnnotationResizer = this;
            SetBindings();
            return ResizerControl;
        }

        /// <summary>
        /// Maps the value to pixels.
        /// </summary>
        internal void MapActualValueToPixels()
        {
            ResizerControl.MapActualValueToPixels();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Binds the property between the <see cref="Resizer"/> and it's relevant annotation.
        /// </summary>
        protected override void SetBindings()
        {
        }

        #endregion

        #endregion        
    }
}
