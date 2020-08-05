using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Charts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
//using Syncfusion.ChartCPP.WinRT;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart fast line segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineSeries"/>
    public partial class FastLineSegment: ChartSegment
    {
        #region Fields

        #region Internal Fields
        
        internal DataTemplate customTemplate;
        
        internal Polyline polyline;

        internal ChartSeries fastSeries;

        #endregion

        #region Protected Fields

        protected List<double> xValues = new List<double>();
        protected List<double> yValues = new List<double>();

        #endregion

        #region Private Fields

        bool segmentUpdated;

        PointCollection points;

        ContentControl control;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, xOffset, yOffset;

        double xTolerance, yTolerance;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastLineSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastLineSegment
        /// </summary>
        /// <param name="series"></param>
        public FastLineSegment(ChartSeries series)
        {
            fastSeries = series;
        }

        /// <summary>
        /// Called when instance created for FastLineSegment
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        /// <param name="series"></param>
        public FastLineSegment(IList<double> xVals, IList<double> yVals, AdornmentSeries series)
            : this(series)
        {
            base.Series = series;
            this.xChartVals = xVals;
            this.yChartVals = yVals;
            base.Item = series.ActualData;
            var fastLineSeries = series as FastLineSeries;
            if (fastLineSeries != null)
                customTemplate = fastLineSeries.CustomTemplate;
            this.SetRange();
        }

        #endregion

        #region Properties

        #region Public Properties

        //private Image image;
        /// <summary>
        /// Gets or sets renderingMode
        /// </summary>

        public RenderingMode RenderingMode
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets Points property
        /// </summary>

        public PointCollection Points
        {
            get { return points; }
            set
            {
                points = value;
                OnPropertyChanged("Points");
            }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets xChartVals property
        /// </summary>
        protected internal IList<double> xChartVals { get; set; }
        /// <summary>
        /// Gets or sets yChartVals property
        /// </summary>
        protected internal IList<double> yChartVals { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>

        public override void SetData(IList<double> xVals, IList<double> yVals)
        {
            this.xChartVals = xVals;
            this.yChartVals = yVals;
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
            if (customTemplate == null)
            {
                polyline = new Polyline();
                polyline.Tag = this;
                SetVisualBindings(polyline);
                return polyline;
            }
            else
            {
                control = new ContentControl();
                control.Content = this;
                control.Tag = this;
                control.ContentTemplate = customTemplate;
                return control;
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

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>

        public override UIElement GetRenderedVisual()
        {
            if (customTemplate == null)
                return polyline;
            return control;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>

        public override void Update(IChartTransformer transformer)
        {
            //if (transformer != null && chartPoints != null && chartPoints.Count > 1)
            if (transformer != null && fastSeries.DataCount > 1)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                var datetime = cartesianTransformer.XAxis as DateTimeAxis;
                //if business hour axis then point calculated from tranform to visible method
                if (datetime != null && datetime.EnableBusinessHours)
                {
                    CalculatePonts(cartesianTransformer);
                }
                else
                {
                    bool isLogarithmic = cartesianTransformer.XAxis.IsLogarithmic || cartesianTransformer.YAxis.IsLogarithmic;
                    x_isInversed = cartesianTransformer.XAxis.IsInversed;
                    y_isInversed = cartesianTransformer.YAxis.IsInversed;
                    xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                    xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                    yStart = cartesianTransformer.YAxis.VisibleRange.Start;
                    yEnd = cartesianTransformer.YAxis.VisibleRange.End;
                    xDelta = x_isInversed ? xStart - xEnd : xEnd - xStart;
                    yDelta = y_isInversed ? yStart - yEnd : yEnd - yStart;
                    if (fastSeries.IsActualTransposed)
                    {
                        ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                        xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                        yOffset = cartesianTransformer.YAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
                        xOffset = cartesianTransformer.XAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;
                    }
                    else
                    {
                        ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                        xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                        yOffset = cartesianTransformer.YAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;
                        xOffset = cartesianTransformer.XAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
                    }
                    xTolerance = Math.Abs((xDelta * 1) / xSize);
                    yTolerance = Math.Abs((yDelta * 1) / ySize);
                   
                    if (x_isInversed)
                    {
                        double temp = xStart;
                        xStart = xEnd;
                        xEnd = temp;
                    }

                    if (y_isInversed)
                    {
                        double temp = yStart;
                        yStart = yEnd;
                        yEnd = temp;
                    }
                    if (!isLogarithmic)
                    {
                        TransformToScreenCo();
                    }
                    else
                    {
                        TransformToScreenCoInLog(cartesianTransformer);
                    }
                }
                UpdateVisual(true);
            }
        }

        #endregion

        #region Internal Methods

        internal void SetRange()
        {
            double X_MAX = 0;
            double Y_MAX = 0;
            double X_MIN = 0;
            double Y_MIN = 0;
            var isGrouping = (fastSeries.ActualXAxis is CategoryAxis) ? (fastSeries.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (fastSeries.DataCount > 0)
            {
                if (fastSeries.IsIndexed)
                {
                    if (!isGrouping)
                        X_MAX = Series.GroupedXValues.Count - 1;
                    else
                        X_MAX = fastSeries.DataCount - 1;
                    Y_MAX = yChartVals.Max();
                    X_MIN = 0;
                    Y_MIN = yChartVals.Min();
                }
                else
                {
                    X_MAX = xChartVals.Max();
                    Y_MAX = yChartVals.Max();
                    X_MIN = xChartVals.Min();
                    Y_MIN = yChartVals.Min();
                }
                XRange = new DoubleRange(X_MIN, X_MAX);
                YRange = new DoubleRange(Y_MIN, Y_MAX);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        internal void UpdateSegment(int index, NotifyCollectionChangedAction action, IChartTransformer transformer)
        {
            if (action == NotifyCollectionChangedAction.Remove)
            {
                this.Points.RemoveAt(index);
            }
            else if (action == NotifyCollectionChangedAction.Add)
            {
                Point point = transformer.TransformToVisible(xChartVals[index], yChartVals[index]);
                Points.Add(point);
                UpdateVisual(false);
            }
        }

        internal void UpdateVisual(bool updatePolyline)
        {
            if (updatePolyline)
            {
                if (polyline != null)
                {
                    if (segmentUpdated)
                        Series.SeriesRootPanel.Clip = null;
                    polyline.Points = Points;
                    segmentUpdated = true;
                }

            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method Implementation for set Binding to ChartSegments properties.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
            if (Series is FastLineSeries)
            {
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeDashArray");
                BindingOperations.SetBinding(this, FastLineSegment.StrokeDashArrayProperty, binding);
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeDashOffset");
                element.SetBinding(Shape.StrokeDashOffsetProperty, binding);
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeDashCap");
                element.SetBinding(Shape.StrokeDashCapProperty, binding);
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeLineJoin");
                element.SetBinding(Shape.StrokeLineJoinProperty, binding);
            }
        }

        #endregion

        #region Private Methods

        private void CalculatePonts(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            int cnt = xChartVals.Count - 1;
            Points = new PointCollection();
            for (int i = 0; i <= cnt; i++)
            {
                double xVal = xChartVals[i];
                double yVal = yChartVals[i];
                Point point = cartesianTransformer.TransformToVisible(xVal, yVal);
                Points.Add(point);
            }

        }
        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCo()
        {
            if (fastSeries.IsActualTransposed)
            {
                TransformToScreenCoVertical();
            }
            else
            {
                TransformToScreenCoHorizontal();
            }
        }

        private void TransformToScreenCoHorizontal()
        {
            int i = 0;
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            double yCoefficient = 0;
            xValues = (Series.ActualXValues is List<double> && !Series.IsIndexed) ? Series.ActualXValues as List<double> : Series.GetXValues();
            var isGrouping = (fastSeries.ActualXAxis is CategoryAxis) ? (fastSeries.ActualXAxis as CategoryAxis).IsIndexed : true;
            var numericalAxis = fastSeries.ActualYAxis as NumericalAxis;
            bool isScaleBreak = numericalAxis != null && numericalAxis.AxisRanges != null && numericalAxis.AxisRanges.Count > 0;
            
            int startIndex = 0;
            prevXValue = fastSeries.IsIndexed ? 1 : xChartVals[0];
            int cnt = xChartVals.Count - 1;

            if (fastSeries.isLinearData)
            {
                for (i = 1; i < cnt; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];

                    if ((xVal <= xEnd) && (xVal >= xStart))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            yCoefficient = 1 - (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yVal) : (yVal - yStart) / yDelta);
                            Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * yCoefficient)));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
                    else if (xVal < xStart)
                    {
                        if (x_isInversed)
                        {
                            yCoefficient = 1 - (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yVal) : (yVal - yStart) / yDelta);
                            Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * yCoefficient)));
                        }
                        else
                            startIndex = i;
                    }
                    else if (xVal > xEnd)
                    {
                        double yValue = ChartMath.GetInterpolarationPoint(xChartVals[i - 1], xChartVals[i], yChartVals[i - 1], yChartVals[i], xEnd);
                        yCoefficient = 1 - (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yValue) : (yValue - yStart) / yDelta);
                        Points.Add(new Point((xOffset + xSize), (yOffset + ySize * yCoefficient)));
                        break;
                    }
                }
            }
            else
            {
                for (i = 1; i < cnt; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];
                    if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        yCoefficient = 1 - (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yVal) : (yVal - yStart) / yDelta);
                        Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * yCoefficient)));
                        prevXValue = xVal;
                        prevYValue = yVal;
                    }
                }
            }
            if (startIndex > 0)
            {
                double yValue = ChartMath.GetInterpolarationPoint(xChartVals[startIndex], xChartVals[startIndex + 1], yChartVals[startIndex], yChartVals[startIndex + 1], xStart);
                yCoefficient = 1 - (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yValue) : (yValue - yStart) / yDelta);
                Points.Insert(0, new Point((xOffset), (yOffset + ySize * yCoefficient)));
            }
            else
            {
                yCoefficient = 1 - (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yChartVals[startIndex]) : (yChartVals[startIndex] - yStart) / yDelta);
                Points.Insert(0, new Point((xOffset + xSize * ((xChartVals[startIndex] - xStart) / xDelta)), (yOffset + ySize * yCoefficient)));
            }
            if (i == cnt)
            {
                yCoefficient = 1 - (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yChartVals[cnt]) : (yChartVals[cnt] - yStart) / yDelta);
                Points.Add(new Point((xOffset + xSize * ((xChartVals[cnt] - xStart) / xDelta)), (yOffset + ySize * yCoefficient)));
            }
        }

        private void TransformToScreenCoVertical()
        {
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            int i = 0;
            double yCoefficient = 0;
            var numericalAxis = fastSeries.ActualYAxis as NumericalAxis;
            bool isScaleBreak = numericalAxis != null && numericalAxis.AxisRanges != null && numericalAxis.AxisRanges.Count > 0;

            xValues = (Series.ActualXValues is List<double> && !Series.IsIndexed) ? Series.ActualXValues as List<double> : Series.GetXValues();
            var isGrouping = (fastSeries.ActualXAxis is CategoryAxis) ? (fastSeries.ActualXAxis as CategoryAxis).IsIndexed : true;

            int startIndex = 0;
            prevXValue = fastSeries.IsIndexed ? 1 : xChartVals[0];
            prevYValue = yChartVals[0];
            int cnt = xChartVals.Count - 1;

            if (fastSeries.isLinearData)
            {
                for (i = 1; i < cnt; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];

                    if ((xVal <= xEnd) && (xVal >= xStart))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            yCoefficient = (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yVal) : 1 - ((yEnd - yVal) / yDelta));
                            Points.Add(new Point((yOffset + ySize * yCoefficient), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
                    else if (xVal < xStart)
                    {
                        if (x_isInversed)
                        {
                            yCoefficient = (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yVal) : 1 - ((yEnd - yVal) / yDelta));
                            Points.Add(new Point((yOffset + ySize * yCoefficient), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        }
                        else
                            startIndex = i;
                    }
                    else if (xVal > xEnd)
                    {
                        double yValue = ChartMath.GetInterpolarationPoint(xChartVals[i - 1], xChartVals[i], yChartVals[i - 1], yChartVals[i], xEnd);
                        yCoefficient = (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yValue) : 1 - ((yEnd - yValue) / yDelta));
                        Points.Add(new Point((yOffset + ySize * yCoefficient), (xOffset)));
                        break;
                    }
                }
            }
            else
            {
                for (i = 1; i < cnt; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];
                    if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        yCoefficient = (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yVal) : 1 - ((yEnd - yVal) / yDelta));
                        Points.Add(new Point((yOffset + ySize * yCoefficient), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        prevXValue = xVal;
                        prevYValue = yVal;
                    }
                }
            }

            if (startIndex > 0)
            {
                double yValue = ChartMath.GetInterpolarationPoint(xChartVals[startIndex], xChartVals[startIndex + 1], yChartVals[startIndex], yChartVals[startIndex + 1], xStart);
                yCoefficient = (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yValue) : 1 - ((yEnd - yValue) / yDelta));
                Points.Insert(0, new Point((yOffset + ySize * yCoefficient), (xOffset + xSize * ((xEnd - xStart) / xDelta))));
            }
            else
            {
                yCoefficient = (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yChartVals[startIndex]) : 1 - ((yEnd - yChartVals[startIndex]) / yDelta));
                Points.Insert(0, new Point((yOffset + ySize * yCoefficient), (xOffset + xSize * ((xEnd - xChartVals[startIndex]) / xDelta))));
            }

            if (i == cnt)
            {
                yCoefficient = (isScaleBreak ? numericalAxis.CalculateValueToCoefficient(yChartVals[cnt]) : 1 - ((yEnd - yChartVals[cnt]) / yDelta));
                Points.Add(new Point((yOffset + ySize * yCoefficient), (xOffset + xSize * ((xEnd - xChartVals[cnt]) / xDelta))));
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;
            if (!fastSeries.IsActualTransposed)
            {
                TransformToScreenCoInLogHorizontal(xBase, yBase);
            }
            else
            {
                TransformToScreenCoInLogVertical(xBase, yBase);
            }
        }
        
        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            int startIndex = 0;
            int i = 0;

            double xVal, yVal;
            prevXValue = fastSeries.IsIndexed ? 1 : xChartVals[0];
            prevYValue = yChartVals[0];
            int cnt = xChartVals.Count - 1;

            if (fastSeries.isLinearData)
            {
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 || xChartVals[i] <= 0 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 || yChartVals[i] <= 0 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if ((xVal <= xEnd) && (xVal >= xStart))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)),
                                (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
                    else if (xVal < xStart)
                    {
                        if (x_isInversed)
                            Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                        else
                            startIndex = i;
                    }
                    else if (xVal > xEnd)
                    {
                        Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)),
                            (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                        break;
                    }
                }
            }

            else
            {
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 || xChartVals[i] <= 0 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 || yChartVals[i] <= 0 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)),
                            (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                        prevXValue = xVal;
                        prevYValue = yVal;
                    }
                }
            }

            xVal = xBase == 1 || xChartVals[startIndex] <= 0 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
            yVal = yBase == 1 || yChartVals[startIndex] <= 0 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
            Points.Insert(0, new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));

            if (i == cnt)
            {
                xVal = xBase == 1 || xChartVals[cnt] <= 0 ? xChartVals[cnt] : Math.Log(xChartVals[cnt], xBase);
                yVal = yBase == 1 || yChartVals[cnt] <= 0 ? yChartVals[cnt] : Math.Log(yChartVals[cnt], yBase);
                Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
            }
        }

        private void TransformToScreenCoInLogVertical(double xBase, double yBase)
        {
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            int startIndex = 0;
            int i = 0;

            double xVal, yVal;
            prevXValue = fastSeries.IsIndexed ? 1 : xChartVals[0];
            prevYValue = yChartVals[0];
            int cnt = xChartVals.Count - 1;

            if (fastSeries.isLinearData)
            {
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 || xChartVals[i] <= 0 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 || yChartVals[i] <= 0 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if ((xVal <= xEnd) && (xVal >= xStart))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
                    else if (xVal < xStart)
                    {
                        if (x_isInversed)
                            Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        else
                            startIndex = i;
                    }
                    else if (xVal > xEnd)
                    {
                        Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        break;
                    }
                }
            }
            else
            {
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 || xChartVals[i] <= 0 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 || yChartVals[i] <= 0 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        prevXValue = xVal;
                        prevYValue = yVal;
                    }
                }
            }

            xVal = xBase == 1 || xChartVals[startIndex] <= 0 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
            yVal = yBase == 1 || yChartVals[startIndex] <= 0 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
            Points.Insert(0, new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
            if (i == cnt)
            {
                xVal = xBase == 1 || xChartVals[cnt] <= 0 ? xChartVals[cnt] : Math.Log(xChartVals[cnt], xBase);
                yVal = yBase == 1 || yChartVals[cnt] <= 0 ? yChartVals[cnt] : Math.Log(yChartVals[cnt], yBase);
                Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
            }
        }

        #endregion

        internal override void Dispose()
        {
            if(polyline != null)
            {
                polyline.Tag = null;
                polyline = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
