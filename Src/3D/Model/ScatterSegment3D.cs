// <copyright file="ScatterSegment3D.cs" company="Syncfusion. Inc">
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
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Class implementation for ScatterSegment3D.
    /// </summary>
    public partial class ScatterSegment3D : ChartSegment3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="ScatterWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty ScatterWidthProperty =
            DependencyProperty.Register(
                "ScatterWidth",
                typeof(double),
                typeof(ScatterSegment3D),
                new PropertyMetadata(double.NaN, OnValueChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ScatterHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ScatterHeightProperty =
            DependencyProperty.Register(
                "ScatterHeight", 
                typeof(double),
                typeof(ScatterSegment3D),
                new PropertyMetadata(double.NaN, OnValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="X"/> property.
        /// </summary>
        internal static readonly DependencyProperty XProperty =
            DependencyProperty.Register(
                "X", 
                typeof(double), 
                typeof(ScatterSegment3D),
                new PropertyMetadata(0d, OnValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Y"/> property.
        /// </summary>
        internal static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                "Y", 
                typeof(double),
                typeof(ScatterSegment3D),
                new PropertyMetadata(0d, OnValueChanged));
        
        #endregion

        #region Fields

        private Polygon3D[] plans;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterSegment3D"/> class.
        /// </summary>
        /// <param name="x1">The X1 Value</param>
        /// <param name="y1">The Y1 Value</param>
        /// <param name="startDepth">The Start Depth</param>
        /// <param name="endDepth">The End Depth</param>
        /// <param name="scatterSeries3D">The Scatter Series</param>
        public ScatterSegment3D(double x1, double y1, double startDepth, double endDepth, ScatterSeries3D scatterSeries3D)
        {
            Series = scatterSeries3D;
            SetData(x1, y1, startDepth, endDepth);
        }

        #endregion
        
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets scatter segment’s width.
        /// </summary>
        public double ScatterWidth
        {
            get { return (double)GetValue(ScatterWidthProperty); }
            set { SetValue(ScatterWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets scatter segment’s height.
        /// </summary>
        public double ScatterHeight
        {
            get { return (double)GetValue(ScatterHeightProperty); }
            set { SetValue(ScatterHeightProperty, value); }
        }
        
        /// <summary>
        /// Gets XData property
        /// </summary>
        public double XData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets YData property
        /// </summary>
        public double YData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets ZData property
        /// </summary>
        public double ZData
        {
            get;
            internal set;
        }

        #endregion

        #region Internal Properties
        
        /// <summary>
        /// Gets or sets the plans
        /// </summary>
        internal Polygon3D[] Plans
        {
            get { return plans; }
            set { plans = value; }
        }

        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        internal double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the Y value.
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
        /// <param name="Values">The Column Segment Values</param>
        public override void SetData(params double[] Values)
        {
            this.X = Values[0];
            this.Y = Values[1];
            this.startDepth = Values[2];
            this.endDepth = Values[3];

            if (!double.IsNaN(X))
                XRange = DoubleRange.Union(X);
            else
                XRange = DoubleRange.Empty;
            if (!double.IsNaN(Y))
                YRange = DoubleRange.Union(Y);
            else
                YRange = DoubleRange.Empty;

            ZRange = new DoubleRange(startDepth, endDepth);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// returns UIElement
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            SetVisualBindings(null);
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
            Rect rect;
            double x, y, width, height;
            if (transformer == null) return;
            var cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            if (cartesianTransformer == null) return;
            if (double.IsNaN(YData) && !Series.ShowEmptyPoints) return;

            ScatterSeries3D series = (Series as ScatterSeries3D);
            var xBase = cartesianTransformer.XAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.XAxis).LogarithmicBase : 1;
            var yBase = cartesianTransformer.YAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.YAxis).LogarithmicBase : 1;
            var xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
            var yIsLogarithmic = cartesianTransformer.YAxis.IsLogarithmic;
            var xValue = xIsLogarithmic ? Math.Log(X, xBase) : X;
            var yValue = yIsLogarithmic ? Math.Log(Y, yBase) : Y;
            var yStart = cartesianTransformer.YAxis.VisibleRange.Start;
            var yEnd = cartesianTransformer.YAxis.VisibleRange.End;
            var xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            var xEnd = cartesianTransformer.XAxis.VisibleRange.End;
            var zSeries = this.Series as XyzDataSeries3D;
            bool isZAxis = cartesianTransformer.ZAxis != null && zSeries.ActualZAxis != null && (zSeries as XyzDataSeries3D).ActualZValues != null;
            Point tlpoint = new Point(0, 0);
            double frontDepth = 0d;
            double backDepth = 0d;
            if (isZAxis)
            {
                var z1value = startDepth;
                var z2value = endDepth;
                var depthDelta = (this.Series as ScatterSeries3D).GetSegmentDepth((this.Series.ActualArea as SfChart3D).ActualDepth).Delta / 2;
                var zStart = cartesianTransformer.ZAxis.VisibleRange.Start;
                var zEnd = cartesianTransformer.ZAxis.VisibleRange.End;

                var zIsLogarithmic = cartesianTransformer.ZAxis.IsLogarithmic;
                var zBase = zIsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.ZAxis).LogarithmicBase : 1;

                var actualZ1 = zIsLogarithmic ? Math.Log(z1value, zBase) : z1value;
                var actualZ2 = zIsLogarithmic ? Math.Log(z2value, zBase) : z2value;

                if (!(actualZ1 <= zEnd && actualZ2 >= zStart)) return;

                var zLogStart = zIsLogarithmic ? Math.Pow(zBase, zStart) : zStart;
                var zLogEnd = zIsLogarithmic ? Math.Pow(zBase, zEnd) : zEnd;
                var tldpoint = cartesianTransformer.TransformToVisible3D(xValue, yValue, actualZ1 < zStart ? zLogStart : actualZ1 > zEnd ? zLogEnd : z1value);
                tlpoint = new Point(tldpoint.X, tldpoint.Y);

                frontDepth = (z1value == zStart) ? tldpoint.Z : tldpoint.Z - ScatterHeight / 2 < tldpoint.Z - depthDelta ? tldpoint.Z - depthDelta : tldpoint.Z - ScatterHeight / 2;
                backDepth = (z2value == zEnd) ? tldpoint.Z : tldpoint.Z + ScatterHeight / 2 > tldpoint.Z + depthDelta ? tldpoint.Z + depthDelta : tldpoint.Z + ScatterHeight / 2;
            }
            else
            {
                tlpoint = transformer.TransformToVisible(X, Y);
            }

            if (!series.Area.SeriesClipRect.Contains(tlpoint)
                && ((xValue != xEnd && xValue != xStart) || (yValue != yEnd && yValue != yStart)) && !(series.IsTransposed))
                return;

            if (series.ScatterHeight <= 0 || series.ScatterWidth <= 0)
                return;

            x = (xValue == (series.IsTransposed ? xEnd : xStart)) ? tlpoint.X : tlpoint.X - (series.IsTransposed ? ScatterHeight / 2 : ScatterWidth / 2);
            y = (yValue == (series.IsTransposed ? yStart : yEnd)) ? tlpoint.Y : tlpoint.Y - (series.IsTransposed ? ScatterWidth / 2 : ScatterHeight / 2);
            width = (xValue == xStart) || (xValue == xEnd) ? ScatterWidth / 2 : ScatterWidth;
            height = (yValue == yStart) || (yValue == yEnd) ? ScatterHeight / 2 : ScatterHeight;
            rect = new Rect(x, y, width, height);

            if (!series.IsTransposed)
            {
                // Clipping segment of nearest range point
                if (!Series.ActualArea.SeriesClipRect.Contains(new Point(x, y)))
                    rect = new Rect(x, Series.ActualArea.SeriesClipRect.Top, width, height + y);
                if (!Series.ActualArea.SeriesClipRect.Contains(new Point(rect.Left, rect.Bottom)))
                    rect = new Rect(x, y, width, Math.Abs(height + (Series.ActualArea.SeriesClipRect.Bottom - rect.Bottom) > ScatterHeight ? height : height + (Series.ActualArea.SeriesClipRect.Bottom - rect.Bottom)));
                if (!Series.ActualArea.SeriesClipRect.Contains(new Point(rect.Left, rect.Top)))
                {
                    var modifiedWidth = Math.Abs(width - (Series.ActualXAxis.RenderedRect.X - rect.X));
                    rect = new Rect(Series.ActualXAxis.RenderedRect.X, y, modifiedWidth, rect.Height);
                }

                if (!Series.ActualArea.SeriesClipRect.Contains(new Point(rect.Left + rect.Width, rect.Top)))
                {
                    var modifiedWidth = Math.Abs(rect.Width + (Series.ActualArea.SeriesClipRect.Right - rect.Right));
                    rect = new Rect(rect.X, rect.Y, modifiedWidth, rect.Height);
                }
            }
            else
            {
                rect = new Rect(x, y, height, width);
                if (x < tlpoint.X && xValue == xStart && yValue == yStart)
                    rect = new Rect(tlpoint.X, y, height, width);
                if (y < tlpoint.Y && yValue == yStart)
                    rect = new Rect(tlpoint.X, tlpoint.Y, height, width);
                if (y == tlpoint.Y && yValue == yStart)
                    rect = new Rect(tlpoint.X, tlpoint.Y - ScatterWidth / 2, height, width);
                if (y < tlpoint.Y && xValue == xEnd)
                    rect = new Rect(tlpoint.X, tlpoint.Y, height, width);
                if (x == tlpoint.X && xValue == xEnd)
                    rect = new Rect(tlpoint.X, tlpoint.Y, height, width);
                if (x == tlpoint.X && xValue == xEnd && yValue != yStart)
                    rect = new Rect(tlpoint.X - ScatterHeight / 2, tlpoint.Y, height, width);
            }

            var area = Series.ActualArea as SfChart3D;

            Vector3D tlfVector = new Vector3D(0, 0, 0);
            Vector3D brbVector = new Vector3D(0, 0, 0);

            if (isZAxis)
            {
                tlfVector = new Vector3D(rect.Left, rect.Top, frontDepth);
                brbVector = new Vector3D(rect.Right, rect.Bottom, backDepth);
            }
            else
            {
                tlfVector = new Vector3D(rect.Left, rect.Top, startDepth);
                brbVector = new Vector3D(rect.Right, rect.Bottom, startDepth + ScatterHeight > endDepth ? endDepth : startDepth + ScatterHeight);
            }

            if (plans == null)
                plans = Polygon3D.CreateBox(
                    tlfVector, 
                    brbVector,
                    this, 
                    series.Segments.IndexOf(this), 
                    area.Graphics3D,
                    Stroke, 
                    Interior,
                    StrokeThickness,
                    Series.IsActualTransposed);
            else
                Polygon3D.UpdateBox(plans, tlfVector, brbVector, Interior, Visibility.Visible);
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">The Size</param>
        public override void OnSizeChanged(Size size)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method Implementation for set Binding to ChartSegments properties.
        /// </summary>
        /// <param name="element">The Shape Element</param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = Series;
            binding.Path = new PropertyPath("ScatterWidth");
            BindingOperations.SetBinding(this, ScatterSegment3D.ScatterWidthProperty, binding);
            binding = new Binding();
            binding.Source = Series;
            binding.Path = new PropertyPath("ScatterHeight");
            BindingOperations.SetBinding(this, ScatterSegment3D.ScatterHeightProperty, binding);
        }

        #endregion

#region Private Static Methods

        /// <summary>
        /// Updates the segment when <see cref="X"/>, <see cref="Y"/>, <see cref="ScatterWidth"/>, <see cref="ScatterHeight"/> values changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var scatterSegment3D = d as ScatterSegment3D;
            if (scatterSegment3D != null) scatterSegment3D.OnValueChanged();
        }

        #endregion

        /// <summary>
        /// Updates the segment when <see cref="X"/>, <see cref="Y"/>, <see cref="ScatterWidth"/>, <see cref="ScatterHeight"/> values changed.
        /// </summary>
        private void OnValueChanged()
        {
            if (Series != null && Series.GetAnimationIsActive())
                Update(Series.CreateTransformer(new Size(), false));
        }     

        #endregion
    }
}
