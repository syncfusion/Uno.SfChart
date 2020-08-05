// <copyright file="PolygonRecycler.cs" company="Syncfusion. Inc">
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
    using System.Linq.Expressions;
    using System.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Class implementation for PolygonRecycler.
    /// </summary>
    public partial class PolygonRecycler : DependencyObject
    {
        #region Fields

        private List<Polygon3D> polygonCache = new List<Polygon3D>();
        private int pointer = 0;
        private Polygon3D polygon;

        #endregion

        #region Polygon
        /// <summary>
        /// Method used to remove the polygon in Queue.
        /// </summary>
        /// <param name="points">The Points</param>
        /// <param name="tag">The Tag</param>
        /// <param name="index">The Index</param>
        /// <param name="stroke">The stroke</param>
        /// <param name="strokeThickness">The stroke Thickness</param>
        /// <param name="fill">The Fill</param>
        /// <returns>Returns the polygon.</returns>
        public Polygon3D DequeuePolygon(Vector3D[] points, DependencyObject tag, int index, Brush stroke, double strokeThickness, Brush fill)
        {
            if (polygonCache.Count > pointer)
            {
                polygon = polygonCache[pointer];
                polygon.Element = new Path();
                polygon.Tag = tag;
                polygon.Index = index;
                polygon.Stroke = stroke;
                polygon.CalcNormal(points[0], points[1], points[2]);
                polygon.VectorPoints = points;
                polygon.CalcNormal();

                pointer++;
            }
            else if (polygonCache.Count <= pointer)
            {
                polygon = new Polygon3D(points, tag, index, stroke, strokeThickness, fill);
                polygon.CalcNormal(points[0], points[1], points[2]);
                polygon.CalcNormal();
                polygonCache.Add(polygon);
                pointer++;
            }

            return polygon;
        }

        /// <summary>
        /// Reset the pointer.
        /// </summary>
        public void Reset()
        {
            pointer = 0;
        }
        #endregion
    }
}
