// <copyright file="LineSegment3D.cs" company="Syncfusion. Inc">
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
    using Windows.ApplicationModel;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Class implementation for LineSegment3D.
    /// </summary>
    public partial class LineSegment3D : ChartSegment3D
    {
        #region Fields

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "This is a public property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a public property")]
        public Polygon3D[] topPolygonCollection, bottomPolygonCollection, frontPolygonCollection, backPolygonCollection;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "This is a public property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a public property")]
        public PolygonRecycler polygonRecycler;

        /// <summary>
        /// The DependencyProperty for <see cref="Y"/> property.
        /// </summary>
        internal static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                "Y",
                typeof(double),
                typeof(LineSegment3D),
                new PropertyMetadata(0d, OnValueChanged));

        private SfChart3D area;
        private double[] point1;
        private double[] point2;
        private Point intersectingPoint;
        private IList<double> xValues;
        private IList<double> yValues;
        private Brush color;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment3D"/> class with default settings.
        /// </summary>
        public LineSegment3D()
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a public method")]

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment3D"/> class with default settings.
        /// </summary>
        /// <param name="xValues">The X Values</param>
        /// <param name="YValues">The Y Values</param>
        /// <param name="startDepth">The Start Depth</param>
        /// <param name="endDepth">The End Depth</param>
        /// <param name="lineSeries3D">The LineSeries3D</param>
        public LineSegment3D(List<double> xValues, IList<double> YValues, double startDepth, double endDepth, LineSeries3D lineSeries3D)
        {
            this.Series = lineSeries3D;
            this.area = lineSeries3D.Area;
            this.polygonRecycler = new PolygonRecycler();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the x value of this segment (data point).
        /// </summary>
        public double XData { get; set; }

        /// <summary>
        /// Gets or sets the y data of this segment (data point).
        /// </summary>
        public double YData { get; set; }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the y value for the segment.
        /// </summary>
        internal double Y
        {
            get { return (double)GetValue(YProperty); }
            set { this.SetValue(YProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">The Size</param>
        public override void OnSizeChanged(Size size)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a public method")]
        
        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues">The X Values</param>
        /// <param name="YValues">The Y Values</param>
        /// <param name="startDepth">The Start Depth</param>
        /// <param name="endDepth">The End Depth</param>
        public override void SetData(List<double> xValues, IList<double> YValues, double startDepth, double endDepth)
        {
            this.xValues = xValues.ToList();
            this.yValues = YValues.ToList();
            this.startDepth = startDepth;
            this.endDepth = endDepth;
            this.XRange = new DoubleRange(xValues.Min(), xValues.Max());
            this.YRange = new DoubleRange(YValues.Min(), YValues.Max());
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// returns UI Element
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            return null;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            if (cartesianTransformer != null)
            {
                Polygon3D bottomPolygon, topPolygon, startPolygon, endPolygon;
                LineSeries3D lineSeries = Series as LineSeries3D;
                if (this.area == null || this.xValues.Count == 1 || lineSeries.StrokeThickness == 0)
                    return;
                var xBase = cartesianTransformer.XAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.XAxis).LogarithmicBase : 1;
                var yBase = cartesianTransformer.YAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.YAxis).LogarithmicBase : 1;
                var xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                var yIsLogarithmic = cartesianTransformer.YAxis.IsLogarithmic;
                double xStart = xIsLogarithmic ? Math.Pow(xBase, cartesianTransformer.XAxis.VisibleRange.Start) : cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = xIsLogarithmic ? Math.Pow(xBase, cartesianTransformer.XAxis.VisibleRange.End) : cartesianTransformer.XAxis.VisibleRange.End;
                var yStart = yIsLogarithmic ? Math.Pow(yBase, cartesianTransformer.YAxis.VisibleRange.Start) : cartesianTransformer.YAxis.VisibleRange.Start;
                var yEnd = yIsLogarithmic ? Math.Pow(yBase, cartesianTransformer.YAxis.VisibleRange.End) : cartesianTransformer.YAxis.VisibleRange.End;

                // Clipping the line series 3D
                if (xValues.Min() < xStart)
                {
                    while (this.xValues[0] < xStart)
                    {
                        this.xValues.RemoveAt(0);
                        this.yValues.RemoveAt(0);
                    }
                }

                if (this.xValues.Max() > xEnd)
                {
                    int index = this.xValues.IndexOf(xEnd);
                    while (this.xValues[index + 1] > xEnd)
                    {
                        this.xValues.RemoveAt(index + 1);
                        this.yValues.RemoveAt(index + 1);
                        if (index >= this.xValues.Count - 1)
                            break;
                    }
                }

                var indexes = from val in this.yValues.ToList() where (val < yStart || val > yEnd) select this.yValues.IndexOf(val);
                if (this.yValues.Count - indexes.Count() < 2)
                    return;
                if (indexes.Count() > 0)
                {
                    foreach (var index in indexes)
                    {
                        if (yValues[index] < yStart)
                            this.yValues[index] = yStart;
                        else
                            this.yValues[index] = yEnd;
                    }
                }

                //// End of clipping logic

                double x = this.xValues[0];
                double y = !lineSeries.IsAnimated && !Series.EnableAnimation ? this.yValues[0] : (this.Y < this.yValues[0] && this.Y > 0) ? this.Y : this.yValues[0];
                Point previousPoint = transformer.TransformToVisible(x, y);
                Point currentPoint;

                this.point2 = new double[10];
                this.point1 = new double[10];
                x = this.xValues[1];
                y = !lineSeries.IsAnimated && !Series.EnableAnimation ? this.yValues[1] : (this.Y < this.yValues[1] && Y > 0) ? this.Y : this.yValues[1];
                currentPoint = transformer.TransformToVisible(x, y);
                int leftThickness = lineSeries.StrokeThickness / 2;
                int rightThickness = (lineSeries.StrokeThickness % 2 == 0
                                                ? (lineSeries.StrokeThickness / 2) - 1
                                                : lineSeries.StrokeThickness / 2);
                LineSegment3D.GetLinePoints(previousPoint.X, previousPoint.Y, currentPoint.X, currentPoint.Y, leftThickness, rightThickness, this.point1);
                int j = 0;

                // To reset the polygon recycler collection index.
                this.polygonRecycler.Reset();

                for (int i = 1; i < this.yValues.Count;)
                {
                    this.point2 = new double[10];
                    previousPoint = currentPoint;
                    bool isMultiColor = (Series.SegmentColorPath != null || Series.Palette != ChartColorPalette.None);
                    this.color = isMultiColor
                                      ? (Series.GetInteriorColor(i - 1))
                                      : (this.Interior);

                    if (i == 1)
                    {
                        Vector3D[] startPolygonVects = new Vector3D[]
                        {
                            new Vector3D(this.point1[6], this.point1[7], startDepth),
                            new Vector3D(this.point1[6], this.point1[7], endDepth),
                            new Vector3D(this.point1[0], this.point1[1], endDepth),
                            new Vector3D(this.point1[0], this.point1[1], startDepth)
                        };

                        startPolygon = this.polygonRecycler.DequeuePolygon(startPolygonVects, this, Series.Segments.IndexOf(this), this.Stroke, this.StrokeThickness, this.color);

                        if (lineSeries.IsAnimated || !Series.EnableAnimation)
                            this.area.Graphics3D.AddVisual(startPolygon);
                    }

                    i++;
                    if (i < this.xValues.Count)
                    {
                        x = this.xValues[i];
                        y = !lineSeries.IsAnimated && !Series.EnableAnimation ? this.yValues[i] : (this.Y < this.yValues[i] && this.Y > 0) ? this.Y : this.yValues[i];
                        x = !(x >= xStart) ? xStart : !(x <= xEnd) ? xEnd : x;
                        y = !(y >= yStart) ? yStart : !(y <= yEnd) ? yEnd : y;
                        currentPoint = transformer.TransformToVisible(x, y);
                        this.UpdatePoints2(previousPoint.X, previousPoint.Y, currentPoint.X, currentPoint.Y, leftThickness, rightThickness);
                    }

                    Vector3D[] bottomPolyVects = new Vector3D[]
                    {
                        new Vector3D(this.point1[2], this.point1[3], startDepth),
                        new Vector3D(this.point1[0], this.point1[1], startDepth),
                        new Vector3D(this.point1[0], this.point1[1], endDepth),
                        new Vector3D(this.point1[2], this.point1[3], endDepth)
                    };

                    Vector3D[] topPolyVects = new Vector3D[]
                    {
                        new Vector3D(this.point1[6], this.point1[7], startDepth),
                        new Vector3D(this.point1[4], this.point1[5], startDepth),
                        new Vector3D(this.point1[4], this.point1[5], endDepth),
                        new Vector3D(this.point1[6], point1[7], endDepth)
                    };

                    bottomPolygon = new Polygon3D(
                        bottomPolyVects,
                        this,
                        Series.Segments.IndexOf(this),
                        Stroke,
                        StrokeThickness,
                        this.color);
                    bottomPolygon.CalcNormal(bottomPolyVects[0], bottomPolyVects[1], bottomPolyVects[2]);
                    bottomPolygon.CalcNormal();

                    topPolygon = new Polygon3D(
                        topPolyVects,
                        this,
                        Series.Segments.IndexOf(this),
                        Stroke,
                        StrokeThickness, 
                        this.color);
                    topPolygon.CalcNormal(topPolyVects[0], topPolyVects[1], topPolyVects[2]);
                    topPolygon.CalcNormal();

                    if (lineSeries.IsAnimated || !Series.EnableAnimation)
                    {
                        this.area.Graphics3D.AddVisual(bottomPolygon);
                        this.area.Graphics3D.AddVisual(topPolygon);
                        this.RenderFrontPolygon(this.point1, this.startDepth, this.endDepth, this.color);
                    }

                    if (this.point2 != null && (i < this.xValues.Count))
                        this.point1 = this.point2;
                    j++;
                }

                Vector3D[] endPolyVects = new Vector3D[]
                {
                    new Vector3D(this.point1[4], this.point1[5], startDepth),
                    new Vector3D(this.point1[4], this.point1[5], endDepth),
                    new Vector3D(this.point1[2], this.point1[3], endDepth),
                    new Vector3D(this.point1[2], this.point1[3], startDepth)
                };

                endPolygon = new Polygon3D(endPolyVects, this, 0, Stroke, 1, this.color);
                endPolygon.CalcNormal(endPolyVects[0], endPolyVects[1], endPolyVects[2]);
                endPolygon.CalcNormal();
                if (lineSeries.IsAnimated || !Series.EnableAnimation)
                    this.area.Graphics3D.AddVisual(endPolygon);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Gets the line points.
        /// </summary>
        /// <param name="x1">The X1 Value</param>
        /// <param name="y1">The Y1 Value</param>
        /// <param name="x2">The X2 Value</param>
        /// <param name="y2">The Y2 Value</param>
        /// <param name="leftThickness">The Left Thickness</param>
        /// <param name="rightThickness">The Right Thickness</param>
        /// <param name="points">The Points</param>
        private static void GetLinePoints(
            double x1,
            double y1,
            double x2,
            double y2,
            double leftThickness,
            double rightThickness,
            double[] points)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            var radian = Math.Atan2(dy, dx);
            var cos = Math.Cos(-radian);
            var sin = Math.Sin(-radian);
            var x11 = (x1 * cos) - (y1 * sin);
            var y11 = (x1 * sin) + (y1 * cos);
            var x12 = (x2 * cos) - (y2 * sin);
            var y12 = (x2 * sin) + (y2 * cos);
            cos = Math.Cos(radian);
            sin = Math.Sin(radian);
            var leftTopX = (x11 * cos) - ((y11 + leftThickness) * sin);
            var leftTopY = (x11 * sin) + ((y11 + leftThickness) * cos);
            var rightTopX = (x12 * cos) - ((y12 + leftThickness) * sin);
            var rightTopY = (x12 * sin) + ((y12 + leftThickness) * cos);
            var leftBottomX = (x11 * cos) - ((y11 - rightThickness) * sin);
            var leftBottomY = (x11 * sin) + ((y11 - rightThickness) * cos);
            var rightBottomX = (x12 * cos) - ((y12 - rightThickness) * sin);
            var rightBottomY = (x12 * sin) + ((y12 - rightThickness) * cos);
            points[0] = (int)leftTopX;
            points[1] = (int)leftTopY;
            points[2] = (int)rightTopX;
            points[3] = (int)rightTopY;
            points[4] = (int)rightBottomX;
            points[5] = (int)rightBottomY;
            points[6] = (int)leftBottomX;
            points[7] = (int)leftBottomY;
            points[8] = (int)leftTopX;
            points[9] = (int)leftTopY;
        }

        /// <summary>
        /// Updates the segment on y value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var lineSegment3D = d as LineSegment3D;
            if (lineSegment3D != null) lineSegment3D.ScheduleRender();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the segment at the given schedules.
        /// </summary>
        private void ScheduleRender()
        {
#if NETFX_CORE
            if (DesignMode.DesignModeEnabled)
                OnValueChanged();
#else
            Dispatcher.BeginInvoke(OnValueChanged);
#endif
        }

        /// <summary>
        /// Updates the segment on y value changed.
        /// </summary>
        private void OnValueChanged()
        {
            if (this.Series != null && Series.GetAnimationIsActive())
            {
                (Series as LineSeries3D).IsAnimated = true;
                var polygons = this.area.Graphics3D.GetVisual();
                var items = polygons.Where(item => item.Tag == this);
                foreach (var item in items.ToList())
                {
                    polygons.Remove(item);
                }

                this.Update(Series.CreateTransformer(new Size(), true));
                if (Series.adornmentInfo != null)
                {
                    var adornments = this.area.Graphics3D.GetVisual().OfType<UIElement3D>().ToList();
                    foreach (var item in adornments)
                    {
                        this.area.Graphics3D.Remove(item);
                        this.area.Graphics3D.AddVisual(item);
                    }
                }

                this.ScheduleView();
            }
        }

        /// <summary>
        /// Updates the segment schedule on view changed.
        /// </summary>
        private void ScheduleView()
        {
#if NETFX_CORE
            if (DesignMode.DesignModeEnabled)
                this.OnViewChanged();
#else
            Dispatcher.BeginInvoke(OnViewChanged);
#endif
        }

        /// <summary>
        /// Updates the segment on view changed.
        /// </summary>
        private void OnViewChanged()
        {
            this.area.Graphics3D.PrepareView();
            this.area.Graphics3D.View(this.area.RootPanel);
        }

        /// <summary>
        /// Renders the front polygon.
        /// </summary>
        /// <param name="point1">The Points Array</param>
        /// <param name="startDepth">The Start Depth</param>
        /// <param name="endDepth">The End Depth</param>
        /// <param name="color">The Color</param>
        private void RenderFrontPolygon(double[] point1, double startDepth, double endDepth, Brush color)
        {
            Vector3D[] frontVectors = new Vector3D[]
            {
                new Vector3D(point1[0], point1[1], startDepth),
                new Vector3D(point1[2], point1[3], startDepth),
                new Vector3D(point1[4], point1[5], startDepth),
                new Vector3D(point1[6], point1[7], startDepth),
                new Vector3D(point1[8], point1[9], startDepth)
            };

            Vector3D[] backVectors = new Vector3D[]
            {
                new Vector3D(point1[0], point1[1], endDepth),
                new Vector3D(point1[2], point1[3], endDepth),
                new Vector3D(point1[4], point1[5], endDepth),
                new Vector3D(point1[6], point1[7], endDepth),
                new Vector3D(point1[8], point1[9], endDepth)
            };

            Polygon3D frontPolygon = new Polygon3D(frontVectors, this, 0, Stroke, 1, color);
            frontPolygon.CalcNormal(frontVectors[0], frontVectors[1], frontVectors[2]);
            frontPolygon.CalcNormal();
            this.area.Graphics3D.AddVisual(frontPolygon);

            Polygon3D backPolygon = new Polygon3D(backVectors, this, 0, Stroke, 1, color);
            backPolygon.CalcNormal(backVectors[0], backVectors[1], backVectors[2]);
            backPolygon.CalcNormal();
            this.area.Graphics3D.AddVisual(backPolygon);
        }

        /// <summary>
        /// Updates the second points.
        /// </summary>
        /// <param name="xStart">The Start X Value</param>
        /// <param name="yStart">The Start Y Value</param>
        /// <param name="xEnd">The End X Value</param>
        /// <param name="yEnd">The End Y Value</param>
        /// <param name="leftThickness">The Left Thickness</param>
        /// <param name="rightThickness">The Right Thickness</param>
        private void UpdatePoints2(double xStart, double yStart, double xEnd, double yEnd, double leftThickness, double rightThickness)
        {
            LineSegment3D.GetLinePoints(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness, point2);

            bool isIntersecting = this.FindIntersectingPoint(
                this.point1[0],
                this.point1[1],
                this.point1[2],
                this.point1[3],
                this.point2[0],
                this.point2[1],
                this.point2[2],
                this.point2[3]);

            if (isIntersecting)
            {
                var diff1 = this.intersectingPoint.X - this.point1[2];
                var diff2 = this.point2[0] - this.point1[2];
                bool canSwap = false;
                if (diff1 < 0)
                    canSwap = diff1 >= -3;
                else
                    canSwap = diff2 >= diff1 || (diff1 - diff2) <= 3;
                if (canSwap)
                {
                    this.point1[2] = this.intersectingPoint.X;
                    this.point1[3] = this.intersectingPoint.Y;

                    this.point2[0] = this.intersectingPoint.X;
                    this.point2[1] = intersectingPoint.Y;

                    this.point2[8] = this.intersectingPoint.X;
                    this.point2[9] = this.intersectingPoint.Y;
                }
            }

            isIntersecting = this.FindIntersectingPoint(
                this.point1[6],
                this.point1[7],
                this.point1[4],
                this.point1[5],
                this.point2[6],
                this.point2[7],
                this.point2[4],
                this.point2[5]);

            if (isIntersecting)
            {
                var diff1 = this.intersectingPoint.X - this.point1[4];
                var diff2 = this.point2[6] - this.point1[4];
                bool canSwap;
                if (diff1 < 0)
                    canSwap = diff1 >= -3;
                else
                    canSwap = diff2 >= diff1 || (diff1 - diff2) <= 3;
                if (canSwap)
                {
                    this.point1[4] = this.intersectingPoint.X;
                    this.point1[5] = this.intersectingPoint.Y;

                    this.point2[6] = this.intersectingPoint.X;
                    this.point2[7] = this.intersectingPoint.Y;
                }
            }
        }

        /// <summary>
        /// Finds the intersecting point.
        /// </summary>
        /// <param name="x11">The X11 Point</param>
        /// <param name="y11">The Y11 Point</param>
        /// <param name="x12">The X12 Point</param>
        /// <param name="y12">The Y12 Point</param>
        /// <param name="x21">The X21 Point</param>
        /// <param name="y21">The Y21 Point</param>
        /// <param name="x22">The X22 Point</param>
        /// <param name="y22">The Y22 Point</param>
        /// <returns>Returns the intersection point.</returns>
        private bool FindIntersectingPoint(double x11, double y11, double x12, double y12, double x21, double y21, double x22, double y22)
        {
            double d = (y22 - y21) * (x12 - x11) -
                       (x22 - x21) * (y12 - y11);
            double na = (x22 - x21) * (y11 - y21) -
                        (y22 - y21) * (x11 - x21);
            //// double nb = (point12.X - point11.X)*(point11.Y - point21.Y) -
            ////             (point12.Y - point11.Y)*(point11.X - point21.X);

            if (d == 0 || d == 1 || d == -1)
                return false;

            double ua = na / d;
            //// double ub = nb/d;

            intersectingPoint = new Point(x11 + (ua * (x12 - x11)), y11 + (ua * (y12 - y11)));

            if (x11 == x12)
            {
                return !(x21 == x22 && x11 != x21);
            }

            if (x21 == x22)
            {
                return true;
            }

            // both lines are not parallel to the y-axis
            var m1 = ((double)(y11 - y12) / (double)(x11 - x12));
            var m2 = ((double)(y21 - y22) / (double)(x21 - x22));
            return m1 != m2;
        }

        #endregion

        #endregion
    }
}
