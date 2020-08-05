using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart candle segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="CandleSeries"/>

    public partial class CandleSegment : ChartSegment
    {
        #region Fields

        #region Private Fields

        private bool isBull;

        private bool isFill;

        private ChartPoint cdpBottomLeft;

        private ChartPoint cdpRightTop;

        private ChartPoint hiPoint;

        private ChartPoint loPoint;

        private ChartPoint hiPoint1;

        private ChartPoint loPoint1;

        private Line hiLoLine;

        private Line hiLoLine1;

        private Line openCloseLine;

        private Canvas segmentCanvas;

        private Rectangle columnSegment;

        private Brush bullFillColor, bearFillColor;

        #endregion

        #endregion

        #region constructor

        public CandleSegment()
        {

        }

        /// <summary>
        /// Called when Instance created for CandleSegment
        /// </summary>
        /// <param name="cdpBottomLeft"></param>
        /// <param name="cdpRightTop"></param>
        /// <param name="hipoint"></param>
        /// <param name="lopoint"></param>
        /// <param name="isbull"></param>
        /// <param name="series"></param>
        public CandleSegment(Point cdpBottomLeft, Point cdpRightTop, Point hipoint, Point lopoint,
              bool isbull, CandleSeries series, object item)
        {

        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the actual color used to paint the interior of the segment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush ActualInterior
        {
            get
            {
                if (Series.ActualArea.SelectedSeriesCollection.Contains(Series)
                    && Series.ActualArea.GetEnableSeriesSelection() && Series.ActualArea.GetSeriesSelectionBrush(Series) != null)
                    return Series.ActualArea.GetSeriesSelectionBrush(Series);
                else if ((Series as ISegmentSelectable).SegmentSelectionBrush != null && IsSegmentSelected())
                    return (Series as ISegmentSelectable).SegmentSelectionBrush;
                else
                {
                    if (isFill || (Series as CandleSeries).ComparisonMode == FinancialPrice.None)
                        return this.Series.Interior != null ? Series.Interior : IsBull
                            ? BullFillColor : BearFillColor;
                    else
                        return this.Series.Interior != null ? this.Interior : new SolidColorBrush(Colors.Transparent);
                }
            }
        }
        public Brush ActualStroke
        {
            get
            {
                CandleSeries series = Series as CandleSeries;
                if (series.Stroke != null)
                    return series.Stroke;
                else
                    return IsBull ? BullFillColor : BearFillColor;
            }
        }

        /// <summary>
        /// Gets or sets the interior of the segment represents bear value.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BearFillColor
        {
            get
            {
                return bearFillColor == null
                    ? this.Interior : bearFillColor;
            }
            set
            {
                if (bearFillColor != value)
                {
                    bearFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets or sets the interior of the segment represents bull value.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BullFillColor
        {
            get
            {
                return bullFillColor == null
                    ? this.Interior : bullFillColor;
            }
            set
            {
                if (bullFillColor != value)
                {
                    bullFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets or sets the high value of this segment.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the low value of this segment.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the open value of this segment.
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// Gets or sets the close value of this segment.
        /// </summary>
        public double Close { get; set; }

        #endregion

        #region Private Properties

        private bool IsBull
        {
            get
            {
                return isBull;
            }
            set
            {
                if (isBull != value)
                {
                    isBull = value;
                    OnPropertyChanged("ActualInterior");
                    OnPropertyChanged("ActualStroke");
                }
            }
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
        /// <param name="BottomLeft"></param>
        /// <param name="RightTop"></param>
        /// <param name="hipoint"></param>
        /// <param name="loPoint"></param>
        /// <param name="isBull"></param>

        [Obsolete("Use SetData(ChartPoint point1, ChartPoint point2, ChartPoint point3,ChartPoint point4,bool isBull)")]
        public override void SetData(Point BottomLeft, Point RightTop, Point hipoint, Point loPoint, bool isBull)
        {
            this.cdpBottomLeft.X = BottomLeft.X; this.cdpBottomLeft.Y = BottomLeft.Y;
            this.cdpRightTop.X = RightTop.X; this.cdpRightTop.Y = RightTop.Y;
            this.hiPoint.X = hipoint.X; this.hiPoint.Y = hipoint.Y;
            this.loPoint.X = loPoint.X; this.loPoint.Y = loPoint.Y;
            this.isFill = (cdpRightTop.Y < cdpBottomLeft.Y);
            if (!isFill)
            {
                this.hiPoint1 = new ChartPoint(hiPoint.X, cdpRightTop.Y);
                this.loPoint1 = new ChartPoint(loPoint.X, cdpBottomLeft.Y);
            }
            else
            {
                this.hiPoint1 = new ChartPoint(hiPoint.X, cdpBottomLeft.Y);
                this.loPoint1 = new ChartPoint(loPoint.X, cdpRightTop.Y);
            }
            this.IsBull = isBull;
            var alignedValues = AlignHiLoSegment(cdpBottomLeft.Y, cdpRightTop.Y, hiPoint.Y, loPoint.Y);
            this.hiPoint.Y = alignedValues[0];
            this.loPoint.Y = alignedValues[1];
            XRange = DoubleRange.Union(new double[] { BottomLeft.X, RightTop.X, hipoint.X, loPoint.X });
            //Candle Series show empty point set to false and IsTransposed to true exception thrown/WPF-14327
            if (!double.IsNaN(BottomLeft.Y) || !double.IsNaN(RightTop.Y) || !double.IsNaN(hipoint.Y) || !double.IsNaN(loPoint.Y))
                YRange = DoubleRange.Union(new double[] { BottomLeft.Y, RightTop.Y, hipoint.Y, loPoint.Y });
            else
                YRange = DoubleRange.Empty;
        }

        public override void SetData(ChartPoint BottomLeft, ChartPoint RightTop, ChartPoint hipoint, ChartPoint loPoint, bool isBull)
        {
            this.cdpBottomLeft = BottomLeft;
            this.cdpRightTop = RightTop;
            this.hiPoint = hipoint;
            this.loPoint = loPoint;
            this.isFill = (cdpRightTop.Y < cdpBottomLeft.Y);
            if (!isFill)
            {
                this.hiPoint1 = new ChartPoint(hiPoint.X, cdpRightTop.Y);
                this.loPoint1 = new ChartPoint(loPoint.X, cdpBottomLeft.Y);
            }
            else
            {
                this.hiPoint1 = new ChartPoint(hiPoint.X, cdpBottomLeft.Y);
                this.loPoint1 = new ChartPoint(loPoint.X, cdpRightTop.Y);
            }
            this.IsBull = isBull;
            var alignedValues = AlignHiLoSegment(cdpBottomLeft.Y, cdpRightTop.Y, hiPoint.Y, loPoint.Y);
            this.hiPoint.Y = alignedValues[0];
            this.loPoint.Y = alignedValues[1];
            XRange = DoubleRange.Union(new double[] { BottomLeft.X, RightTop.X, hipoint.X, loPoint.X });
            //Candle Series show empty point set to false and IsTransposed to true exception thrown/WPF-14327
            if (!double.IsNaN(BottomLeft.Y) || !double.IsNaN(RightTop.Y) || !double.IsNaN(hipoint.Y) || !double.IsNaN(loPoint.Y))
                YRange = DoubleRange.Union(new double[] { BottomLeft.Y, RightTop.Y, hipoint.Y, loPoint.Y });
            else
                YRange = DoubleRange.Empty;
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

            columnSegment = new Rectangle();
            SetVisualBindings(columnSegment);
            Canvas.SetZIndex(columnSegment, 1);
            segmentCanvas.Children.Add(columnSegment);
            columnSegment.Tag = this;

            hiLoLine = new Line();
            SetVisualBindings(hiLoLine);
            Canvas.SetZIndex(hiLoLine, 0);
            segmentCanvas.Children.Add(hiLoLine);
            hiLoLine.Tag = this;

            hiLoLine1 = new Line();
            SetVisualBindings(hiLoLine1);
            Canvas.SetZIndex(hiLoLine1, 0);
            segmentCanvas.Children.Add(hiLoLine1);
            hiLoLine1.Tag = this;

            openCloseLine = new Line();
            SetVisualBindings(openCloseLine);
            segmentCanvas.Children.Add(openCloseLine);
            openCloseLine.Tag = this;

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
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>

        public override void Update(IChartTransformer transformer)
        {
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            double xStart = Math.Floor(cartesianTransformer.XAxis.VisibleRange.Start);
            double xEnd = Math.Ceiling(cartesianTransformer.XAxis.VisibleRange.End);
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
            double left = xIsLogarithmic ? Math.Log(cdpBottomLeft.X, xBase) : cdpBottomLeft.X;
            double right = xIsLogarithmic ? Math.Log(cdpRightTop.X, xBase) : cdpRightTop.X;
            if ((left <= xEnd && right >= xStart) || Series.ShowEmptyPoints)
            {
                double spacing = (Series as ISegmentSpacing).SegmentSpacing;
                columnSegment.Visibility = Visibility.Visible;
                hiLoLine.Visibility = Visibility.Visible;
                hiLoLine1.Visibility = Visibility.Visible;
                openCloseLine.Visibility = Visibility.Visible;
                Point blpoint = transformer.TransformToVisible(cdpBottomLeft.X, cdpBottomLeft.Y);
                Point trpoint = transformer.TransformToVisible(cdpRightTop.X, cdpRightTop.Y);
                rect = new Rect(blpoint, trpoint);
                if (spacing > 0 && spacing <= 1)
                {
                    if (Series.IsActualTransposed)
                    {
                        rect.Y = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, rect.Bottom, rect.Top);
                        columnSegment.Height = rect.Height = (1 - spacing) * rect.Height;
                    }
                    else
                    {
                        rect.X = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, rect.Right, rect.Left);
                        columnSegment.Width = rect.Width = (1 - spacing) * rect.Width;
                    }
                }
                if (cdpBottomLeft.Y == cdpRightTop.Y)
                {
                    if (!double.IsNaN(cdpBottomLeft.Y) && !double.IsNaN(cdpRightTop.Y))
                    {
                        columnSegment.Visibility = Visibility.Collapsed;
                        this.openCloseLine.X1 = rect.Left;
                        this.openCloseLine.Y1 = rect.Top;
                        this.openCloseLine.X2 = rect.Right;
                        this.openCloseLine.Y2 = rect.Bottom;
                    }
                    else
                    {
                        openCloseLine.Visibility = Visibility.Collapsed;
                        openCloseLine.ClearUIValues();
                    }
                }
                else if (!double.IsNaN(cdpBottomLeft.Y) && !double.IsNaN(cdpRightTop.Y))
                {
                    openCloseLine.Visibility = Visibility.Collapsed;
                    columnSegment.SetValue(Canvas.LeftProperty, rect.X);
                    columnSegment.SetValue(Canvas.TopProperty, rect.Y);
                    columnSegment.Width = rect.Width;
                    columnSegment.Height = rect.Height;
                }
                else
                {
                    columnSegment.Visibility = Visibility.Collapsed;
                    columnSegment.ClearUIValues();
                }
                Point point1 = transformer.TransformToVisible(this.hiPoint.X, this.hiPoint.Y);
                Point point2 = transformer.TransformToVisible(this.hiPoint1.X, this.hiPoint1.Y);
                Point point3 = transformer.TransformToVisible(this.loPoint.X, this.loPoint.Y);
                Point point4 = transformer.TransformToVisible(this.loPoint1.X, this.loPoint1.Y);
                if (!double.IsNaN(point1.Y) && !double.IsNaN(point2.Y) && !double.IsNaN(point1.X) && !double.IsNaN(point2.X) && !double.IsNaN(point3.Y) && !double.IsNaN(point4.Y) && !double.IsNaN(point3.X) && !double.IsNaN(point4.X))
                {
                    this.hiLoLine.X1 = point1.X;
                    this.hiLoLine.X2 = point2.X;
                    this.hiLoLine.Y1 = point1.Y;
                    this.hiLoLine.Y2 = point2.Y;
                    this.hiLoLine1.X1 = point3.X;
                    this.hiLoLine1.X2 = point4.X;
                    this.hiLoLine1.Y1 = point3.Y;
                    this.hiLoLine1.Y2 = point4.Y;
                    if (rect.Contains(point1))
                        this.hiLoLine.Visibility = Visibility.Collapsed;
                    if (rect.Contains(point3))
                        this.hiLoLine1.Visibility = Visibility.Collapsed;
                }
                else
                {
                    hiLoLine.Visibility = Visibility.Collapsed;
                    hiLoLine.ClearUIValues();
                    hiLoLine1.Visibility = Visibility.Collapsed;
                    hiLoLine1.ClearUIValues();
                }
            }
            else
            {
                columnSegment.ClearUIValues();
                hiLoLine.ClearUIValues();
                hiLoLine1.ClearUIValues();
                openCloseLine.ClearUIValues();
                columnSegment.Visibility = Visibility.Collapsed;
                hiLoLine.Visibility = Visibility.Collapsed;
                hiLoLine1.Visibility = Visibility.Collapsed;
                openCloseLine.Visibility = Visibility.Collapsed;
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
        /// Method Implementation for set Binding to CandleSegment properties.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.FillProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualStroke");
            element.SetBinding(Shape.StrokeProperty, binding);
        }


        /// <summary>
        /// Called when Property changed
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged(string name)
        {
            if (name == "Interior")
                name = "ActualInterior";
            else if (name == "Stroke")
                name = "ActualStroke";
            base.OnPropertyChanged(name);
        }

        #endregion

        internal override void Dispose()
        {
            if (segmentCanvas != null)
            {
                segmentCanvas.Children.Clear();
                segmentCanvas = null;
            }
            if (columnSegment != null)
            {
                columnSegment.Tag = null;
                columnSegment = null;
            }
            if (hiLoLine != null)
            {
                hiLoLine.Tag = null;
                hiLoLine = null;
            }
            if (hiLoLine1 != null)
            {
                hiLoLine1.Tag = null;
                hiLoLine1 = null;
            }
            if (openCloseLine != null)
            {
                openCloseLine.Tag = null;
                openCloseLine = null;
            }

            base.Dispose();
        }

        #endregion
    }
}
