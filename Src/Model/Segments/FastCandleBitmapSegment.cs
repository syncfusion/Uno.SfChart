using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart fast candle bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WinRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSeries"/>    
    public partial class FastCandleBitmapSegment : ChartSegment
    {
        #region Fields

        #region Internal Fields

        internal ChartSeries fastCandleBitmapSeries;

        #endregion

        #region Private Fields

        private DoubleRange sbsInfo;

        private WriteableBitmap bitmap;
#if NETFX_CORE 
        private byte[] fastBuffer;
#endif
        private IList<double> xChartVals, openChartVals, closeChartVals, highChartVals, lowChartVals;

        double xStart, xEnd, yStart, yEnd, xSize, ySize, xOffset, yOffset;

        int count;

        Color seriesSelectionColor = Colors.Transparent;

        Color segmentSelectionColor = Colors.Transparent;

        bool isSeriesSelected;

        List<float> xValues;
        List<float> x1Values;
        List<float> x2Values;
        List<float> openValue;
        List<float> closeValue;
        List<float> highValue;
        List<float> highValue1;
        List<float> lowValue;
        List<float> lowValue1;
        List<bool> isBull;
        List<bool> isHollow;
        int startIndex = 0;

        #endregion

        #endregion

        #region Constructor

        public FastCandleBitmapSegment()
        {

        }

        public FastCandleBitmapSegment(ChartSeriesBase series)
        {
            xValues = new List<float>();
            x1Values = new List<float>();
            x2Values = new List<float>();
            openValue = new List<float>();
            closeValue = new List<float>();
            highValue = new List<float>();
            highValue1 = new List<float>();
            lowValue = new List<float>();
            lowValue1 = new List<float>();
            isBull = new List<bool>();
            isHollow = new List<bool>();
            fastCandleBitmapSeries = series as ChartSeries;
        }

        public FastCandleBitmapSegment(IList<double> xValues, IList<double> OpenValues, IList<double> CloseValues, IList<double> highValues, IList<double> LowValues, ChartSeriesBase series)
            : this(series)
        {

        }

        #endregion

        #region Methods

        #region Public Override Methods
        
        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public override UIElement CreateVisual(Size size)
        {
            return null;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="openValue"></param>
        /// <param name="closeValue"></param>
        /// <param name="highValue"></param>
        /// <param name="lowValue"></param>
        public override void SetData(IList<double> xValues, IList<double> openValue, IList<double> closeValue, IList<double> highValue, IList<double> lowValue)
        {
            sbsInfo = fastCandleBitmapSeries.GetSideBySideInfo(fastCandleBitmapSeries);
            this.xChartVals = xValues;
            this.openChartVals = openValue;
            this.closeChartVals = closeValue;
            this.highChartVals = highValue;
            this.lowChartVals = lowValue;

            double X_MAX = xValues.Max() + sbsInfo.End;
            double Y_MAX = highValue.Max();
            double X_MIN = xValues.Min() + sbsInfo.Start;

            double _Min = lowValue.Min();
            double Y_MIN;
            if (double.IsNaN(_Min))
            {
                var lowVal = lowValue.Where(e => !double.IsNaN(e));
                Y_MIN = (!lowVal.Any()) ? 0 : lowVal.Min();
            }
            else
            {
                Y_MIN = _Min;
            }

            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);

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
            bitmap = fastCandleBitmapSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastCandleBitmapSeries.DataCount > 0)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;

                count = (int)(Math.Ceiling(xEnd));
                count = (int)Math.Min(count, xChartVals.Count);

                x_isInversed = cartesianTransformer.XAxis.IsInversed;
                y_isInversed = cartesianTransformer.YAxis.IsInversed;
                xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                yStart = cartesianTransformer.YAxis.VisibleRange.Start;
                yEnd = cartesianTransformer.YAxis.VisibleRange.End;
                if (fastCandleBitmapSeries.IsActualTransposed)
                {
                    ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                    xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Left - fastCandleBitmapSeries.Area.SeriesClipRect.Left;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Top - fastCandleBitmapSeries.Area.SeriesClipRect.Top;
                }
                else
                {
                    ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                    xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Top - fastCandleBitmapSeries.Area.SeriesClipRect.Top;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Left - fastCandleBitmapSeries.Area.SeriesClipRect.Left;
                }

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
                isHollow.Clear();
                isBull.Clear();
                xValues.Clear();
                x1Values.Clear();
                x2Values.Clear();
                openValue.Clear();
                closeValue.Clear();
                highValue.Clear();
                highValue1.Clear();
                lowValue.Clear();
                lowValue1.Clear();
                //Removed the screen point calculation methods and added the point to value method.
                CalculatePoints(cartesianTransformer);
                UpdateVisual();
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        public override void OnSizeChanged(Size size)
        {
        }

        #endregion

        #region Internal Methods

        internal void UpdateVisual()
        {
            Color color = new Color();
            if (bitmap != null && xValues.Count != 0)
            {
                fastBuffer = fastCandleBitmapSeries.Area.GetFastBuffer();
                int width = (int)fastCandleBitmapSeries.Area.SeriesClipRect.Width;
                int height = (int)fastCandleBitmapSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastCandleBitmapSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastCandleBitmapSeries.StrokeThickness % 2 == 0.0
                    ? (fastCandleBitmapSeries.StrokeThickness / 2) : (fastCandleBitmapSeries.StrokeThickness / 2) + 1);

                if (fastCandleBitmapSeries is FastCandleBitmapSeries)
                {
                    SfChart chart = fastCandleBitmapSeries.Area;
                    isSeriesSelected = false;

                    //Set SeriesSelectionBrush and Check EnableSeriesSelection        
                    if (chart.GetEnableSeriesSelection())
                    {
                        Brush seriesSelectionBrush = chart.GetSeriesSelectionBrush(fastCandleBitmapSeries);
                        if (seriesSelectionBrush != null && chart.SelectedSeriesCollection.Contains(fastCandleBitmapSeries))
                        {
                            isSeriesSelected = true;
                            seriesSelectionColor = ((SolidColorBrush)seriesSelectionBrush).Color;
                        }
                    }
                    else if (chart.GetEnableSegmentSelection())//Set SegmentSelectionBrush and Check EnableSegmentSelection
                    {
                        Brush segmentSelectionBrush = (fastCandleBitmapSeries as ISegmentSelectable).SegmentSelectionBrush;
                        if (segmentSelectionBrush != null)
                        {
                            segmentSelectionColor = ((SolidColorBrush)segmentSelectionBrush).Color;
                        }
                    }

                    if (!fastCandleBitmapSeries.IsActualTransposed)
                        UpdateVisualHorizontal(width, height, color, leftThickness, rightThickness);
                    else
                        UpdateVisualVertical(width, height, color, leftThickness, rightThickness);
                }
            }
            fastCandleBitmapSeries.Area.CanRenderToBuffer = true;

            xValues.Clear();
            x1Values.Clear();
            x2Values.Clear();
            openValue.Clear();
            closeValue.Clear();
            highValue.Clear();
            lowValue.Clear();
        }

        /// <summary>
        /// Gets the segment color based on data point
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal Color GetSegmentBrush(int index)
        {
            Color color;
            if (isBull.Count > index && isBull[index])
                color = (fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor != null ? color = ((SolidColorBrush)(fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
            else
                color = (fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor != null ? color = ((SolidColorBrush)(fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor).Color : ((SolidColorBrush)this.Interior).Color;

            return color;
        }

        #endregion

        #region Private Methods

        private void CalculatePoints(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            IList<double> values = (Series as FastCandleBitmapSeries).GetComparisionModeValues();
            ChartAxis xAxis = cartesianTransformer.XAxis;
            if (fastCandleBitmapSeries.IsIndexed)
            {
                bool isGrouping = (fastCandleBitmapSeries.ActualXAxis is CategoryAxis) &&
                                  (fastCandleBitmapSeries.ActualXAxis as CategoryAxis).IsIndexed;
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
                    end = end > highChartVals.Count - 1 ? highChartVals.Count - 1 : end;
                }
                start = start < 0 ? 0 : start;
                startIndex = start;
                if (!fastCandleBitmapSeries.IsActualTransposed)
                {
                    for (int i = start; i <= end; i++)
                    {
                        AddHorizontalPoint(cartesianTransformer, i, values);
                    }
                }
                else
                {
                    double xBase = cartesianTransformer.XAxis.IsLogarithmic
                        ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase
                        : 1;
                    double yBase = cartesianTransformer.YAxis.IsLogarithmic
                        ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase
                        : 1;
                    for (int i = start; i <= end; i++)
                    {
                        AddVerticalPoint(cartesianTransformer, i, values, xBase, yBase);
                    }
                }
            }
            else
            {
                startIndex = 0;
                if (fastCandleBitmapSeries.isLinearData)
                {
                    double start = xAxis.VisibleRange.Start;
                    double end = xAxis.VisibleRange.End;
                    int count = xChartVals.Count - 1;
                    int i = 0;
                    if (!fastCandleBitmapSeries.IsActualTransposed)
                    {
                        double xBase = cartesianTransformer.XAxis.IsLogarithmic ?
                            (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 0;
                        for (i = 1; i < count; i++)
                        {
                            var xValue = xChartVals[i];
                            if (cartesianTransformer.XAxis.IsLogarithmic)
                                xValue = Math.Log(xValue, xBase);
                            if (xValue <= end && xValue >= start)
                                AddHorizontalPoint(cartesianTransformer, i, values);
                            else if (xValue < start)
                                startIndex = i;
                            else if (xValue > end)
                            {
                                AddHorizontalPoint(cartesianTransformer, i, values);
                                break;
                            }
                        }
                        InsertHorizontalPoint(cartesianTransformer, startIndex, values);
                        if (i == count)
                            AddHorizontalPoint(cartesianTransformer, i, values);
                    }
                    else
                    {
                        double xBase = cartesianTransformer.XAxis.IsLogarithmic
                            ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase
                            : 1;
                        double yBase = cartesianTransformer.YAxis.IsLogarithmic
                            ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase
                            : 1;
                        for (i = 1; i < count; i++)
                        {
                            var xValue = xChartVals[i];
                            if (cartesianTransformer.XAxis.IsLogarithmic)
                                xValue = Math.Log(xValue, xBase);
                            if (xValue <= end && xValue >= start)
                                AddVerticalPoint(cartesianTransformer, i, values, xBase, yBase);
                            else if (xValue < start)
                                startIndex = i;
                            else if (xValue > end)
                            {
                                AddVerticalPoint(cartesianTransformer, i, values, xBase, yBase);
                                break;
                            }
                        }
                        InsertVerticalPoint(cartesianTransformer, startIndex, values, xBase, yBase);
                        if (i == count)
                            AddVerticalPoint(cartesianTransformer, i, values, xBase, yBase);
                    }
                }
                else
                {
                    startIndex = 0;
                    for (int i = 0; i < this.Series.DataCount; i++)
                    {
                        if (!fastCandleBitmapSeries.IsActualTransposed)
                            AddHorizontalPoint(cartesianTransformer, i, values);
                        else
                        {
                            double xBase = cartesianTransformer.XAxis.IsLogarithmic
                                ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase
                                : 1;
                            double yBase = cartesianTransformer.YAxis.IsLogarithmic
                                ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase
                                : 1;
                            AddVerticalPoint(cartesianTransformer, i, values, xBase, yBase);
                        }
                    }
                }

            }
        }
        private void AddVerticalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index,
         IList<double> values, double xBase, double yBase)
        {
            double x1Val, x2Val, openVal, closeVal, yHiVal, yHiVal1, yLoVal, yLoVal1;
            double sbsCenter = sbsInfo.Median;
            double sbsStart = sbsInfo.Start;
            double sbsEnd = sbsInfo.End;
            double highValues = highChartVals[index];
            double lowValues = lowChartVals[index];
            double openValues = openChartVals[index];
            double closeValues = closeChartVals[index];
            double tempOpenVal;
            bool bullBearFlag = false;
            var alignedValues = AlignHiLoSegment(openChartVals[index], closeChartVals[index], highValues,
                lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            GetPoints(index, out x1Val, out x2Val, sbsStart, sbsEnd);
            openVal = y_isInversed
                ? closeChartVals[index]
                : openChartVals[index];
            closeVal = y_isInversed
                ? openChartVals[index]
                : closeChartVals[index];
            if (index == 0 || (Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = openValues < closeValues;
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Add(bullBearFlag);
            isHollow.Add(closeValues > openValues &&
                         !((Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None));
            x1Val = xBase == 1 ? x1Val : Math.Log(x1Val, xBase);
            x2Val = xBase == 1 ? x2Val : Math.Log(x2Val, xBase);
            openVal = yBase == 1 ? openVal : Math.Log(openVal, yBase);
            closeVal = yBase == 1 ? closeVal : Math.Log(closeVal, yBase);
            if (y_isInversed ? openVal > closeVal : openVal < closeVal)
            {
                tempOpenVal = openVal;
                openVal = closeVal;
                closeVal = tempOpenVal;
            }
            yHiVal = highValues;
            yLoVal = lowValues;
            if (openValues > closeValues)
            {
                yHiVal1 = openValues;
                yLoVal1 = closeValues;
            }
            else
            {
                yHiVal1 = closeValues;
                yLoVal1 = openValues;
            }
            Point point1, point2, point3, point4;
            if (fastCandleBitmapSeries.IsIndexed)
            {
                point1 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal);
                point2 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal);
                point3 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal1);
                point4 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal1);
            }
            else
            {
                point1 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yHiVal);
                point2 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yLoVal);
                point3 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal1);
                point4 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal1);
            }
            xValues.Add((float)point1.Y);
            x1Values.Add((float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val)));
            x2Values.Add((float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val)));
            openValue.Add((float)(yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(openVal))));
            closeValue.Add((float)(yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(closeVal))));
            highValue.Add((float)point1.X);
            lowValue.Add((float)point2.X);
            highValue1.Add((float)point3.X);
            lowValue1.Add((float)point4.X);
        }

        private void InsertVerticalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index,
        IList<double> values, double xBase, double yBase)
        {
            double x1Val, x2Val, openVal, closeVal, yHiVal, yHiVal1, yLoVal, yLoVal1;
            double sbsCenter = sbsInfo.Median;
            double sbsStart = sbsInfo.Start;
            double sbsEnd = sbsInfo.End;
            double highValues = highChartVals[index];
            double lowValues = lowChartVals[index];
            double openValues = openChartVals[index];
            double closeValues = closeChartVals[index];
            double tempOpenVal;
            bool bullBearFlag = false;
            var alignedValues = AlignHiLoSegment(openChartVals[index], closeChartVals[index], highValues,
                lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            GetPoints(index, out x1Val, out x2Val, sbsStart, sbsEnd);
            openVal = y_isInversed
                ? closeChartVals[index]
                : openChartVals[index];
            closeVal = y_isInversed
                ? openChartVals[index]
                : closeChartVals[index];
            if (index == 0 || (Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = openValues < closeValues;
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Insert(0, bullBearFlag);
            isHollow.Insert(0, closeValues > openValues &&
                         !((Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None));
            x1Val = xBase == 1 ? x1Val : Math.Log(x1Val, xBase);
            x2Val = xBase == 1 ? x2Val : Math.Log(x2Val, xBase);
            openVal = yBase == 1 ? openVal : Math.Log(openVal, yBase);
            closeVal = yBase == 1 ? closeVal : Math.Log(closeVal, yBase);
            if (y_isInversed ? openVal > closeVal : openVal < closeVal)
            {
                tempOpenVal = openVal;
                openVal = closeVal;
                closeVal = tempOpenVal;
            }
            yHiVal = highValues;
            yLoVal = lowValues;
            if (openValues > closeValues)
            {
                yHiVal1 = openValues;
                yLoVal1 = closeValues;
            }
            else
            {
                yHiVal1 = closeValues;
                yLoVal1 = openValues;
            }
            Point point1 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yHiVal);
            Point point2 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yLoVal);
            Point point3 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal1);
            Point point4 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal1);
            xValues.Insert(0, (float)point1.Y);
            x1Values.Insert(0,
                (float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val)));
            x2Values.Insert(0,
                (float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val)));
            openValue.Insert(0,
                (float)
                (yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(openVal))));
            closeValue.Insert(0,
                (float)
                (yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(closeVal))));
            highValue.Insert(0, ((float)point1.X));
            lowValue.Insert(0, (float)point2.X);
            highValue1.Insert(0, (float)point3.X);
            lowValue1.Insert(0, (float)point4.X);
        }

        private void AddHorizontalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index, IList<double> values)
        {
            double x1Val, x2Val, openVal, closeVal, yHiVal, yHiVal1, yLoVal, yLoVal1;
            double sbsCenter = sbsInfo.Median;
            double sbsStart = sbsInfo.Start;
            double sbsEnd = sbsInfo.End;
            double highValues = highChartVals[index];
            double lowValues = lowChartVals[index];
            double openValues = openChartVals[index];
            double closeValues = closeChartVals[index];
            double tempOpenVal;
            bool bullBearFlag = false;
            var alignedValues = AlignHiLoSegment(openChartVals[index], closeChartVals[index], highValues,
                lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            GetPoints(index, out x1Val, out x2Val, sbsStart, sbsEnd);
            openVal = y_isInversed
            ? closeChartVals[index]
            : openChartVals[index];
            closeVal = y_isInversed
                ? openChartVals[index]
                : closeChartVals[index];
            if (index == 0 || (Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = openValues < closeValues;
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Add(bullBearFlag);
            isHollow.Add(closeValues > openValues &&
                         !((Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None));
            if (y_isInversed ? openVal > closeVal : openVal < closeVal)
            {
                tempOpenVal = openVal;
                openVal = closeVal;
                closeVal = tempOpenVal;
            }
            yHiVal = highValues;
            yLoVal = lowValues;
            if (openValues > closeValues)
            {
                yHiVal1 = openValues;
                yLoVal1 = closeValues;
            }
            else
            {
                yHiVal1 = closeValues;
                yLoVal1 = openValues;
            }
            Point blpoint = cartesianTransformer.TransformToVisible(x1Val, openVal);
            Point trpoint = cartesianTransformer.TransformToVisible(x2Val, closeVal);
            Point point1, point2, point3, point4;
            if (fastCandleBitmapSeries.IsIndexed)
            {
                point1 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal);
                point2 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal);
            }
            else
            {
                point1 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yHiVal);
                point2 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yLoVal);
            }
            point3 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal1);
            point4 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal1);
            xValues.Add((float)point1.X);
            x1Values.Add((float)blpoint.X);
            x2Values.Add((float)trpoint.X);
            openValue.Add((float)blpoint.Y);
            closeValue.Add((float)trpoint.Y);
            highValue.Add((float)point1.Y);
            lowValue.Add((float)point2.Y);
            highValue1.Add((float)point3.Y);
            lowValue1.Add((float)point4.Y);
        }
        private void InsertHorizontalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index, IList<double> values)
        {
            double x1Val, x2Val, openVal, closeVal, yHiVal, yHiVal1, yLoVal, yLoVal1;
            double sbsCenter = sbsInfo.Median;
            double sbsStart = sbsInfo.Start;
            double sbsEnd = sbsInfo.End;
            double highValues = highChartVals[index];
            double lowValues = lowChartVals[index];
            double openValues = openChartVals[index];
            double closeValues = closeChartVals[index];
            double tempOpenVal;
            bool bullBearFlag = false;
            var alignedValues = AlignHiLoSegment(openChartVals[index], closeChartVals[index], highValues,
                lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            GetPoints(index, out x1Val, out x2Val, sbsStart, sbsEnd);
            openVal = y_isInversed
            ? closeChartVals[index]
            : openChartVals[index];
            closeVal = y_isInversed
                ? openChartVals[index]
                : closeChartVals[index];
            if (index == 0 || (Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = openValues < closeValues;
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Insert(0, bullBearFlag);
            isHollow.Insert(0, closeValues > openValues &&
                         !((Series as FastCandleBitmapSeries).ComparisonMode == FinancialPrice.None));
            if (y_isInversed ? openVal > closeVal : openVal < closeVal)
            {
                tempOpenVal = openVal;
                openVal = closeVal;
                closeVal = tempOpenVal;
            }
            yHiVal = highValues;
            yLoVal = lowValues;
            if (openValues > closeValues)
            {
                yHiVal1 = openValues;
                yLoVal1 = closeValues;
            }
            else
            {
                yHiVal1 = closeValues;
                yLoVal1 = openValues;
            }
            Point blpoint = cartesianTransformer.TransformToVisible(x1Val, openVal);
            Point trpoint = cartesianTransformer.TransformToVisible(x2Val, closeVal);
            Point point1, point2, point3, point4;
            if (fastCandleBitmapSeries.IsIndexed)
            {
                point1 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal);
                point2 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal);
            }
            else
            {
                point1 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yHiVal);
                point2 = cartesianTransformer.TransformToVisible(xChartVals[index] + sbsCenter, yLoVal);
            }
            point3 = cartesianTransformer.TransformToVisible(index + sbsCenter, yHiVal1);
            point4 = cartesianTransformer.TransformToVisible(index + sbsCenter, yLoVal1);
            xValues.Insert(0, ((float)point1.X));
            x1Values.Insert(0, (float)blpoint.X);
            x2Values.Insert(0, (float)trpoint.X);
            openValue.Insert(0, (float)blpoint.Y);
            closeValue.Insert(0, (float)trpoint.Y);
            highValue.Insert(0, (float)point1.Y);
            lowValue.Insert(0, (float)point2.Y);
            highValue1.Insert(0, (float)point3.Y);
            lowValue1.Insert(0, (float)point4.Y);
        }

        private void GetPoints(int index, out double x1Val, out double x2Val, double sbsStart, double sbsEnd)
        {
            if (fastCandleBitmapSeries.IsIndexed)
            {
                x1Val = !x_isInversed ? (sbsStart + index) : (sbsEnd + index);
                x2Val = !x_isInversed ? (sbsEnd + index) : (sbsStart + index);
            }
            else
            {
                x1Val = x_isInversed ? (xChartVals[index] + sbsEnd) : (xChartVals[index] + sbsStart);
                x2Val = x_isInversed ? (xChartVals[index] + sbsStart) : (xChartVals[index] + sbsEnd);
            }
        }

        private void UpdateVisualVertical(int width, int height, Color color, int leftThickness, int rightThickness)
        {
            float x, x1, x2, open, close, high, high1, low, low1;
            int dataCount = xValues.Count;
            Brush bullFillColor = (fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor;
            Brush bearFillColor = (fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor;

            for (int i = 0; i < dataCount; i++)
            {
                if (isSeriesSelected)
                    color = seriesSelectionColor;
                else if (fastCandleBitmapSeries.SelectedSegmentsIndexes.Contains(startIndex) && (fastCandleBitmapSeries as ISegmentSelectable).SegmentSelectionBrush != null)
                    color = segmentSelectionColor;
                else if (this.Series.Interior != null)
                    color = (Series.Interior as SolidColorBrush).Color;
                else if (isBull[i])
                    color = bullFillColor != null ? color = ((SolidColorBrush)bullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = bearFillColor != null ? color = ((SolidColorBrush)bearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                startIndex++;
                x = xValues[i];
                x1 = x1Values[i];
                x2 = x2Values[i];
                open = openValue[i];
                close = closeValue[i];
                high = y_isInversed ? lowValue[i] : highValue[i];
                low = y_isInversed ? highValue[i] : lowValue[i];
                high1 = y_isInversed ? lowValue1[i] : highValue1[i];
                low1 = y_isInversed ? highValue1[i] : lowValue1[i];
                var leftOffset = (int)x - leftThickness;
                var rightOffset = (int)x + rightThickness;
                double spacing = (fastCandleBitmapSeries as ISegmentSpacing).SegmentSpacing;
                if (spacing > 0 && spacing <= 1)
                {
                    double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, x1, x2);
                    double rightpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, x2, x1);
                    x2 = (float)leftpos;
                    x1 = (float)rightpos;
                }
                close = open == close ? close + 1 : close;

                if (isHollow[i])
                    bitmap.DrawRectangle(fastBuffer, (int)width, (int)height, (int)(width - close), (int)(height - x2), (int)(width - open), (int)(height - x1), color, fastCandleBitmapSeries.bitmapPixels);
                else
                    bitmap.FillRectangle(fastBuffer, (int)width, (int)height, (int)(width - close), (int)(height - x2), (int)(width - open), (int)(height - x1), color, fastCandleBitmapSeries.bitmapPixels);
                bitmap.FillRectangle(fastBuffer, width, height, (int)high1, (int)(leftOffset), (int)high, (int)(rightOffset), color, fastCandleBitmapSeries.bitmapPixels);
                bitmap.FillRectangle(fastBuffer, width, height, (int)low, (int)(leftOffset), (int)low1, (int)(rightOffset), color, fastCandleBitmapSeries.bitmapPixels);
            }
        }

        private void UpdateVisualHorizontal(int width, int height, Color color, int leftThickness, int rightThickness)
        {
            float x, x1, x2, open, close, high, low, high1, low1;
            int dataCount = xValues.Count;
            Brush bullFillColor = (fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor;
            Brush bearFillColor = (fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor;

            for (int i = 0; i < dataCount; i++)
            {
                if (isSeriesSelected)
                    color = seriesSelectionColor;
                else if (fastCandleBitmapSeries.SelectedSegmentsIndexes.Contains(startIndex) && (fastCandleBitmapSeries as ISegmentSelectable).SegmentSelectionBrush != null)
                    color = segmentSelectionColor;
                else if (this.Series.Interior != null)
                    color = (Series.Interior as SolidColorBrush).Color;
                else if (isBull[i])
                    color = bullFillColor != null ? color = ((SolidColorBrush)bullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = bearFillColor != null ? color = ((SolidColorBrush)bearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                startIndex++;
                x = xValues[i];
                x1 = x1Values[i];
                x2 = x2Values[i];
                open = openValue[i];//FastCandleBitmap series not render properly in Empty point item source-WPF-18144
                close = closeValue[i];
                high = y_isInversed ? lowValue[i] : highValue[i];
                low = y_isInversed ? highValue[i] : lowValue[i];
                high1 = y_isInversed ? lowValue1[i] : highValue1[i];
                low1 = y_isInversed ? highValue1[i] : lowValue1[i];
                var leftOffset = x - leftThickness;
                var rightOffset = x + rightThickness;
                double spacing = (fastCandleBitmapSeries as ISegmentSpacing).SegmentSpacing;
                if (spacing > 0 && spacing <= 1)
                {
                    double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, x2, x1);
                    double rightpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, x1, x2);
                    x1 = (float)(leftpos);
                    x2 = (float)(rightpos);
                }
                close = open == close ? close + 1 : close;

                if (isHollow[i])
                    bitmap.DrawRectangle(fastBuffer, width, height, (int)(x1), (int)open, (int)x2, (int)close, color, fastCandleBitmapSeries.bitmapPixels);
                else
                    bitmap.FillRectangle(fastBuffer, width, height, (int)(x1), (int)open, (int)x2, (int)close, color, fastCandleBitmapSeries.bitmapPixels);
                bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)high, (int)rightOffset, (int)high1, color, fastCandleBitmapSeries.bitmapPixels);
                bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)low1, (int)rightOffset, (int)low, color, fastCandleBitmapSeries.bitmapPixels);
            }
        }

        #endregion

        #endregion        
    }
}
