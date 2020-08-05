// <copyright file="StackingColumnSegment3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{

    /// <summary>
    /// Represents chart stacking column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="StackingColumnSeries3D"/>
    public partial class StackingColumnSegment3D : ColumnSegment3D
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StackingColumnSegment3D"/> class.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="startDepth">The start depth.</param>
        /// <param name="endDepth">The end depth.</param>
        /// <param name="series">The series.</param>
        public StackingColumnSegment3D(double x1, double y1, double x2, double y2, double startDepth, double endDepth, ChartSeriesBase series)
               : base(x1, y1, x2, y2, startDepth, endDepth)
        {
            Series = series;
        }
        
        #endregion

        #region Methods

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
    }
}
