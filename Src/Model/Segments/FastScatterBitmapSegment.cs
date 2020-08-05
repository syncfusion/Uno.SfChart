using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart fast scatter bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastScatterBitmapSeries"/>
    public partial class FastScatterBitmapSegment : ChartSegment
    {
        #region Fields

        #region Internal Fields

        internal FastScatterBitmapSeries fastSeries;

        #endregion

        #region Private Fields

        private IList<double> xChartVals, yChartVals;

        List<double> xValues;

        List<double> yValues;

        int startIndex = 0;

        private WriteableBitmap bitmap;

        Color seriesSelectionColor = Colors.Transparent;

        Color segmentSelectionColor = Colors.Transparent;

        bool isSeriesSelected;

        private byte[] fastBuffer;

        #endregion

        #endregion
        
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public FastScatterBitmapSegment()
        {

        }

        public FastScatterBitmapSegment(IList<double> xVals, IList<double> yVals, FastScatterBitmapSeries series)
        {
            xValues = new List<double>();
            yValues = new List<double>();
            base.Series = series;
            fastSeries = series;
            base.Item = series.ActualData;
            this.xChartVals = xVals;
            this.yChartVals = yVals;

            SetRange();
        }

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
        /// `Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>retuns UIElement</returns>
        public override UIElement CreateVisual(Size size)
        {
            return null;
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
        /// <returns></returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer"></param>
        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastSeries.DataCount > 0)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                xValues.Clear();
                yValues.Clear();
                //Removed the existing screen point calculation methods and added the TransformVisible method.
                CalculatePoints(cartesianTransformer);
                UpdateVisual();
            }
        }

        #endregion

        #region Internal Methods

        internal void SetRange()
        {
            if (fastSeries.DataCount > 0)
            {
                double _Min = yChartVals.Min();
                double Y_MIN;
                if (double.IsNaN(_Min))
                {
                    var yVal = yChartVals.Where(e => !double.IsNaN(e));
                    Y_MIN = (!yVal.Any()) ? 0 : yVal.Min();
                }
                else
                {
                    Y_MIN = _Min;
                }
                if (fastSeries.IsIndexed)
                {
                    var isGrouping = (fastSeries.ActualXAxis is CategoryAxis) ? (fastSeries.ActualXAxis as CategoryAxis).IsIndexed : true;
                    double X_MAX = 0;
                    if (!isGrouping)
                        X_MAX = xChartVals.Max();
                    else
                        X_MAX = fastSeries.DataCount - 1;
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = 0;

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
                else
                {
                    double X_MAX = xChartVals.Max();
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = xChartVals.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
            }
        }

        internal void UpdateVisual()
        {
            double xr = fastSeries.ScatterWidth, yr = fastSeries.ScatterHeight;
            bool isMultiColor = Series.Palette != ChartColorPalette.None && fastSeries.Interior == null;
            Color color = GetColor(Interior);
            if (bitmap != null && xValues.Count > 0)
            {
                fastBuffer = fastSeries.Area.GetFastBuffer();
                int width = (int)fastSeries.Area.SeriesClipRect.Width;
                int height = (int)fastSeries.Area.SeriesClipRect.Height;

                if (fastSeries is FastScatterBitmapSeries)
                {
                    isSeriesSelected = false;
                    SfChart chart = fastSeries.Area;

                    //Set SeriesSelectionBrush and Check EnableSeriesSelection        
                    if (chart.GetEnableSeriesSelection())
                    {
                        Brush seriesSelectionBrush = chart.GetSeriesSelectionBrush(fastSeries);
                        if (seriesSelectionBrush != null && chart.SelectedSeriesCollection.Contains(fastSeries))
                        {
                            isSeriesSelected = true;
                            seriesSelectionColor = ((SolidColorBrush)seriesSelectionBrush).Color;
                        }
                    }
                    else if (chart.GetEnableSegmentSelection())//Set SegmentSelectionBrush and Check EnableSegmentSelection
                    {
                        Brush segmentSelectionBrush = (fastSeries as ISegmentSelectable).SegmentSelectionBrush;
                        if (segmentSelectionBrush != null)
                        {
                            segmentSelectionColor = ((SolidColorBrush)segmentSelectionBrush).Color;
                        }
                    }

                    DrawScatterType(fastSeries.ShapeType, width, height, color, isMultiColor, xr, yr);
                }
            }

            fastSeries.Area.CanRenderToBuffer = true;

            xValues.Clear();
            yValues.Clear();
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method Implementation for set Binding to ChartSegments properties.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {

        }

        #endregion

        #region Private Methods

        private Color GetSegmentInterior(Color color, bool isMultiColor)
        {
            if (isSeriesSelected)
            {
                color = seriesSelectionColor;
            }
            else if (fastSeries.SelectedSegmentsIndexes.Contains(startIndex) && (fastSeries as ISegmentSelectable).SegmentSelectionBrush != null)
            {
                color = segmentSelectionColor;
            }
            else if (Series.SegmentColorPath != null && fastSeries.Interior == null)
            {
                if (Series.ColorValues.Count > 0 && Series.ColorValues[startIndex] != null)
                    color = GetColor(Series.ColorValues[startIndex]);
                else if (Series.Palette == ChartColorPalette.None)
                {
                    int serIndex = Series.ActualArea.GetSeriesIndex(this.Series);
                    color = GetColor(Series.ActualArea.ColorModel.GetBrush(serIndex));
                }
                else
                {
                    Brush brush = Series.ColorModel.GetBrush(startIndex);
                    color = GetColor(brush);
                }
            }
            else if (isMultiColor)
            {
                Brush brush = Series.ColorModel.GetBrush(startIndex);
                color = GetColor(brush);
            }

            return color;
        }

        private void DrawScatterType(ChartSymbol shapeType, int width, int height, Color color, bool isMultiColor, double xr, double yr)
        {
            double xValue = 0, yValue = 0;

            for (int i = 0; i < xValues.Count; i++)
            {
                xValue = xValues[i];
                yValue = yValues[i];
                Color segmentColor = GetSegmentInterior(color, isMultiColor);
                startIndex++;

                if (yValue > -1)
                {
                    switch (shapeType)
                    {
                        case ChartSymbol.Ellipse:
                            DrawEllipse(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.Square:
                            DrawRectangle(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.Cross:
                            DrawCross(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.Diamond:
                            DrawDiamond(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.Hexagon:
                            DrawHexagon(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.InvertedTriangle:
                            DrawInvertedTriangle(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.Pentagon:
                            DrawPentagon(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.Plus:
                            DrawPlus(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                        case ChartSymbol.Triangle:
                            DrawTriangle(width, height, segmentColor, xValue, yValue, xr, yr);
                            break;
                    }
                }
            }
        }

        private void DrawEllipse(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillEllipseCentered(fastBuffer, height, width, (int)yValue, (int)xValue, (int)scatterWidth, (int)scatterHeight, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillEllipseCentered(fastBuffer, height, width, (int)xValue, (int)yValue, (int)scatterWidth, (int)scatterHeight, color, fastSeries.bitmapPixels);
            }
        }

        private void DrawRectangle(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue - scatterWidth / 2;
            double y1 = yValue - scatterHeight / 2;
            double x2 = xValue + scatterWidth / 2;
            double y2 = yValue + scatterHeight / 2;

            if (y1 < y2)
            {
                if (fastSeries.IsActualTransposed)
                {
                    bitmap.FillRectangle(fastBuffer, width, height, (int)y1, (int)x1, (int)y2, (int)x2, color, fastSeries.bitmapPixels);
                    Series.bitmapRects.Add(new Rect(new Point(y1, x1), new Point(y2, x2)));
                }
                else
                {
                    bitmap.FillRectangle(fastBuffer, width, height, (int)x1, (int)y1, (int)x2, (int)y2, color, fastSeries.bitmapPixels);
                    Series.bitmapRects.Add(new Rect(new Point(x1, y1), new Point(x2, y2)));
                }
            }
            else
            {
                if (fastSeries.IsActualTransposed)
                {
                    bitmap.FillRectangle(fastBuffer, width, height, (int)y2, (int)x1, (int)y1, (int)x2, color, fastSeries.bitmapPixels);
                    Series.bitmapRects.Add(new Rect(new Point(y2, x1), new Point(y1, x2)));
                }
                else
                {
                    bitmap.FillRectangle(fastBuffer, width, height, (int)x1, (int)y2, (int)x2, (int)y1, color, fastSeries.bitmapPixels);
                    Series.bitmapRects.Add(new Rect(new Point(x1, y2), new Point(x2, y1)));
                }
            }
        }

        private void DrawTriangle(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue - scatterWidth / 2;
            double y1 = !fastSeries.IsActualTransposed ? yValue + scatterHeight / 2 : yValue - scatterHeight / 2;
            double x2 = xValue + scatterWidth / 2;
            double y2 = !fastSeries.IsActualTransposed ? yValue + scatterHeight / 2 : yValue - scatterHeight / 2;
            double x3 = xValue;
            double y3 = !fastSeries.IsActualTransposed ? yValue - scatterHeight / 2 : yValue + scatterHeight / 2;

            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)y1, (int)x1, (int)y2, (int)x2, (int)y3, (int)x3, (int)y1, (int)x1 }, width, height, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (int)x1, (int)y1 }, width, height, color, fastSeries.bitmapPixels);
            }
        }

        private void DrawInvertedTriangle(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue - scatterWidth / 2;
            double y1 = !fastSeries.IsActualTransposed ? yValue - scatterHeight / 2 : yValue + scatterHeight / 2;
            double x2 = xValue + scatterWidth / 2;
            double y2 = !fastSeries.IsActualTransposed ? yValue - scatterHeight / 2 : yValue + scatterHeight / 2;
            double x3 = xValue;
            double y3 = !fastSeries.IsActualTransposed ? yValue + scatterHeight / 2 : yValue - scatterHeight / 2;

            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)y1, (int)x1, (int)y2, (int)x2, (int)y3, (int)x3, (int)y1, (int)x1 }, width, height, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (int)x1, (int)y1 }, width, height, color, fastSeries.bitmapPixels);
            }
        }

        private void DrawDiamond(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue - scatterWidth / 2;
            double y1 = yValue;
            double x2 = xValue;
            double y2 = yValue - scatterHeight / 2;
            double x3 = xValue + scatterWidth / 2;
            double y3 = yValue;
            double x4 = xValue;
            double y4 = yValue + scatterHeight / 2;

            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)y1, (int)x1, (int)y2, (int)x2, (int)y3, (int)x3, (int)y4, (int)x4, (int)y1, (int)x1 }, width, height, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (int)x4, (int)y4,(int)x1, (int)y1 }, width, height, color, fastSeries.bitmapPixels);
            }
        }

        private void DrawHexagon(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue - scatterWidth / 2;
            double y1 = yValue;
            double x2 = xValue - scatterWidth / 4;
            double y2 = yValue - scatterHeight / 2;
            double x3 = xValue + scatterWidth / 4;
            double y3 = yValue - scatterHeight / 2;
            double x4 = xValue + scatterWidth / 2;
            double y4 = yValue;
            double x5 = xValue + scatterWidth / 4;
            double y5 = yValue + scatterHeight / 2;
            double x6 = xValue - scatterWidth / 4;
            double y6 = yValue + scatterHeight / 2;

            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)y1, (int)x1, (int)y2, (int)x2, (int)y3, (int)x3, (int)y4, (int)x4, (int)y5, (int)x5, (int)y6, (int)x6, (int)y1, (int)x1 }, width, height, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (int)x4, (int)y4, (int)x5, (int)y5, (int)x6, (int)y6, (int)x1, (int)y1 }, width, height, color, fastSeries.bitmapPixels);
            }
        }

        private void DrawPentagon(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue - scatterWidth / 4;
            double y1 = !fastSeries.IsActualTransposed ? yValue + scatterHeight / 2 : yValue - scatterHeight / 2;
            double x2 = xValue - scatterWidth / 2;
            double y2 = !fastSeries.IsActualTransposed ? yValue - scatterHeight / 8 : yValue + scatterHeight / 8;
            double x3 = xValue;
            double y3 = !fastSeries.IsActualTransposed ? yValue - scatterHeight / 2 : yValue + scatterHeight / 2;
            double x4 = xValue + scatterWidth / 2;
            double y4 = !fastSeries.IsActualTransposed ? yValue - scatterHeight / 8 : yValue + scatterHeight / 8;
            double x5 = xValue + scatterWidth / 4;
            double y5 = !fastSeries.IsActualTransposed ? yValue + scatterHeight / 2 : yValue - scatterHeight / 2;

            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)y1, (int)x1, (int)y2, (int)x2, (int)y3, (int)x3, (int)y4, (int)x4, (int)y5, (int)x5, (int)y1, (int)x1 }, width, height, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (int)x4, (int)y4, (int)x5, (int)y5, (int)x1, (int)y1 }, width, height, color, fastSeries.bitmapPixels);
            }
        }

        private void DrawPlus(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue - scatterWidth / 8;
            double y1 = yValue + scatterHeight / 8;
            double x2 = xValue - scatterWidth / 2;
            double y2 = yValue + scatterHeight / 8;
            double x3 = xValue - scatterWidth / 2;
            double y3 = yValue - scatterHeight / 8;
            double x4 = xValue - scatterWidth / 8;
            double y4 = yValue - scatterHeight / 8;
            double x5 = xValue - scatterWidth / 8;
            double y5 = yValue - scatterHeight / 2;
            double x6 = xValue + scatterWidth / 8;
            double y6 = yValue - scatterHeight / 2;
            double x7 = xValue + scatterWidth / 8;
            double y7 = yValue - scatterHeight / 8;
            double x8 = xValue + scatterWidth / 2;
            double y8 = yValue - scatterHeight / 8;
            double x9 = xValue + scatterWidth / 2;
            double y9 = yValue + scatterHeight / 8;
            double x10 = xValue + scatterWidth / 8;
            double y10 = yValue + scatterHeight / 8;
            double x11 = xValue + scatterWidth / 8;
            double y11 = yValue + scatterHeight / 2;
            double x12 = xValue - scatterWidth / 8;
            double y12 = yValue + scatterHeight / 2;

            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)y1, (int)x1, (int)y2, (int)x2, (int)y3, (int)x3, (int)y4, (int)x4, (int)y5, (int)x5, (int)y6, (int)x6, (int)y7, (int)x7, (int)y8, (int)x8, (int)y9, (int)x9, (int)y10, (int)x10, (int)y11, (int)x11, (int)y12, (int)x12, (int)y1, (int)x1 }, width, height, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (int)x4, (int)y4, (int)x5, (int)y5, (int)x6, (int)y6, (int)x7, (int)y7, (int)x8, (int)y8, (int)x9, (int)y9, (int)x10, (int)y10, (int)x11, (int)y11, (int)x12, (int)y12, (int)x1, (int)y1 }, width, height, color, fastSeries.bitmapPixels);
            }
        }

        private void DrawCross(int width, int height, Color color, double xValue, double yValue, double scatterWidth, double scatterHeight)
        {
            double x1 = xValue;
            double y1 = yValue + scatterHeight / 4;
            double x2 = xValue - scatterWidth / 4;
            double y2 = yValue + scatterHeight / 2;
            double x3 = xValue - scatterWidth / 2;
            double y3 = yValue + scatterHeight / 4;
            double x4 = xValue - scatterWidth / 4;
            double y4 = yValue;
            double x5 = xValue - scatterWidth / 2;
            double y5 = yValue - scatterHeight / 4;
            double x6 = xValue - scatterWidth / 4;
            double y6 = yValue - scatterHeight / 2;
            double x7 = xValue;
            double y7 = yValue - scatterHeight / 4;
            double x8 = xValue + scatterWidth / 4;
            double y8 = yValue - scatterHeight / 2;
            double x9 = xValue + scatterWidth / 2;
            double y9 = yValue - scatterHeight / 4;
            double x10 = xValue + scatterWidth / 4;
            double y10 = yValue;
            double x11 = xValue + scatterWidth / 2;
            double y11 = yValue + scatterHeight / 4;
            double x12 = xValue + scatterWidth / 4;
            double y12 = yValue + scatterHeight / 2;

            if (fastSeries.IsActualTransposed)
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)y1, (int)x1, (int)y2, (int)x2, (int)y3, (int)x3, (int)y4, (int)x4, (int)y5, (int)x5, (int)y6, (int)x6, (int)y7, (int)x7, (int)y8, (int)x8, (int)y9, (int)x9, (int)y10, (int)x10, (int)y11, (int)x11, (int)y12, (int)x12, (int)y1, (int)x1 }, width, height, color, fastSeries.bitmapPixels);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, new int[] { (int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (int)x4, (int)y4, (int)x5, (int)y5, (int)x6, (int)y6, (int)x7, (int)y7, (int)x8, (int)y8, (int)x9, (int)y9, (int)x10, (int)y10, (int)x11, (int)y11, (int)x12, (int)y12, (int)x1, (int)y1 }, width, height, color, fastSeries.bitmapPixels);
            }
        }

        private void CalculatePoints(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            var areaBorderThickness = (this.Series.ActualArea as SfChart).AreaBorderThickness;
            ChartAxis xAxis = cartesianTransformer.XAxis;
            if (fastSeries.IsIndexed)
            {
                var isGrouping = (fastSeries.ActualXAxis is CategoryAxis) &&
                                 (fastSeries.ActualXAxis as CategoryAxis).IsIndexed;
                int start = 0, end = 0;
                if (!isGrouping)
                {
                    start = 0;
                    end = xChartVals.Count - 1;
                }
                else
                {
                    start = (int)Math.Floor(xAxis.VisibleRange.Start);
                    end = (int)Math.Ceiling(xAxis.VisibleRange.End);
                    end = end > yChartVals.Count - 1 ? yChartVals.Count - 1 : end;
                }
                start = start < 0 ? 0 : start;
                startIndex = start;
                for (int i = start; i <= end; i++)
                {
                    AddDataPoint(cartesianTransformer, areaBorderThickness, i);
                }
            }
            else
            {
                if (fastSeries.isLinearData)
                {
                    int i = 0;
                    double start = xAxis.VisibleRange.Start;
                    double end = xAxis.VisibleRange.End;
                    startIndex = 0;
                    int count = xChartVals.Count - 1;
                    double xBase = cartesianTransformer.XAxis.IsLogarithmic ?
                        (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 0;
                    for (i = 1; i < count; i++)
                    {
                        double xValue = xChartVals[i];
                        if (cartesianTransformer.XAxis.IsLogarithmic)
                            xValue = Math.Log(xValue, xBase);
                        if (xValue >= start && xValue <= end)
                            AddDataPoint(cartesianTransformer, areaBorderThickness, i);
                        else if (xValue < start)
                            startIndex = i;
                        else if (xValue > end)
                        {
                            AddDataPoint(cartesianTransformer, areaBorderThickness, i);
                            break;
                        }
                    }
                    InsertDataPoint(cartesianTransformer, areaBorderThickness, startIndex);
                    if (i == count)
                        AddDataPoint(cartesianTransformer, areaBorderThickness, count);
                }
                else
                {
                    startIndex = 0;
                    for (int i = 0; i < this.Series.DataCount; i++)
                    {
                        AddDataPoint(cartesianTransformer, areaBorderThickness, i);
                    }
                }
            }
        }
        private void AddDataPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer,
            Thickness areaBorderThickness, int index)
        {
            double xValue, yValue;
            GetXYValue(index, out xValue, out yValue);
            Point point = cartesianTransformer.TransformToVisible(xValue, yValue);
            if (!fastSeries.IsTransposed)
            {
                xValues.Add(point.X - areaBorderThickness.Left);
                yValues.Add(point.Y - areaBorderThickness.Top);
            }
            else
            {
                xValues.Add(point.Y - areaBorderThickness.Left);
                yValues.Add(point.X - areaBorderThickness.Top);
            }
        }

        private void InsertDataPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer,
           Thickness areaBorderThickness, int index)
        {
            double xValue, yValue;
            GetXYValue(index, out xValue, out yValue);
            Point point = cartesianTransformer.TransformToVisible(xValue, yValue);
            if (!fastSeries.IsTransposed)
            {
                xValues.Insert(0, (point.X - areaBorderThickness.Left));
                yValues.Insert(0, (point.Y - areaBorderThickness.Top));
            }
            else
            {
                xValues.Insert(0, (point.Y - areaBorderThickness.Left));
                yValues.Insert(0, (point.X - areaBorderThickness.Top));
            }
        }

        private void GetXYValue(int index, out double xValue, out double yValue)
        {
            xValue = xChartVals[index];
            yValue = yChartVals[index];
        }

        #endregion

        #endregion
    }
}
