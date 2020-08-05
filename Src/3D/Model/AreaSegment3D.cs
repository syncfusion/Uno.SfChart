// <copyright file="AreaSegment3D.cs" company="Syncfusion. Inc">
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
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Represents a polygon, which is a connected series of lines that form a closed shape.
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ChartSegment3D" />
    public partial class AreaSegment3D : ChartSegment3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Y"/> property.
        /// </summary>
        internal static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                "Y", 
                typeof(double), 
                typeof(AreaSegment3D),
                new PropertyMetadata(0d, OnValueChanged));
        
        #endregion

        #region Fields

        private Polygon3D frontPolygon, bottomPolygon, rightPolygon, leftPolygon, backPolygon;
        private Polygon3D[] topPolygonCollection;
        private SfChart3D area;
        private IList<double> xValues;
        private IList<double> yValues;

        #endregion
        
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSegment3D"/> class with default settings.
        /// </summary>
        public AreaSegment3D()
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a public method")]
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSegment3D"/> class.
        /// </summary>
        /// <param name="xValues">The X Values</param>
        /// <param name="YValues">The Y Values</param>
        /// <param name="startDepth">The Start Depth</param>
        /// <param name="endDepth">The End Depth</param>
        /// <param name="areaSeries3D">The AreaSeries 3D</param>
        public AreaSegment3D(List<double> xValues, IList<double> YValues, double startDepth, double endDepth, AreaSeries3D areaSeries3D)
        {
            this.Series = areaSeries3D;
            this.area = areaSeries3D.Area;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the x value of this segment (data point).
        /// </summary>
        public double XData { get; set; }

        /// <summary>
        /// Gets or sets the y value of this segment (data point).
        /// </summary>
        public double YData { get; set; }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        internal double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

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
        /// Used for creating UIElement for rendering this segment. 
        /// This method is not intended to be called explicitly outside the Chart  
        /// but it can be overridden by any derived class.
        /// </summary>
        /// <param name="size">The Size</param>
        /// <returns>Returns the created visual.</returns>
        public override UIElement CreateVisual(Size size)
        {
            return null;
        }
        
        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>Returns the rendered visual</returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }
        
        /// <summary>
        /// Updates the segments based on its data point value. 
        /// This method is not intended to be called explicitly outside the Chart 
        /// but it can be overridden by any derived class.
        /// </summary>
        /// <param name="transformer">The Chart Points Positioning Transformer</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Needed For Point Generation Logic")]       
        public override void Update(IChartTransformer transformer)
        {
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;

            if (cartesianTransformer != null)
            {
                var xBase = cartesianTransformer.XAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.XAxis).LogarithmicBase : 1;
                var yBase = cartesianTransformer.YAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.YAxis).LogarithmicBase : 1;
                var xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                var yIsLogarithmic = cartesianTransformer.YAxis.IsLogarithmic;
                double xStart = xIsLogarithmic ? Math.Pow(xBase, cartesianTransformer.XAxis.VisibleRange.Start) : cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = xIsLogarithmic ? Math.Pow(xBase, cartesianTransformer.XAxis.VisibleRange.End) : cartesianTransformer.XAxis.VisibleRange.End;
                var yStart = yIsLogarithmic ? Math.Pow(yBase, cartesianTransformer.YAxis.VisibleRange.Start) :  cartesianTransformer.YAxis.VisibleRange.Start;
                var yEnd = yIsLogarithmic? Math.Pow(yBase, cartesianTransformer.YAxis.VisibleRange.End) : cartesianTransformer.YAxis.VisibleRange.End;

                // Clippiing the area series 3D.
                if (this.xValues.Min() < xStart)
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

                if (this.yValues.Min() < yStart)
                {
                    List<double> tempYVal = this.yValues.ToList();
                    foreach (var yVal in tempYVal)
                    {
                        if (yVal < yStart)
                        {
                            this.yValues[this.yValues.IndexOf(yVal)] = yStart;
                        }
                    }
                }

                if (this.yValues.Max() > yEnd)
                {
                    List<double> tempYVal = this.yValues.ToList();
                    foreach (var yVal in tempYVal)
                    {
                        if (yVal > yEnd)
                            this.yValues[this.yValues.IndexOf(yVal)] = yEnd;
                    }
                }

                // End of clipping logic
                double x = this.xValues[0];
                double y = !(Series as AreaSeries3D).IsAnimated && !Series.EnableAnimation ? this.yValues[0] : (this.Y < this.yValues[0] && this.Y > 0) ? this.Y : this.yValues[0];
                Point previousPoint = transformer.TransformToVisible(x, y);
                Point currentPoint;
                List<Point> areaPoint = new List<Point>();
                areaPoint.Add(previousPoint);
                this.topPolygonCollection = new Polygon3D[this.yValues.Count];
                Vector3D vector1, vector2, vector3, vector4;
                for (int i = 1; i < this.xValues.Count; i++)
                {
                    x = this.xValues[i];
                    y = !(Series as AreaSeries3D).IsAnimated && !Series.EnableAnimation ? this.yValues[i] : (this.Y < this.yValues[i] && this.Y > 0) ? this.Y : this.yValues[i];
                    x = !(x >= xStart) ? xStart: !(x <= xEnd) ? xEnd : x;
                    y = !(y >= yStart) ? yStart : !(y <= yEnd) ? yEnd : y;

                    currentPoint = transformer.TransformToVisible(x, y);
                    vector1 = new Vector3D(previousPoint.X, previousPoint.Y, startDepth);
                    vector2 = new Vector3D(currentPoint.X, currentPoint.Y, startDepth);
                    vector3 = new Vector3D(currentPoint.X, currentPoint.Y, endDepth);
                    vector4 = new Vector3D(previousPoint.X, previousPoint.Y, endDepth);
                    var points = new Vector3D[] { vector1, vector2, vector3, vector4 };
                    this.topPolygonCollection[i] = new Polygon3D(points, this, Series.Segments.IndexOf(this), Stroke, StrokeThickness, Interior);
                    this.topPolygonCollection[i].CalcNormal(points[0], points[1], points[2]);
                    this.topPolygonCollection[i].CalcNormal();
                    previousPoint = currentPoint;
                    areaPoint.Add(currentPoint);
                }

                x = this.xValues[0];
                y = !(Series as AreaSeries3D).IsAnimated && !Series.EnableAnimation ? this.yValues[0] : (this.Y < this.yValues[0] && this.Y > 0) ? this.Y : this.yValues[0];
                Point topLeft = transformer.TransformToVisible(x, y);
                x = this.xValues[this.xValues.Count - 1];
                y = !(Series as AreaSeries3D).IsAnimated && !Series.EnableAnimation ? this.yValues[this.xValues.Count - 1] : (this.Y < this.yValues[this.yValues.Count - 1]) ? this.Y : this.yValues[this.yValues.Count - 1];
                Point topRight = transformer.TransformToVisible(x, y);
                double origin = Series.ActualYAxis.Origin < yStart ? yStart : Series.ActualYAxis.Origin;
                x = this.xValues[0];
                Point bottomLeft = transformer.TransformToVisible(x, origin);
                x = this.xValues[this.xValues.Count - 1];
                Point bottomRight = transformer.TransformToVisible(x, origin);
                areaPoint.Add(bottomRight);
                areaPoint.Add(bottomLeft);

                Vector3D tlfVector = new Vector3D(topLeft.X, topLeft.Y, startDepth);
                Vector3D tldVector = new Vector3D(topLeft.X, topLeft.Y, endDepth);
                Vector3D trfVector = new Vector3D(topRight.X, topRight.Y, startDepth);
                Vector3D trdVector = new Vector3D(topRight.X, topRight.Y, endDepth);
                Vector3D blfVector = new Vector3D(bottomLeft.X, bottomLeft.Y, startDepth);
                Vector3D bldVector = new Vector3D(bottomLeft.X, bottomLeft.Y, endDepth);
                Vector3D brfVector = new Vector3D(bottomRight.X, bottomRight.Y, startDepth);
                Vector3D brdVector = new Vector3D(bottomRight.X, bottomRight.Y, endDepth);
                Vector3D[] leftVectors = new Vector3D[4] { tlfVector, tldVector, bldVector, blfVector };
                Vector3D[] rightVectors = new Vector3D[4] { trfVector, trdVector, brdVector, brfVector };
                Vector3D[] bottomVectors = new Vector3D[4] { blfVector, bldVector, brdVector, brfVector };
                Vector3D[] frontVector = new Vector3D[areaPoint.Count];
                Vector3D[] backVector = new Vector3D[areaPoint.Count];
                for (int i = 0; i < areaPoint.Count; i++)
                {
                    Point point = areaPoint[i];
                    frontVector[i] = new Vector3D(point.X, point.Y, startDepth);
                    backVector[i] = new Vector3D(point.X, point.Y, endDepth);
                }

                if ((Series as AreaSeries3D).IsAnimated || !Series.EnableAnimation)
                {
                    this.leftPolygon = new Polygon3D(
                        leftVectors, 
                        this, 
                        Series.Segments.IndexOf(this),
                        Stroke,
                        StrokeThickness,
                        Interior);
                    this.leftPolygon.CalcNormal(leftVectors[0], leftVectors[1], leftVectors[2]);
                    this.leftPolygon.CalcNormal();
                    this.area.Graphics3D.AddVisual(this.leftPolygon);

                    this.rightPolygon = new Polygon3D(
                        rightVectors,
                        this,
                        Series.Segments.IndexOf(this),
                        Stroke,
                        StrokeThickness,
                        Interior);
                    this.rightPolygon.CalcNormal(rightVectors[0], rightVectors[1], rightVectors[2]);
                    this.rightPolygon.CalcNormal();
                    this.area.Graphics3D.AddVisual(this.rightPolygon);

                    this.bottomPolygon = new Polygon3D(
                        bottomVectors,
                        this,
                        Series.Segments.IndexOf(this), 
                        Stroke,
                        StrokeThickness,
                        Interior);
                    this.bottomPolygon.CalcNormal(bottomVectors[0], bottomVectors[1], bottomVectors[2]);
                    this.bottomPolygon.CalcNormal();
                    this.area.Graphics3D.AddVisual(this.bottomPolygon);

                    this.frontPolygon = new Polygon3D(
                        frontVector, 
                        this,
                        Series.Segments.IndexOf(this), 
                        Stroke,
                        StrokeThickness, 
                        Interior);
                    this.frontPolygon.CalcNormal(frontVector[0], frontVector[1], frontVector[2]);
                    this.frontPolygon.CalcNormal();
                    this.area.Graphics3D.AddVisual(this.frontPolygon);

                    this.backPolygon = new Polygon3D(
                        backVector,
                        this, 
                        Series.Segments.IndexOf(this),
                        Stroke,
                        StrokeThickness,
                        Interior);
                    this.backPolygon.CalcNormal(backVector[0], backVector[1], backVector[2]);
                    this.backPolygon.CalcNormal();
                    this.area.Graphics3D.AddVisual(this.backPolygon);

                    for (int i = 1; i < this.yValues.Count; i++)
                    {
                        this.area.Graphics3D.AddVisual(this.topPolygonCollection[i]);
                    }
                }
            }
        }
        
        /// <summary>
        /// Method Implementation for set  Binding to ChartSegments properties.
        /// </summary>
        /// <param name="size">The Size</param>
        public override void OnSizeChanged(Size size)
        {
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the segment when Y value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var areaSegment3D = d as AreaSegment3D;
            if (areaSegment3D != null)
            {
                areaSegment3D.ScheduleRender();
            }
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
            {
                OnValueChanged();
            }
#else
            Dispatcher.BeginInvoke(OnValueChanged);
#endif
        }

        /// <summary>
        /// Updates the segment when Y value changed.
        /// </summary>
        private void OnValueChanged()
        {
            if (this.Series != null && Series.GetAnimationIsActive())
            {
                (Series as AreaSeries3D).IsAnimated = true;
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
            {
                this.OnViewChanged();
            }         
#else
            Dispatcher.BeginInvoke(OnViewChanged);
#endif
        }

        /// <summary>
        /// Updates the segment on view changed.
        /// </summary>
        private void OnViewChanged()
        {
            area.Graphics3D.PrepareView();
            area.Graphics3D.View(area.RootPanel);
        }
        
        #endregion
        
        #endregion
    }
}
