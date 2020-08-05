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
    /// Represents chart fast hilo open close bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSegment"/>
    /// <seealso cref="FastHiLoSegment"/>
    public partial class FastHiLoOpenCloseSegment : ChartSegment
    {
        #region Fields

        #region Internal Fields

        internal AdornmentSeries fastHiLoOpenCloseSeries;
        
        #endregion

        #region Private Fields

        private WriteableBitmap bitmap;

        private byte[] fastBuffer;

        private IList<double> xChartVals;

        private IList<double> yHiChartVals;

        private IList<double> yLoChartVals;

        private IList<double> yOpenChartVals;

        private IList<double> yCloseChartVals;

        DoubleRange sbsInfo;

        double center, Left, Right;

        Color seriesSelectionColor = Colors.Transparent;

        Color segmentSelectionColor = Colors.Transparent;

        bool isSeriesSelected;

        List<float> xValues;
        List<float> yHiValues;
        List<float> yLoValues;
        List<float> yOpenStartValues;
        List<float> yOpenEndValues;
        List<float> yCloseValues;
        List<float> yCloseEndValues;
        List<bool> isBull;
        int startIndex = 0;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastHiLoOpenCloseSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastHiLoOpenCloseSegment
        /// </summary>
        /// <param name="series"></param>
        public FastHiLoOpenCloseSegment(AdornmentSeries series)
        {
            xValues = new List<float>();
            yHiValues = new List<float>();
            yLoValues = new List<float>();
            yOpenStartValues = new List<float>();
            yOpenEndValues = new List<float>();
            yCloseValues = new List<float>();
            yCloseEndValues = new List<float>();
            isBull = new List<bool>();
            fastHiLoOpenCloseSeries = series;
        }

        /// <summary>
        /// Called when instance created for FastHiLoOpenCloseSegment with following arguments
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="highValues"></param>
        /// <param name="lowValues"></param>
        /// <param name="openValues"></param>
        /// <param name="closeValues"></param>
        /// <param name="series"></param>
        public FastHiLoOpenCloseSegment(List<double> xValues, IList<double> highValues, IList<double> lowValues, IList<double> openValues, IList<double> closeValues, AdornmentSeries series)
            : this(series)
        {

        }

        #endregion

        #region Methods

        #region Public Override Methods
        
        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        public override void OnSizeChanged(Size size)
        {
            bitmap = fastHiLoOpenCloseSeries.Area.GetFastRenderSurface();
            fastBuffer = fastHiLoOpenCloseSeries.Area.GetFastBuffer();
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastHiLoOpenCloseSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastHiLoOpenCloseSeries.DataCount > 0)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                xValues.Clear();
                yHiValues.Clear();
                yLoValues.Clear();
                yOpenStartValues.Clear();
                yOpenEndValues.Clear();
                yCloseValues.Clear();
                yCloseEndValues.Clear();
                isBull.Clear();
                x_isInversed = cartesianTransformer.XAxis.IsInversed;
                y_isInversed = cartesianTransformer.YAxis.IsInversed;
                sbsInfo = (fastHiLoOpenCloseSeries as ChartSeriesBase).GetSideBySideInfo(fastHiLoOpenCloseSeries as ChartSeriesBase);
                center = sbsInfo.Median;
                Left = sbsInfo.Start;
                Right = sbsInfo.End;
                //Removed the screen point calculation methods and added the Transform to Visible method.
                CalculatePoint(cartesianTransformer);
                UpdateVisual(true);
            }
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yHiValues"></param>
        /// <param name="yLowValues"></param>
        /// <param name="yOpenValues"></param>
        /// <param name="yCloseValues"></param>       
        public override void SetData(IList<double> xValues, IList<double> yHiValues, IList<double> yLowValues, IList<double> yOpenValues, IList<double> yCloseValues)
        {
            DoubleRange sbsInfo = fastHiLoOpenCloseSeries.GetSideBySideInfo(fastHiLoOpenCloseSeries);
            this.xChartVals = xValues;
            this.yHiChartVals = yHiValues;
            this.yLoChartVals = yLowValues;
            this.yOpenChartVals = yOpenValues;
            this.yCloseChartVals = yCloseValues;
            List<double> yValues = new List<double>();
            yValues.AddRange(yHiValues);
            yValues.AddRange(yLowValues);
            if (fastHiLoOpenCloseSeries.DataCount > 0)
            {
                double _Min = yValues.Min();
                double Y_MIN;
                if (double.IsNaN(_Min))
                {
                    var yval = yValues.Where(e => !double.IsNaN(e));
                    Y_MIN = (!yval.Any()) ? 0 : yval.Min();
                }
                else
                {
                    Y_MIN = _Min;
                }
                double X_MAX, Y_MAX, X_MIN;
                if (fastHiLoOpenCloseSeries.IsIndexed)
                {
                    X_MAX = ((Series.ActualXAxis is CategoryAxis && !(Series.ActualXAxis as CategoryAxis).IsIndexed)) ?
                       Series.GroupedXValuesIndexes.Max() : fastHiLoOpenCloseSeries.DataCount - 1;
                    X_MIN = 0;
                }
                else
                {
                    X_MAX = xChartVals.Max();
                    X_MIN = xChartVals.Min();

                }
                Y_MAX = yValues.Max();
                X_MIN += sbsInfo.Start;
                X_MAX += sbsInfo.End;
                XRange = new DoubleRange(X_MIN, X_MAX);
                YRange = new DoubleRange(Y_MIN, Y_MAX);
            }
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
            bitmap = fastHiLoOpenCloseSeries.Area.GetFastRenderSurface();
            fastBuffer = fastHiLoOpenCloseSeries.Area.GetFastBuffer();
            return null;
        }

        #endregion

        #region Internal Methods

          /// <summary>
        /// Gets the segment color based on data point
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal Color GetSegmentBrush(int index)
        {
            Color color;
            if (isBull.Count > index && isBull[index])
                color = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor != null ? color = ((SolidColorBrush)(fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
            else
                color = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor != null ? color = ((SolidColorBrush)(fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
               
            return color;
         }

        internal void UpdateVisual(bool updateHiLoLine)
        {
            if (bitmap != null && xValues.Count > 0)
            {
                fastBuffer = fastHiLoOpenCloseSeries.Area.GetFastBuffer();
                int width = (int)fastHiLoOpenCloseSeries.Area.SeriesClipRect.Width;
                int height = (int)fastHiLoOpenCloseSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastHiLoOpenCloseSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastHiLoOpenCloseSeries.StrokeThickness % 2 == 0
                    ? (fastHiLoOpenCloseSeries.StrokeThickness / 2) : fastHiLoOpenCloseSeries.StrokeThickness / 2+1);

                if (fastHiLoOpenCloseSeries is FastHiLoOpenCloseBitmapSeries)
                {
                    SfChart chart = fastHiLoOpenCloseSeries.Area;
                    isSeriesSelected = false;

                    //Set SeriesSelectionBrush and Check EnableSeriesSelection        
                    if (chart.GetEnableSeriesSelection())
                    {
                        Brush seriesSelectionBrush = chart.GetSeriesSelectionBrush(fastHiLoOpenCloseSeries);
                        if (seriesSelectionBrush != null && chart.SelectedSeriesCollection.Contains(fastHiLoOpenCloseSeries))
                        {
                            isSeriesSelected = true;
                            seriesSelectionColor = ((SolidColorBrush)seriesSelectionBrush).Color;
                        }
                    }
                    else if (chart.GetEnableSegmentSelection())//Set SegmentSelectionBrush and Check EnableSegmentSelection
                    {
                        Brush segmentSelectionBrush = (fastHiLoOpenCloseSeries as ISegmentSelectable).SegmentSelectionBrush;
                        if (segmentSelectionBrush != null)
                        {
                            segmentSelectionColor = ((SolidColorBrush)segmentSelectionBrush).Color;
                        }
                    }

                    if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                        UpdateVisualHorizontal(width, height,leftThickness,rightThickness);
                    else
                        UpdateVisualVertical(width, height, leftThickness, rightThickness);
                }
            }
            fastHiLoOpenCloseSeries.Area.CanRenderToBuffer = true;

            xValues.Clear();
            yHiValues.Clear();
            yLoValues.Clear();
            yOpenStartValues.Clear();
            yOpenEndValues.Clear();
            yCloseValues.Clear();
            yCloseEndValues.Clear();
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
        }

        #endregion

        #region Private Methods

        private void AddHorizontalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index, IList<double> values)
        {
            bool bullBearFlag = false;
            double highValues = yHiChartVals[index];
            double lowValues = yLoChartVals[index];
            var alignedValues = AlignHiLoSegment(yOpenChartVals[index], yCloseChartVals[index], highValues, lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            Point hiPoint, loPoint, startopenpoint, endopenpoint, startclosepoint, endclosepoint;
            GetPoints(cartesianTransformer, index, out hiPoint, out loPoint, out startopenpoint,
                out endopenpoint, out startclosepoint, out endclosepoint, highValues, lowValues);
            xValues.Add((float)hiPoint.X);
            yHiValues.Add((float)hiPoint.Y);
            yLoValues.Add((float)loPoint.Y);
            yOpenStartValues.Add((float)startopenpoint.Y);
            yOpenEndValues.Add((float)endopenpoint.X);
            yCloseValues.Add((float)endclosepoint.Y);
            yCloseEndValues.Add((float)endclosepoint.X);
            if (index == 0 || (Series as FastHiLoOpenCloseBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = yOpenChartVals[index] <= yCloseChartVals[index];
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Add(bullBearFlag);
        }

        private void InsertHorizontalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index, IList<double> values)
        {
            bool bullBearFlag = false;
            double highValues = yHiChartVals[index];
            double lowValues = yLoChartVals[index];
            var alignedValues = AlignHiLoSegment(yOpenChartVals[index], yCloseChartVals[index], highValues, lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            Point hiPoint, loPoint, startopenpoint, endopenpoint, startclosepoint, endclosepoint;
            GetPoints(cartesianTransformer, index, out hiPoint, out loPoint, out startopenpoint,
                out endopenpoint, out startclosepoint, out endclosepoint, highValues, lowValues);
            xValues.Insert(0, (float)hiPoint.X);
            yHiValues.Insert(0, (float)hiPoint.Y);
            yLoValues.Insert(0, (float)loPoint.Y);
            yOpenStartValues.Insert(0, (float)startopenpoint.Y);
            yOpenEndValues.Insert(0, (float)endopenpoint.X);
            yCloseValues.Insert(0, (float)endclosepoint.Y);
            yCloseEndValues.Insert(0, (float)endclosepoint.X);
            if (index == 0 || (Series as FastHiLoOpenCloseBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = yOpenChartVals[index] <= yCloseChartVals[index];
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Insert(0, bullBearFlag);
        }
        private void GetPoints(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index,
            out Point hiPoint, out Point loPoint, out Point startopenpoint,
            out Point endopenpoint, out Point startclosepoint, out Point endclosepoint,
            double highValues, double lowValues)
        {
            if (fastHiLoOpenCloseSeries.IsIndexed)
            {
                hiPoint = cartesianTransformer.TransformToVisible(index + center, highValues);
                loPoint = cartesianTransformer.TransformToVisible(xChartVals[index] + center, lowValues);
                startopenpoint = cartesianTransformer.TransformToVisible(xChartVals[index] + center, yOpenChartVals[index]);
                endopenpoint = cartesianTransformer.TransformToVisible(xChartVals[index] + Left, yOpenChartVals[index]);
                startclosepoint = cartesianTransformer.TransformToVisible(index + center, yCloseChartVals[index]);
                endclosepoint = cartesianTransformer.TransformToVisible(index + Right, yCloseChartVals[index]);
            }
            else
            {
                hiPoint = cartesianTransformer.TransformToVisible(xChartVals[index] + center, highValues);
                loPoint = cartesianTransformer.TransformToVisible(xChartVals[index] + center, lowValues);
                startopenpoint = cartesianTransformer.TransformToVisible(xChartVals[index] + center, yOpenChartVals[index]);
                endopenpoint = cartesianTransformer.TransformToVisible(xChartVals[index] + Left, yOpenChartVals[index]);
                startclosepoint = cartesianTransformer.TransformToVisible(xChartVals[index] + center, yCloseChartVals[index]);
                endclosepoint = cartesianTransformer.TransformToVisible(xChartVals[index] + Right, yCloseChartVals[index]);
            }
        }
        private void AddVerticalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index,
            IList<double> values)
        {
            bool bullBearFlag = false;
            double highValues = yHiChartVals[index];
            double lowValues = yLoChartVals[index];
            var alignedValues = AlignHiLoSegment(yOpenChartVals[index], yCloseChartVals[index], highValues, lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            Point hiPoint, loPoint, startopenpoint, startclosepoint, endopenpoint, endclosepoint;
            GetPoints(cartesianTransformer, index, out hiPoint, out loPoint, out startopenpoint,
                out endopenpoint, out startclosepoint, out endclosepoint, highValues, lowValues);
            xValues.Add((float)hiPoint.Y);
            yHiValues.Add((float)hiPoint.X);
            yLoValues.Add((float)loPoint.X);
            yOpenStartValues.Add((float)startopenpoint.X);
            yOpenEndValues.Add((float)endopenpoint.Y);
            yCloseValues.Add((float)endclosepoint.X);
            yCloseEndValues.Add((float)endclosepoint.Y);
            if (index == 0 || (Series as FastHiLoOpenCloseBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = yOpenChartVals[index] <= yCloseChartVals[index];
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Add(bullBearFlag);
        }

        private void InsertVerticalPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index,
           IList<double> values)
        {
            bool bullBearFlag = false;
            double highValues = yHiChartVals[index];
            double lowValues = yLoChartVals[index];
            var alignedValues = AlignHiLoSegment(yOpenChartVals[index], yCloseChartVals[index], highValues, lowValues);
            highValues = alignedValues[0];
            lowValues = alignedValues[1];
            Point hiPoint, loPoint, startopenpoint, startclosepoint, endopenpoint, endclosepoint;
            GetPoints(cartesianTransformer, index, out hiPoint, out loPoint, out startopenpoint,
                 out endopenpoint, out startclosepoint, out endclosepoint, highValues, lowValues);
            xValues.Insert(0, ((float)hiPoint.Y));
            yHiValues.Insert(0, (float)hiPoint.X);
            yLoValues.Insert(0, (float)loPoint.X);
            yOpenStartValues.Insert(0, (float)startopenpoint.X);
            yOpenEndValues.Insert(0, (float)endopenpoint.Y);
            yCloseValues.Insert(0, (float)endclosepoint.X);
            yCloseEndValues.Insert(0, (float)endclosepoint.Y);
            if (index == 0 || (Series as FastHiLoOpenCloseBitmapSeries).ComparisonMode == FinancialPrice.None)
                bullBearFlag = yOpenChartVals[index] <= yCloseChartVals[index];
            else
                bullBearFlag = values[index] >= values[index - 1];
            isBull.Insert(0, bullBearFlag);
        }

        private void CalculatePoint(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            IList<double> values = (Series as FastHiLoOpenCloseBitmapSeries).GetComparisionModeValues();
            double spacing = (Series as ISegmentSpacing).SegmentSpacing;
            double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, Right, Left);
            double rightpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, Left, Right);
            if (spacing > 0 && spacing <= 1)
            {
                Left = leftpos;
                Right = rightpos;
            }
            ChartAxis xAxis = cartesianTransformer.XAxis;
            if (fastHiLoOpenCloseSeries.IsIndexed)
            {
                bool isGrouping = (fastHiLoOpenCloseSeries.ActualXAxis is CategoryAxis) &&
                                (fastHiLoOpenCloseSeries.ActualXAxis as CategoryAxis).IsIndexed;

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
                    end = end > yHiChartVals.Count - 1 ? yHiChartVals.Count - 1 : end;
                }
                start = start < 0 ? 0 : start;
                startIndex = start;
                if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                {
                    for (int i = start; i <= end; i++)
                    {
                        AddHorizontalPoint(cartesianTransformer, i, values);
                    }
                }
                else
                {
                    for (int i = start; i <= end; i++)
                    {
                        AddVerticalPoint(cartesianTransformer, i, values);
                    }
                }
            }
            else
            {
                if (fastHiLoOpenCloseSeries.isLinearData)
                {
                    double start = xAxis.VisibleRange.Start;
                    double end = xAxis.VisibleRange.End;
                    int i = 0;
                    startIndex = 0;
                    int count = xChartVals.Count - 1;
                    if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                    {
                        double xBase = cartesianTransformer.XAxis.IsLogarithmic ?
                        (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 0;
                        for (i = 1; i < count; i++)
                        {
                            double xValue = xChartVals[i];
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
                            AddHorizontalPoint(cartesianTransformer, count, values);
                    }
                    else
                    {
                        double xBase = cartesianTransformer.XAxis.IsLogarithmic ?
                         (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 0;
                        for (i = 1; i < count; i++)
                        {
                            double xValue = xChartVals[i];
                            if (cartesianTransformer.XAxis.IsLogarithmic)
                                xValue = Math.Log(xValue, xBase);
                            if (xValue <= end && xValue >= start)
                                AddVerticalPoint(cartesianTransformer, i, values);
                            else if (xValue < start)
                                startIndex = i;
                            else if (xValue > end)
                            {
                                AddVerticalPoint(cartesianTransformer, i, values);
                                break;
                            }
                        }
                        InsertVerticalPoint(cartesianTransformer, startIndex, values);
                        if (i == count)
                            AddVerticalPoint(cartesianTransformer, count, values);
                    }
                }
                else
                {
                    startIndex = 0;
                    for (int i = 0; i < this.Series.DataCount; i++)
                    {
                        if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                            AddHorizontalPoint(cartesianTransformer, i, values);
                        else
                            AddVerticalPoint(cartesianTransformer, i, values);
                    }
                }
            }
        }

        private void UpdateVisualVertical(int width, int height, int leftThickness, int rightThickness)
        {
            float xStart = 0;
            float yStart = 0;
            float yEnd = 0;
            float yOpen = 0;
            float yOpenEnd = 0;
            float yClose = 0;
            float yCloseEnd = 0;
            Color color;
            int leftOffset, rightOffset;
            Brush bullFillColor = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor;
            Brush bearFillColor = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor;

            for (int i = 0; i < xValues.Count; i++)
            {
                if (isSeriesSelected)
                    color = seriesSelectionColor;
                else if (fastHiLoOpenCloseSeries.SelectedSegmentsIndexes.Contains(startIndex) && (fastHiLoOpenCloseSeries as ISegmentSelectable).SegmentSelectionBrush != null)
                    color = segmentSelectionColor;
                else if (this.Series.Interior != null)
                    color = (Series.Interior as SolidColorBrush).Color;
                else if (isBull[i])
                    color = bullFillColor != null ? color = ((SolidColorBrush)bullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = bearFillColor != null ? color = ((SolidColorBrush)bearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                startIndex++;
                xStart = xValues[i];
                yStart = y_isInversed ? yLoValues[i] : yHiValues[i];
                yEnd = y_isInversed ? yHiValues[i] : yLoValues[i];
                yOpen = x_isInversed ? yCloseValues[i] : yOpenStartValues[i];
                yOpenEnd = x_isInversed ? yCloseEndValues[i] : yOpenEndValues[i];
                yClose = x_isInversed ? yOpenStartValues[i] : yCloseValues[i];
                yCloseEnd = x_isInversed ? yOpenEndValues[i] : yCloseEndValues[i];

                leftOffset = (int)xStart - leftThickness;
                rightOffset = (int)xStart + rightThickness;

                if (!double.IsNaN(yStart) && !double.IsNaN(yEnd))
                    bitmap.FillRectangle(fastBuffer, width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset,
                    color, fastHiLoOpenCloseSeries.bitmapPixels);

                if (!double.IsNaN(yOpen))
                {
                    leftOffset = (int)yOpen - leftThickness;
                    rightOffset = (int)yOpen + rightThickness;
                    bitmap.FillRectangle(fastBuffer, width, height, leftOffset, (int)xStart + leftThickness, (int)rightOffset,
                    (int)yOpenEnd, color, fastHiLoOpenCloseSeries.bitmapPixels);
                }

                if (!double.IsNaN(yClose))
                {
                    leftOffset = (int)yClose - leftThickness;
                    rightOffset = (int)yClose + rightThickness;
                    bitmap.FillRectangle(fastBuffer, width, height, leftOffset, (int)yCloseEnd, (int)rightOffset,
                    (int)xStart - leftThickness, color, fastHiLoOpenCloseSeries.bitmapPixels);
                }
            }
        }

        private void UpdateVisualHorizontal(int width, int height, int leftThickness, int rightThickness)
        {
            Color color;
            float xStart = 0;
            float yStart = 0;
            float yEnd = 0;
            float yOpen = 0;
            float yOpenEnd = 0;
            float yClose = 0;
            float yCloseEnd = 0;
            int leftOffset, rightOffset;
            Brush bullFillColor = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor;
            Brush bearFillColor = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor;

            for (int i = 0; i < xValues.Count; i++)
            {
                if (isSeriesSelected)
                    color = seriesSelectionColor;
                else if (fastHiLoOpenCloseSeries.SelectedSegmentsIndexes.Contains(startIndex) && (fastHiLoOpenCloseSeries as ISegmentSelectable).SegmentSelectionBrush != null)
                    color = segmentSelectionColor;
                else if (this.Series.Interior != null)
                    color = (Series.Interior as SolidColorBrush).Color;
                else if (isBull[i])
                    color = bullFillColor != null ? color = ((SolidColorBrush)bullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = bearFillColor != null ? color = ((SolidColorBrush)bearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                startIndex++;
                xStart = xValues[i];
                yStart = y_isInversed ? yLoValues[i] : yHiValues[i];
                yEnd = y_isInversed ? yHiValues[i] : yLoValues[i];
                yOpen = x_isInversed ? yCloseValues[i] : yOpenStartValues[i];
                yOpenEnd = x_isInversed ? yCloseEndValues[i] : yOpenEndValues[i];
                yClose = x_isInversed ? yOpenStartValues[i] : yCloseValues[i];
                yCloseEnd = x_isInversed ? yOpenEndValues[i] : yCloseEndValues[i];

                leftOffset = (int)xStart - leftThickness;
                rightOffset = (int)xStart + rightThickness;
                if (!double.IsNaN(yStart) && !double.IsNaN(yEnd))
                    bitmap.FillRectangle(fastBuffer, width, height, leftOffset, (int)yStart, rightOffset, (int)yEnd,
                        color, fastHiLoOpenCloseSeries.bitmapPixels);
                if (!double.IsNaN(yOpen))
                {
                    leftOffset = (int)yOpen - leftThickness;
                    rightOffset = (int)yOpen + rightThickness;

                    bitmap.FillRectangle(fastBuffer, width, height, (int)yOpenEnd, leftOffset,
                        (int)xStart - leftThickness, (int)rightOffset, color, fastHiLoOpenCloseSeries.bitmapPixels);
                }
                if (!double.IsNaN(yClose))
                {
                    leftOffset = (int)yClose - leftThickness;
                    rightOffset = (int)yClose + rightThickness;

                    bitmap.FillRectangle(fastBuffer, width, height, (int)xStart + leftThickness, leftOffset,
                        (int)yCloseEnd, (int)rightOffset, color, fastHiLoOpenCloseSeries.bitmapPixels);
                }
            }

        }

        #endregion

        #endregion
    }
}
