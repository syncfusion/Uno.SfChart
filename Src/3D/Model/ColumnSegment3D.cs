// <copyright file="ColumnSegment3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents chart column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WinRT Chart building system.</remarks>
    public partial class ColumnSegment3D : ChartSegment3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Top"/> property.
        /// </summary>
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register(
                "Top",
                typeof(double),
                typeof(ColumnSegment3D),
                new PropertyMetadata(0d, OnValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Bottom"/> property.
        /// </summary>
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.Register(
                "Bottom",
                typeof(double),
                typeof(ColumnSegment3D),
                new PropertyMetadata(0d, OnValueChanged));

        #endregion

        #region Fields
      
        private double rectX, rectY, width, height;

        private double internaltop;

        private Polygon3D[] plans;

        private double internalBottom;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnSegment3D"/> class.
        /// </summary>
        /// <param name="x1">The X1 Value</param>
        /// <param name="y1">The Y1 Value</param>
        /// <param name="x2">The X2 Value</param>
        /// <param name="y2">The Y2 Value</param>
        /// <param name="startDepth">The Start Depth</param>
        /// <param name="endDepth">The End Depth</param> 
        /// <param name="series">The Series</param>        /// 
        public ColumnSegment3D(double x1, double y1, double x2, double y2, double startDepth, double endDepth, ChartSeriesBase series)
        {
            Left = 0d;
            Right = 0d;
            this.Series = series;
            this.SetData(x1, y1, x2, y2, startDepth, endDepth);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnSegment3D"/> class.
        /// </summary>
        /// <param name="x1">The X1 Value</param>
        /// <param name="y1">The Y1 Value</param>
        /// <param name="x2">The X2 Value</param>
        /// <param name="y2">The Y2 Value</param>
        /// <param name="startDepth">The Start Depth</param>
        /// <param name="endDepth">The End Depth</param>
        public ColumnSegment3D(double x1, double y1, double x2, double y2, double startDepth, double endDepth)
        {
            Left = 0d;
            Right = 0d;
            this.SetData(x1, y1, x2, y2, startDepth, endDepth);
        }
        
        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the XData property
        /// </summary>
        public double XData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the YData property
        /// </summary>
        public double YData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the ZData property
        /// </summary>
        public double ZData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets Width property
        /// </summary>
        public double Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
                this.OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Gets or sets Height property
        /// </summary>
        public double Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;
                this.OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Gets or sets <see cref="RectX"/> property
        /// </summary>
        public double RectX
        {
            get
            {
                return this.rectX;
            }

            set
            {
                this.rectX = value;
                this.OnPropertyChanged("RectX");
            }
        }

        /// <summary>
        /// Gets or sets <see cref="RectY"/> property
        /// </summary>
        public double RectY
        {
            get
            {
                return this.rectY;
            }

            set
            {
                this.rectY = value;
                OnPropertyChanged("RectY");
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { this.SetValue(TopProperty, value); }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        public double Bottom
        {
            get { return (double)GetValue(BottomProperty); }
            set { this.SetValue(BottomProperty, value); }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        public double InternalBottom
        {
            get { return this.internalBottom; }
            set { this.internalBottom = value; }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the polygon 3D Plans.
        /// </summary>
        internal Polygon3D[] Plans
        {
            get { return this.plans; }
            set { this.plans = value; }
        }

        /// <summary>
        /// Gets or sets the internal top.
        /// </summary>
        internal double InternalTop
        {
            get { return this.internaltop; }
            set { this.internaltop = value; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the left of the segment.
        /// </summary>
        protected double Left { get; set; }

        /// <summary>
        /// Gets or sets the right of the segment.
        /// </summary>
        protected double Right { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="values">The Column Segment Values</param>
        public override void SetData(params double[] values)
        {
            this.Plans = null;
            this.Left = values[0];
            this.internalBottom = this.Bottom = values[3];
            this.internaltop = this.Top = values[1];
            this.Right = values[2];
            this.startDepth = values[4];
            this.endDepth = values[5];
            this.XRange = new DoubleRange(this.Left, this.Right);
            this.ZRange = new DoubleRange(startDepth, endDepth);
            if (!double.IsNaN(this.Top) && !double.IsNaN(this.Bottom))
                this.YRange = new DoubleRange(this.Top, this.Bottom);
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
            if (transformer == null)
            {
                return;
            }

            var cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            if (cartesianTransformer == null)
            {
                return;
            }

            if (double.IsNaN(this.YData) && !Series.ShowEmptyPoints)
            {
                return;
            }

            var xBase = cartesianTransformer.XAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.XAxis).LogarithmicBase : 1;
            var xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
            var left = xIsLogarithmic ? Math.Log(this.Left, xBase) : this.Left;
            var right = xIsLogarithmic ? Math.Log(this.Right, xBase) : this.Right;
            var yBase = cartesianTransformer.YAxis.IsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.YAxis).LogarithmicBase : 1;
            var yIsLogarithmic = cartesianTransformer.YAxis.IsLogarithmic;
            var bottom = yIsLogarithmic ? Math.Pow(yBase, cartesianTransformer.YAxis.VisibleRange.Start) : cartesianTransformer.YAxis.VisibleRange.Start;
            var top = yIsLogarithmic ? Math.Pow(yBase, cartesianTransformer.YAxis.VisibleRange.End) : cartesianTransformer.YAxis.VisibleRange.End;
            var xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            var xEnd = cartesianTransformer.XAxis.VisibleRange.End;

            double z1 = startDepth, z2 = endDepth;

            var zSeries = this.Series as XyzDataSeries3D;

            bool isZAxis = cartesianTransformer.ZAxis != null && zSeries.ActualZAxis != null && zSeries.ActualZValues != null;

            double spacing = (Series as ISegmentSpacing).SegmentSpacing;
            if (!(left <= xEnd && right >= xStart))
            {
                return;
            }

            // WPF -14524 3D Column and Bar Series is rendering out of the pane. while cross the bar value to visualRange of axis.
            double topValue;
            if (this.Top < 0)
            {
                topValue = this.Top > bottom ? this.Top : bottom;
            }
            else
            {
                topValue = this.Top < top ? this.Top : top;
            }

            this.Bottom = this.Bottom > top ? top : this.Bottom;
            if (spacing > 0 && spacing <= 1)
            {
                double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, right, left);
                double rightpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, left, right);
                this.Left = leftpos;
                this.Right = rightpos;
            }

            var area = Series.ActualArea as SfChart3D;
            var tlfVector = new Vector3D(0, 0, 0);
            var brbVector = new Vector3D(0, 0, 0);
            
            var tlpoint = transformer.TransformToVisible(left > xStart ? this.Left : xStart, topValue < bottom ? bottom : topValue);
            var rbpoint = transformer.TransformToVisible(xEnd > right ? this.Right : xEnd, bottom > this.Bottom ? bottom : this.Bottom);

            if (isZAxis)
            {
                double zStart = cartesianTransformer.ZAxis.VisibleRange.Start;
                double zEnd = cartesianTransformer.ZAxis.VisibleRange.End;

                var zIsLogarithmic = cartesianTransformer.ZAxis.IsLogarithmic;
                var zBase = zIsLogarithmic ? ((LogarithmicAxis3D)cartesianTransformer.ZAxis).LogarithmicBase : 1;

                var actualZ1 = zIsLogarithmic ? Math.Log(z1, zBase) : z1;
                var actualZ2 = zIsLogarithmic ? Math.Log(z2, zBase) : z2;

                if (!(actualZ1 <= zEnd && actualZ2 >= zStart)) return;

                tlfVector = cartesianTransformer.TransformToVisible3D(this.Left > xStart ? this.Left : xStart, topValue < bottom ? bottom : topValue, z1 > zStart ? z1 : zStart);
                brbVector = cartesianTransformer.TransformToVisible3D(xEnd > this.Right ? this.Right : xEnd, bottom > this.Bottom ? bottom : this.Bottom, zEnd > z2 ? z2 : zEnd);
            }
            else
            {
                var rect = new Rect(tlpoint, rbpoint);

                tlfVector = new Vector3D(rect.Left, rect.Top, z1);
                brbVector = new Vector3D(rect.Right, rect.Bottom, z2);
            }

            if (this.plans == null)
            {
                this.plans = Polygon3D.CreateBox(
                    tlfVector,
                    brbVector,
                    this,
                    Series.Segments.IndexOf(this),
                    area.Graphics3D,
                    this.Stroke,
                    Interior,
                    this.StrokeThickness,
                    Series.IsActualTransposed);
            }
            else
            {
                Polygon3D.UpdateBox(this.plans, tlfVector, brbVector, this.Interior, tlpoint.Y == rbpoint.Y ? Visibility.Collapsed : Visibility.Visible);
            }
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
        /// Method implementation for Set Bindings to properties in ColumnSegment.
        /// </summary>
        /// <param name="element">The Element To Be Bind</param>
        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the segment on top or bottom Value Changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var columnSegment3D = d as ColumnSegment3D;
            if (columnSegment3D != null) columnSegment3D.OnValueChanged();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the segment on top or bottom Value Changed.
        /// </summary>
        private void OnValueChanged()
        {
            if (this.Series != null && Series.GetAnimationIsActive()
                && this.Series.ActualArea != null && !(this.Series.ActualArea as SfChart3D).IsRotationScheduleUpdate)
                this.Update(Series.CreateTransformer(new Size(), false));
        }

        #endregion

        #endregion
    }
}
