// <copyright file="ArrowLine.cs" company="Syncfusion. Inc">
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
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Provides arrow line rendering support, which includes a positioning attributes. 
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    public partial class ArrowLine : DependencyObject
    {
        #region Dependency Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="X1"/> property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(
                "X1",
                typeof(double),
                typeof(ArrowLine),
                new PropertyMetadata(0.0));

        /// <summary>
        ///    The DependencyProperty for <see cref="Y1"/> property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(
                "Y1",
                typeof(double),
                typeof(ArrowLine),
                new PropertyMetadata(0.0));

        /// <summary>
        ///    The DependencyProperty for <see cref="X2"/> property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(
                "X2",
                typeof(double),
                typeof(ArrowLine),
                new PropertyMetadata(0.0));

        /// <summary>
        ///  The DependencyProperty for <see cref="Y2"/> property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(
                "Y2",
                typeof(double),
                typeof(ArrowLine),
                new PropertyMetadata(0.0));

        #endregion

        #region Fields

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Protected Property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Protected Property")]
        protected PathGeometry pathGeometry;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Protected Property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Protected Property")]
        protected PathFigure pathFigureLine;

        private Windows.UI.Xaml.Media.LineSegment segmentLine;
        private PathFigure pathFigureHead;
        private PolyLineSegment polySegmentHead;
        private double arrowAngle = 45, arrowLength = 12.0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowLine"/> class.
        /// </summary>
        public ArrowLine()
        {
            pathGeometry = new PathGeometry();

            pathFigureLine = new PathFigure();
            segmentLine = new Windows.UI.Xaml.Media.LineSegment();
            pathFigureLine.Segments.Add(segmentLine);

            pathFigureHead = new PathFigure();
            polySegmentHead = new PolyLineSegment();
            pathFigureHead.Segments.Add(polySegmentHead);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the x-coordinate of the ArrowLine start point.
        /// </summary>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the ArrowLine start point.
        /// </summary>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the ArrowLine end point.
        /// </summary>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the ArrowLine end point.
        /// </summary>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Draw a arrow line.
        /// </summary>
        /// <returns>Return the <see cref="pathGeometry"/> of arrow line.</returns>
        public Geometry GetGeometry()
        {
            Point point1 = new Point(X1, Y1);
            Point point2 = new Point(X2, Y2);
            pathGeometry.Figures.Clear();
            pathFigureLine.StartPoint = point1;
            segmentLine.Point = point2;
            pathGeometry.Figures.Add(pathFigureLine);
            pathGeometry.Figures.Add(CalculateArrow(pathFigureHead, point1, point2));
            return pathGeometry;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Vector and matrix multiplication
        /// </summary>
        /// <param name="point">The Point</param>
        /// <param name="mat">The Matrix</param>
        /// <returns>Returns the multiplied result.</returns>       
        private static Point MultiplyMatrixVector(Point point, Matrix mat)
        {
            double x = mat.M11 * point.X + mat.M12 * point.Y;
            double y = mat.M21 * point.X + mat.M22 * point.Y;
            return new Point(x, y);
        }

        /// <summary>
        /// Matrixes multiplication
        /// </summary>
        /// <param name="mat1">The First Matrix</param>
        /// <param name="mat2">The Second Matrix</param>
        /// <returns>Returns the multiplied result matrix.</returns>
        private static Matrix MultiplyMatrixes(Matrix mat1, Matrix mat2)
        {
            double m11 = mat1.M11 * mat2.M11 + mat1.M12 * mat2.M21;
            double m12 = mat1.M11 * mat2.M12 + mat1.M12 * mat2.M22;
            double m21 = mat1.M21 * mat2.M11 + mat1.M22 * mat2.M21;
            double m22 = mat1.M21 * mat2.M12 + mat1.M22 * mat2.M22;
            return new Matrix(m11, m12, m21, m22, 0, 0);
        }

        /// <summary>
        /// Vector Normalization.
        /// </summary>
        /// <param name="vectPoint">The vector Point</param>
        /// <param name="len">The Length</param>
        /// <returns>Returns the normalized point.</returns>
        private static Point FindNormalization(Point vectPoint, double len)
        {
            double length = Math.Sqrt((vectPoint.X * vectPoint.X) + (vectPoint.Y * vectPoint.Y));
            return new Point((vectPoint.X / length) * len, (vectPoint.Y / length) * len);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the arrow points.
        /// </summary>
        /// <param name="pathfigure">The Path Figure</param>
        /// <param name="point1">The First Point</param>
        /// <param name="point2">The Second Point</param>
        /// <returns>Returns the <see cref="PathFigure"/> for the arrow.</returns>
        private PathFigure CalculateArrow(PathFigure pathfigure, Point point1, Point point2)
        {
            Matrix matx = new Matrix();

            // To made the matx as identity matrix
            matx.M11 = 1;
            matx.M22 = 1;

            // Find the width and height 
            Point vectPoint = new Point(point1.X - point2.X, point1.Y - point2.Y);

            // To find the vector normalization for the width and height , arrow length
            vectPoint = ArrowLine.FindNormalization(vectPoint, arrowLength);

            PolyLineSegment polyseg = pathfigure.Segments[0] as PolyLineSegment;
            polyseg.Points.Clear();

            // Rotation matrix calculation and start point calculation of the arrow
            var angleRadians = (2 * Math.PI * (arrowAngle / 2)) / 360;
            var sine = Math.Sin(angleRadians);
            var cosine = Math.Cos(angleRadians);
            var matrix = new Matrix(cosine, sine, -sine, cosine, 0, 0);
            matx = ArrowLine.MultiplyMatrixes(matx, matrix);
            Point tempPoint = ArrowLine.MultiplyMatrixVector(vectPoint, matx);
            pathfigure.StartPoint = new Point(point2.X + tempPoint.X, point2.Y + tempPoint.Y);

            polyseg.Points.Add(point2);

            // Rotation matrix calculation and end point calculation of the arrow
            angleRadians = (2 * Math.PI * -arrowAngle) / 360;
            sine = Math.Sin(angleRadians);
            cosine = Math.Cos(angleRadians);
            matrix = new Matrix(cosine, sine, -sine, cosine, 0, 0);
            matx = ArrowLine.MultiplyMatrixes(matx, matrix);
            tempPoint = ArrowLine.MultiplyMatrixVector(vectPoint, matx);
            polyseg.Points.Add(new Point(point2.X + tempPoint.X, point2.Y + tempPoint.Y));
            pathfigure.IsClosed = true;
            return pathfigure;
        }

        #endregion

        #endregion
    }
}
