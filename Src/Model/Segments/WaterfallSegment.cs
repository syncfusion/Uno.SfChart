using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="ColumnSeries"/>    
    public partial class WaterfallSegment : ChartSegment
    {
        #region Fields

        #region Internal Fields

        internal WaterfallSegment PreviousWaterfallSegment;

        #endregion

        #region Protected Internal Fields

        /// <summary>
        /// Variables declarations
        /// </summary>
        protected internal double Left = 0d, Top = 0d, Bottom = 0d, Right = 0d;

        protected internal Line LineSegment;

        #endregion

        #region Protected Fields

        /// <summary>
        /// RectSegment property declarations
        /// </summary>
        protected Rectangle WaterfallRectSegment;

        #endregion

        #region Private Fields

        private double rectX, rectY, width, height;

        private Canvas segmentCanvas;

        private WaterfallSegmentType segmentType = WaterfallSegmentType.Positive;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Defines the Column Rectangle
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public WaterfallSegment(double x1, double y1, double x2, double y2, WaterfallSeries series)
        {
            base.Series = series;
            SetData(x1, y1, x2, y2);
        }

        /// <summary>
        /// Called when instance created for ColumnSegment
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public WaterfallSegment(double x1, double y1, double x2, double y2)
        {
            SetData(x1, y1, x2, y2);
        }

        #endregion

        #region Properties

        #region Public Properties


        /// <summary>
        /// Gets the data point value, bind with x for this segment.
        /// </summary>
        public double XData
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the data point value, bind with y for this segment.
        /// </summary>
        public double YData
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets or sets the Width of the <c>ColumnSegment</c>.
        /// </summary>
        public double Width
        {

            get
            {

                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged("Width");
            }
        }


        /// <summary>
        /// Gets or sets the Height of the <c>ColumnSegment</c>.
        /// </summary>
        public double Height
        {
            get
            {

                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Gets or sets the X position of the segment rect.
        /// </summary>
        public double RectX
        {
            get
            {
                return rectX;
            }
            set
            {
                rectX = value;
                OnPropertyChanged("RectX");
            }
        }

        /// <summary>
        /// Gets or sets the Y position of the segment RectY.
        /// </summary>
        public double RectY
        {
            get
            {

                return rectY;
            }
            set
            {
                rectY = value;
                OnPropertyChanged("RectY");
            }
        }

        #endregion

        #region Internal Properties
        
        /// <summary>
        /// Gets or sets the summary value till to this segment except this segment. 
        /// </summary>
        internal double WaterfallSum { get; set; }

        /// <summary>
        /// Gets or sets the summary value till to this segment. 
        /// </summary>
        internal double Sum { get; set; }

        internal WaterfallSegmentType SegmentType
        {
            get { return segmentType; }
            set { segmentType = value; OnPropertyChanged("SegmentType"); }
        }
        
        #endregion

        #endregion
        
        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            Left = Values[0];
            Top = Values[1];
            Right = Values[2];
            Bottom = Values[3];
            XRange = new DoubleRange(Left, Right);
            YRange = new DoubleRange(Top, Bottom);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            segmentCanvas = new Canvas();

            WaterfallRectSegment = new Rectangle();
            SetVisualBindings(WaterfallRectSegment);
            Canvas.SetZIndex(WaterfallRectSegment, 1);
            segmentCanvas.Children.Add(WaterfallRectSegment);
            WaterfallRectSegment.Tag = this;

            LineSegment = new Line();
            LineSegment.Style = (Series as WaterfallSeries).ConnectorLineStyle ?? ChartDictionaries.GenericCommonDictionary["defaultWaterfallConnectorStyle"] as Style;
            SetVisualBindings(LineSegment);
            Canvas.SetZIndex(LineSegment, 0);
            segmentCanvas.Children.Add(LineSegment);
            LineSegment.Tag = this;

            //StrokeDashArray applied only for the first element when it is applied through style. 
            //It is bug in the framework.
            //And hence manually setting stroke dash array for each and every connector line.
            DoubleCollection collection = LineSegment.StrokeDashArray;
            if (collection != null && collection.Count > 0)
            {
                DoubleCollection doubleCollection = new DoubleCollection();
                foreach (double value in collection)
                {
                    doubleCollection.Add(value);
                }
                LineSegment.StrokeDashArray = doubleCollection;
            }

            return segmentCanvas;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return segmentCanvas;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            if (transformer != null)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double left = xIsLogarithmic ? Math.Log(Left, xBase) : Left;
                double right = xIsLogarithmic ? Math.Log(Right, xBase) : Right;
                double xStart = Math.Floor(cartesianTransformer.XAxis.VisibleRange.Start);
                double xEnd = Math.Ceiling(cartesianTransformer.XAxis.VisibleRange.End);

                if (!double.IsNaN(Top) && !double.IsNaN(Bottom)
                    && (left <= xEnd && right >= xStart))
                {
                    //Calculate the rect for segment rendering.
                    rect = CalculateSegmentRect(transformer);
                    segmentCanvas.Visibility = Visibility.Visible;

                    if (WaterfallRectSegment != null)
                    {
                        WaterfallRectSegment.SetValue(Canvas.LeftProperty, rect.X);
                        WaterfallRectSegment.SetValue(Canvas.TopProperty, rect.Y);
                        WaterfallRectSegment.Visibility = Visibility.Visible;
                        Width = WaterfallRectSegment.Width = rect.Width;
                        Height = WaterfallRectSegment.Height = rect.Height;

                        //set position for connector line. 
                        UpdateConnectorLine(transformer);
                    }
                }
                else
                    segmentCanvas.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        public override void OnSizeChanged(Size size)
        {

        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method implementation for Set Bindings to properties in ColumnSegement.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            if (element is Line)
            {
                Binding binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("ShowConnector");
                binding.Converter = new BooleanToVisibilityConverter();
                binding.Mode = BindingMode.TwoWay;
                element.SetBinding(Line.VisibilityProperty, binding);
            }
            else
            {
                base.SetVisualBindings(element);
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("Stroke");
                element.SetBinding(Shape.StrokeProperty, binding);
            }
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Method used to update the connector line position. 
        /// </summary>
        /// <param name="transformer"></param>
        private void UpdateConnectorLine(IChartTransformer transformer)
        {

            if (PreviousWaterfallSegment != null && (Series as WaterfallSeries).ShowConnector)
            {
                Rect previousSegmentRect = PreviousWaterfallSegment.rect;
                LineSegment.Visibility = Visibility.Visible;
                bool isBottomLine = false;

                int i = Series.Segments.IndexOf(this);

                if (i == Series.Segments.Count - 1)
                {
                    if (Series.IsActualTransposed == true && rect.Width == 0)
                    {
                        if (Series.ActualXAxis.IsInversed)
                            rect.Y = rect.Y + rect.Height;
                        else
                            rect.Y = rect.Y - rect.Height;
                    }
                    else if (Series.IsActualTransposed == false && rect.Height == 0)
                    {
                        if (Series.ActualXAxis.IsInversed)
                            rect.X = rect.X - rect.Width;
                        else
                            rect.X = rect.X + rect.Width;
                    }
                }

                //Identifying line position
                if (PreviousWaterfallSegment.segmentType == WaterfallSegmentType.Sum)
                {
                    double sum;
                    //sum the points in YValues till this segment data. 
                    if (i != 0)
                        sum = PreviousWaterfallSegment.WaterfallSum;
                    else
                        sum = PreviousWaterfallSegment.YData;

                    if ((sum < 0 && !Series.ActualYAxis.IsInversed)
                        ||
                        (sum > 0 && Series.ActualYAxis.IsInversed))
                        isBottomLine = true;
                }
                else if (!Series.ActualYAxis.IsInversed)
                {
                    if (PreviousWaterfallSegment.YData < 0)
                        isBottomLine = true;
                }
                else if (Series.ActualYAxis.IsInversed)
                {
                    if (PreviousWaterfallSegment.SegmentType == WaterfallSegmentType.Positive
                        || PreviousWaterfallSegment.SegmentType == WaterfallSegmentType.Sum)
                        isBottomLine = true;
                }

                //Setting line points
                if (Series.IsActualTransposed == true)
                {
                    //checking x-axis in reversed or not. Setting Y1 and Y2 positions.
                    if (Series.ActualXAxis.IsInversed)
                    {
                        LineSegment.Y1 = previousSegmentRect.Width == 0 ? previousSegmentRect.Y :
                            previousSegmentRect.Y + previousSegmentRect.Height;
                        LineSegment.Y2 = rect.Y;
                    }
                    else
                    {
                        LineSegment.Y1 = previousSegmentRect.Width != 0 ? previousSegmentRect.Y :
                            previousSegmentRect.Y + previousSegmentRect.Height;
                        LineSegment.Y2 = rect.Y + rect.Height;
                    }

                    //Setting X1 and X2 positions. 
                    if (isBottomLine)
                    {
                        LineSegment.X1 = previousSegmentRect.X;
                        LineSegment.X2 = GetCurrentSegmentXValue();
                    }
                    else
                    {
                        LineSegment.X1 = previousSegmentRect.X + previousSegmentRect.Width;
                        LineSegment.X2 = GetCurrentSegmentXValue();
                    }
                }
                else
                {
                    //checking x-axis in reversed or not. Setting X1 and X2 positions.
                    if (Series.ActualXAxis.IsInversed)
                    {
                        LineSegment.X1 = previousSegmentRect.Height != 0 ? previousSegmentRect.X
                            : previousSegmentRect.X + previousSegmentRect.Width;
                        LineSegment.X2 = rect.X + rect.Width;
                    }
                    else
                    {
                        LineSegment.X1 = previousSegmentRect.Height == 0 ? previousSegmentRect.X
                            : previousSegmentRect.X + previousSegmentRect.Width;
                        LineSegment.X2 = rect.X;
                    }

                    //Setting Y1 and Y2 positions.
                    if (isBottomLine)
                    {
                        LineSegment.Y1 = previousSegmentRect.Y + previousSegmentRect.Height;
                        LineSegment.Y2 = GetCurrentSegmentYValue();
                    }
                    else
                    {
                        LineSegment.Y1 = previousSegmentRect.Y;
                        LineSegment.Y2 = GetCurrentSegmentYValue();
                    }
                }
            }
            else if (!(Series as WaterfallSeries).ShowConnector)
                LineSegment.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Returns current segment rendering x value.
        /// </summary>
        /// <returns></returns>
        private double GetCurrentSegmentXValue()
        {
            if (!Series.ActualYAxis.IsInversed)
            {
                if (SegmentType == WaterfallSegmentType.Negative
                ||
                ((SegmentType == WaterfallSegmentType.Sum
                && WaterfallSum >= 0 && (Series as WaterfallSeries).AllowAutoSum)
                    ||
                ((SegmentType == WaterfallSegmentType.Sum
                && YData >= 0 && !(Series as WaterfallSeries).AllowAutoSum))))
                    return rect.X + rect.Width;
                else
                    return rect.X;
            }
            else
            {
                if (SegmentType == WaterfallSegmentType.Negative
                ||
                ((SegmentType == WaterfallSegmentType.Sum
                && WaterfallSum >= 0 && (Series as WaterfallSeries).AllowAutoSum)
                    ||
                ((SegmentType == WaterfallSegmentType.Sum
                && YData >= 0 && !(Series as WaterfallSeries).AllowAutoSum))))
                    return rect.X;
                else
                    return rect.X + rect.Width;
            }
        }

        /// <summary>
        /// Returns current segment rendering y value. 
        /// </summary>
        /// <returns></returns>
        private double GetCurrentSegmentYValue()
        {
            if (!Series.ActualYAxis.IsInversed)
            {
                if (SegmentType == WaterfallSegmentType.Negative
                ||
                ((SegmentType == WaterfallSegmentType.Sum
                && WaterfallSum >= 0 && (Series as WaterfallSeries).AllowAutoSum)
                    ||
                ((SegmentType == WaterfallSegmentType.Sum
                && YData >= 0 && !(Series as WaterfallSeries).AllowAutoSum))))
                    return rect.Y;
                else
                    return rect.Y + rect.Height;
            }
            else
            {
                if (SegmentType == WaterfallSegmentType.Negative
                ||
                ((SegmentType == WaterfallSegmentType.Sum
                && WaterfallSum >= 0 && (Series as WaterfallSeries).AllowAutoSum)
                    ||
                ((SegmentType == WaterfallSegmentType.Sum
                && YData >= 0 && !(Series as WaterfallSeries).AllowAutoSum))))
                    return rect.Y + rect.Height;
                else
                    return rect.Y;
            }
        }

        /// <summary>
        /// Method used to calculate the segment's rendering rect.
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        private Rect CalculateSegmentRect(IChartTransformer transformer)
        {
            double spacing = (Series is HistogramSeries) ? 0.0 : (Series as ISegmentSpacing).SegmentSpacing;
            Point tlpoint = transformer.TransformToVisible(Left, Top);
            Point rbpoint = transformer.TransformToVisible(Right, Bottom);
            Rect segmentRect = new Rect(tlpoint, rbpoint);

            if (spacing > 0.0 && spacing <= 1)
            {
                if (Series.IsActualTransposed == true)
                {
                    double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing,
                        segmentRect.Bottom, segmentRect.Top);
                    segmentRect.Y = leftpos;
                    Height = segmentRect.Height = (1 - spacing) * segmentRect.Height;
                }

                else
                {
                    double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, segmentRect.Right,
                        segmentRect.Left);
                    segmentRect.X = leftpos;
                    Width = segmentRect.Width = (1 - spacing) * segmentRect.Width;
                }
            }

            return segmentRect;
        }

        #endregion

        #endregion

    }
}
