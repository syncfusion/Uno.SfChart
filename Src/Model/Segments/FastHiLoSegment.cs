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
    /// Represents chart fast hilo bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    public partial class FastHiLoSegment : ChartSegment
    {
        #region Fields

        #region Internal Fields

        internal ChartSeries fastHiloSeries;
        
        #endregion

        #region Private Fields

        private WriteableBitmap bitmap;

        private byte[] fastBuffer;

        private IList<double> xChartVals;

        private IList<double> yHiChartVals;

        private IList<double> yLoChartVals;

        Color seriesSelectionColor = Colors.Transparent;

        Color segmentSelectionColor = Colors.Transparent;

        bool isSeriesSelected;

        List<float> xValues;

        List<float> yHiValues;

        List<float> yLoValues;

        int startIndex = 0;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastHiLoSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastHiLoSegment
        /// </summary>
        /// <param name="series"></param>
        public FastHiLoSegment(ChartSeries series)
        {
            xValues = new List<float>();

            yHiValues = new List<float>();

            yLoValues = new List<float>();

            fastHiloSeries = series;
        }

        /// <summary>
        /// Called when instance created for FastHiLoSegment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="hiValues"></param>
        /// <param name="loValues"></param>
        /// <param name="series"></param>
        public FastHiLoSegment(IList<double> xValues, IList<double> hiValues, IList<double> loValues, AdornmentSeries series)
            : this(series)
        {

        }

        #endregion

        #region Methods

        #region Public Override Methods

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
            bitmap = fastHiloSeries.Area.GetFastRenderSurface();
            fastBuffer = fastHiloSeries.Area.GetFastBuffer();
            return null;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="hiVals"></param>
        /// <param name="lowVals"></param>        
        public override void SetData(IList<double> xVals, IList<double> hiVals, IList<double> lowVals)
        {
            this.xChartVals = xVals;
            this.yHiChartVals = hiVals;
            this.yLoChartVals = lowVals;
            List<double> yValues = new List<double>();
            yValues.AddRange(hiVals as List<double>);
            yValues.AddRange(lowVals as List<double>);


            if (fastHiloSeries.DataCount > 0)
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
                if (fastHiloSeries.IsIndexed)
                {
                    double X_MAX = ((Series.ActualXAxis is CategoryAxis && !(Series.ActualXAxis as CategoryAxis).IsIndexed)) ?
                     xChartVals.Max() : fastHiloSeries.DataCount - 1;
                    double Y_MAX = yValues.Max();
                    double X_MIN = 0;

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
                else
                {
                    double X_MAX = xChartVals.Max();
                    double Y_MAX = yValues.Max();
                    double X_MIN = xChartVals.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
            }

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
            bitmap = fastHiloSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastHiloSeries.DataCount > 0)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                xValues.Clear();
                yHiValues.Clear();
                yLoValues.Clear();
                x_isInversed = cartesianTransformer.XAxis.IsInversed;
                y_isInversed = cartesianTransformer.YAxis.IsInversed;
                //Removed the screen point calculation methods and added the point to value method.
                CalculatePoints(cartesianTransformer);
                UpdateVisual(true);
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
            bitmap = fastHiloSeries.Area.GetFastRenderSurface();
            fastBuffer = fastHiloSeries.Area.GetFastBuffer();
        }

        #endregion

        #region Internal Methods

        internal void UpdateVisual(bool updateHiLoLine)
        {
            bool isMultiColor = fastHiloSeries.Palette != ChartColorPalette.None && fastHiloSeries.Interior == null;
            Color color = GetColor(this.Interior);
            int dataCount = 0;
            if (yLoValues.Count < xValues.Count)
                dataCount = yLoValues.Count;
            else
                dataCount = xValues.Count;
            if (bitmap != null && xValues.Count > 0)
            {
                fastBuffer = fastHiloSeries.Area.GetFastBuffer();
                int width = (int)fastHiloSeries.Area.SeriesClipRect.Width;
                int height = (int)fastHiloSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastHiloSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastHiloSeries.StrokeThickness % 2 == 0
                    ? (fastHiloSeries.StrokeThickness / 2) : fastHiloSeries.StrokeThickness / 2 + 1);

                if (fastHiloSeries is FastHiLoBitmapSeries)
                {
                    SfChart chart = fastHiloSeries.Area;
                    isSeriesSelected = false;

                    //Set SeriesSelectionBrush and Check EnableSeriesSelection        
                    if (chart.GetEnableSeriesSelection())
                    {
                        Brush seriesSelectionBrush = chart.GetSeriesSelectionBrush(fastHiloSeries);
                        if (seriesSelectionBrush != null && chart.SelectedSeriesCollection.Contains(fastHiloSeries))
                        {
                            isSeriesSelected = true;
                            seriesSelectionColor = ((SolidColorBrush)seriesSelectionBrush).Color;
                        }
                    }
                    else if (chart.GetEnableSegmentSelection())//Set SegmentSelectionBrush and Check EnableSegmentSelection
                    {
                        Brush segmentSelectionBrush = (fastHiloSeries as ISegmentSelectable).SegmentSelectionBrush;
                        if (segmentSelectionBrush != null)
                        {
                            segmentSelectionColor = ((SolidColorBrush)segmentSelectionBrush).Color;
                        }
                    }

                    if (!fastHiloSeries.IsActualTransposed)
                        UpdateVisualHorizontal(width, height, color, leftThickness, rightThickness, isMultiColor, dataCount);
                    else
                        UpdateVisualVertical(width, height, color, leftThickness, rightThickness, isMultiColor, dataCount);
                }
            }
            fastHiloSeries.Area.CanRenderToBuffer = true;
            xValues.Clear();
            yHiValues.Clear();
            yLoValues.Clear();
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

        private void AddDataPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index)
        {
            Point hipoint, lopoint;
            GetHiLoPoints(cartesianTransformer, index, out hipoint, out lopoint);
            if (!fastHiloSeries.IsActualTransposed)
            {
                xValues.Add((float)hipoint.X);
                yHiValues.Add((float)hipoint.Y);
                yLoValues.Add((float)lopoint.Y);
            }
            else
            {
                xValues.Add((float)hipoint.Y);
                yHiValues.Add((float)hipoint.X);
                yLoValues.Add((float)lopoint.X);
            }
        }
        private void InsertDataPoint(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index)
        {
            Point hipoint, lopoint;
            GetHiLoPoints(cartesianTransformer, index, out hipoint, out lopoint);
            if (!fastHiloSeries.IsActualTransposed)
            {
                xValues.Insert(0, (float)hipoint.X);
                yHiValues.Insert(0, (float)hipoint.Y);
                yLoValues.Insert(0, (float)lopoint.Y);
            }
            else
            {
                xValues.Insert(0, (float)hipoint.Y);
                yHiValues.Insert(0, (float)hipoint.X);
                yLoValues.Insert(0, (float)lopoint.X);
            }
        }

        private void GetHiLoPoints(ChartTransform.ChartCartesianTransformer cartesianTransformer, int index,
            out Point hipoint, out Point lopoint)
        {
            if (fastHiloSeries.IsIndexed)
            {
                hipoint = cartesianTransformer.TransformToVisible(index, yHiChartVals[index]);
                lopoint = cartesianTransformer.TransformToVisible(index, yLoChartVals[index]);
            }
            else
            {
                hipoint = cartesianTransformer.TransformToVisible(xChartVals[index], yHiChartVals[index]);
                lopoint = cartesianTransformer.TransformToVisible(xChartVals[index], yLoChartVals[index]);
            }
        }
        private void CalculatePoints(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            ChartAxis xAxis = cartesianTransformer.XAxis;
            if (fastHiloSeries.IsIndexed)
            {
                bool isGrouping = (fastHiloSeries.ActualXAxis is CategoryAxis) &&
                                  (fastHiloSeries.ActualXAxis as CategoryAxis).IsIndexed;
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
                for (int i = start; i <= end; i++)
                {
                    if (i < yHiChartVals.Count)
                    {
                        AddDataPoint(cartesianTransformer, i);
                    }
                }
            }
            else if (fastHiloSeries.isLinearData)
            {
                double start = xAxis.VisibleRange.Start;
                double end = xAxis.VisibleRange.End;
                startIndex = 0;
                int count = xChartVals.Count - 1;
                int i = 0;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ?
                        (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 0;
                for (i = 1; i < count; i++)
                {
                    double xValue = xChartVals[i];
                    if (cartesianTransformer.XAxis.IsLogarithmic)
                        xValue = Math.Log(xValue, xBase);
                    if (xValue <= end && xValue >= start)
                        AddDataPoint(cartesianTransformer, i);
                    else if (xValue < start)
                        startIndex = i;
                    else if (xValue > end)
                    {
                        AddDataPoint(cartesianTransformer, i);
                        break;
                    }
                }
                InsertDataPoint(cartesianTransformer, startIndex);
                if (i == count)
                    AddDataPoint(cartesianTransformer, count);
            }
            else
            {
                for (int i = 0; i < this.Series.DataCount; i++)
                    AddDataPoint(cartesianTransformer, i);
            }
        }

        private void UpdateVisualVertical(int width, int height, Color color, int leftThickness, int rightThickness, bool isMultiColor, int dataCount)
        {
            float xStart = 0;
            float yStart = 0;
            float xEnd = 0;
            float yEnd = 0;
            Color segmentColor = color;

            for (int i = 0; i < dataCount; i++)
            {
                xStart = xValues[i];
                yStart = y_isInversed ? yLoValues[i] : yHiValues[i];
                xEnd = xValues[i];
                yEnd = y_isInversed ? yHiValues[i] : yLoValues[i];

                var leftOffset = (int)xStart - leftThickness;
                var rightOffset = (int)xEnd + rightThickness;

                if (isSeriesSelected)
                    color = seriesSelectionColor;
                else if (fastHiloSeries.SelectedSegmentsIndexes.Contains(startIndex) && (fastHiloSeries as ISegmentSelectable).SegmentSelectionBrush != null)
                    color = segmentSelectionColor;
                else if (isMultiColor)
                    color = GetColor(fastHiloSeries.ColorModel.GetBrush(startIndex));
                else
                    color = segmentColor;
                startIndex++;

                bitmap.FillRectangle(fastBuffer, width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, color, fastHiloSeries.bitmapPixels);
            }
        }

        private void UpdateVisualHorizontal(int width, int height, Color color, int leftThickness, int rightThickness, bool isMultiColor, int dataCount)
        {
            float xStart = 0;
            float yStart = 0;
            float yEnd = 0;
            Color segmentColor = color;

            for (int i = 0; i < dataCount; i++)
            {
                xStart = xValues[i];
                yStart = y_isInversed ? yLoValues[i] : yHiValues[i];
                yEnd = y_isInversed ? yHiValues[i] : yLoValues[i];
                var leftOffset = xStart - leftThickness;
                var rightOffset = xStart + rightThickness;

                if (isSeriesSelected)
                    color = seriesSelectionColor;
                else if (fastHiloSeries.SelectedSegmentsIndexes.Contains(startIndex) && (fastHiloSeries as ISegmentSelectable).SegmentSelectionBrush != null)
                    color = segmentSelectionColor;
                else if (isMultiColor)
                    color = GetColor(fastHiloSeries.ColorModel.GetBrush(startIndex));
                else
                    color = segmentColor;
                startIndex++;

                bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, color, fastHiloSeries.bitmapPixels);
            }
        }

        #endregion

        #endregion
    }
}
