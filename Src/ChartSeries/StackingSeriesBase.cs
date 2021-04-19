using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for StackingSeriesBase
    /// </summary>
    public abstract partial class StackingSeriesBase : XyDataSeries
    {
        #region Dependency Property Registration
        
        /// <summary>
        ///  The DependencyProperty for <see cref="GroupingLabel"/> property.       .
        /// </summary>
        public static readonly DependencyProperty GroupingLabelProperty =
            DependencyProperty.Register(
                "GroupingLabel",
                typeof(string),
                typeof(StackingSeriesBase),
                new PropertyMetadata(null, OnGroupingLabelChanged));

        #endregion

        #region Fields

        #region Internal Fields

        internal bool stackValueCalculated;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the label to group and stack the similar stacked series types. This is a bindable property. 
        /// </summary>
        public string GroupingLabel
        {
            get { return (string)GetValue(GroupingLabelProperty); }
            set { SetValue(GroupingLabelProperty, value); }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets the start y values collection.
        /// </summary>
        protected internal IList<double> YRangeStartValues { get; set; }

        /// <summary>
        /// Gets or sets the end y values collection.
        /// </summary>
        protected internal IList<double> YRangeEndValues { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Returns the stacked value of the series.
        /// </summary>
        /// <param name="series">ChartSeries</param>
        /// <returns>StackedYValues class instance</returns>       
        public StackingValues GetCumulativeStackValues(ChartSeriesBase series)
        {
            if (Area.StackedValues == null || !Area.StackedValues.Keys.Contains(series))
                CalculateStackingValues();
            if (Area.StackedValues.Keys.Contains(series))
                return Area.StackedValues[series];
            return null;
        }

        #endregion

        #region Public Override Methods

        /// <summary>
        /// Finds the nearest point in ChartSeries relative to the mouse point/touch position.
        /// </summary>
        /// <param name="point">The co-ordinate point representing the current mouse point /touch position.</param>
        /// <param name="x">x-value of the nearest point.</param>
        /// <param name="y">y-value of the nearest point</param>
        /// <param name="stackedYValue"></param>
        public override void FindNearestChartPoint(Point point, out double x, out double y, out double stackedYValue)
        {
            base.FindNearestChartPoint(point, out x, out y, out stackedYValue);
            if (!double.IsNaN(x) && !double.IsNaN(y))
            {
                if (this.ActualXValues is IList<double> && !this.IsIndexed)
                {
                    IList<double> xValues;
                    xValues = this.ActualXValues as IList<double>;
                    stackedYValue = this.GetStackedYValue(xValues.IndexOf(x));
                }
                else
                    stackedYValue = this.GetStackedYValue((int)x);
            }
        }

        #endregion

        #region Internal Override Methods

        internal override void ReValidateYValues(List<int>[] emptyPointIndex)
        {
            base.ReValidateYValues(emptyPointIndex);

            CalculateStackingValues(); // WPF-22116 - Recalculating statcked value once empty point value is reset
        }

        /// <summary>
        /// This method used to get the segment pixel positions
        /// </summary>
        internal override void GeneratePixels()
        {
            if (!IsActualTransposed)
                GenerateStackingColumnPixels();
            else
                GenerateStackingBarPixels();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// This method is used to gets the selected data point segment pixel positions
        /// </summary>
        internal void GenerateStackingColumnPixels()
        {
            WriteableBitmap bmp = Area.fastRenderSurface;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = CreateTransformer(
                new Size(
                    Area.SeriesClipRect.Width,
                    Area.SeriesClipRect.Height),
                    true) as ChartTransform.ChartCartesianTransformer;

            bool x_isInversed = cartesianTransformer.XAxis.IsInversed;
            bool y_isInversed = cartesianTransformer.YAxis.IsInversed;

            double x1ChartVal, x2ChartVal, y1ChartVal, y2ChartVal;
            int i = dataPoint.Index;
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            List<double> xValues = (ActualXValues is List<double>) ? ActualXValues as List<double> : GetXValues();

            if (!this.IsIndexed)
            {
                x1ChartVal = xValues[i] + sbsInfo.Start;
                x2ChartVal = xValues[i] + sbsInfo.End;
                y1ChartVal = YRangeEndValues[i];
                y2ChartVal = YRangeStartValues[i];
            }
            else
            {
                x1ChartVal = i + sbsInfo.Start;
                x2ChartVal = i + sbsInfo.End;
                y1ChartVal = YRangeEndValues[i];
                y2ChartVal = YRangeStartValues[i];
            }

            double x1Val = x_isInversed
                                  ? x2ChartVal
                                  : x1ChartVal;
            double x2Val = x_isInversed
                               ? x1ChartVal
                               : x2ChartVal;
            double y2Val = y_isInversed
                               ? y1ChartVal
                               : y2ChartVal;
            double y1Val = y_isInversed
                               ? y2ChartVal
                               : y1ChartVal;
            Point tlpoint = cartesianTransformer.TransformToVisible(x1Val, y1Val);
            Point rbpoint = cartesianTransformer.TransformToVisible(x2Val, y2Val);
            float x1Value = ((float)tlpoint.X);
            float x2Value = ((float)rbpoint.X);
            float y1Value = ((float)tlpoint.Y);
            float y2Value = ((float)rbpoint.Y);

            float x1 = 0;
            float x2 = 0;
            float y1 = 0;
            float y2 = 0;
            int width = (int)Area.SeriesClipRect.Width;
            int height = (int)Area.SeriesClipRect.Height;
            selectedSegmentPixels.Clear();

            x1 = x1Value;
            x2 = x2Value;
            y1 = y1ChartVal > 0 ? y1Value : y2Value;
            y2 = y1ChartVal > 0 ? y2Value : y1Value;

            var spacingSegment = this as ISegmentSpacing;
            if (spacingSegment != null)
            {
                double spacing = spacingSegment.SegmentSpacing;
                if (spacing > 0 && spacing <= 1)
                {
                    double leftpos = spacingSegment.CalculateSegmentSpacing(spacing, x2, x1);
                    double rightpos = spacingSegment.CalculateSegmentSpacing(spacing, x1, x2);
                    x1 = (float)(leftpos);
                    x2 = (float)(rightpos);
                }
            }

            if (y1 < y2)
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)(x1), (int)y1, (int)x2, (int)y2, selectedSegmentPixels);
            else
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)(x1), (int)y2, (int)x2, (int)y1, selectedSegmentPixels);
        }

        /// <summary>
        /// This method is used to gets the selected data point segment pixel positions
        /// </summary>
        internal void GenerateStackingBarPixels()
        {
            WriteableBitmap bmp = Area.fastRenderSurface;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = CreateTransformer(
                new Size(
                    Area.SeriesClipRect.Width,
                    Area.SeriesClipRect.Height),
                    true) as ChartTransform.ChartCartesianTransformer;

            bool isLogarithmic = cartesianTransformer.XAxis.IsLogarithmic || cartesianTransformer.YAxis.IsLogarithmic;
            bool x_isInversed = cartesianTransformer.XAxis.IsInversed;
            bool y_isInversed = cartesianTransformer.YAxis.IsInversed;

            double x1ChartVal, x2ChartVal, y1ChartVal, y2ChartVal;
            double xStart, xEnd, yStart, yEnd, width, height, left, top;
            int i = dataPoint.Index;
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            List<double> xValues = (ActualXValues is List<double>) ? ActualXValues as List<double> : GetXValues();

            if (!this.IsIndexed)
            {
                x1ChartVal = xValues[i] + sbsInfo.Start;
                x2ChartVal = xValues[i] + sbsInfo.End;
                y1ChartVal = YRangeEndValues[i];
                y2ChartVal = YRangeStartValues[i];
            }
            else
            {
                x1ChartVal = i + sbsInfo.Start;
                x2ChartVal = i + sbsInfo.End;
                y1ChartVal = YRangeEndValues[i];
                y2ChartVal = YRangeStartValues[i];
            }

            xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            xEnd = cartesianTransformer.XAxis.VisibleRange.End;

            yStart = cartesianTransformer.YAxis.VisibleRange.Start;
            yEnd = cartesianTransformer.YAxis.VisibleRange.End;

            width = cartesianTransformer.XAxis.RenderedRect.Height;
            height = cartesianTransformer.YAxis.RenderedRect.Width;

            // WPF-14441 - Calculating Bar Position for the Series  
            left = Area.SeriesClipRect.Right - cartesianTransformer.YAxis.RenderedRect.Right;
            top = Area.SeriesClipRect.Bottom - cartesianTransformer.XAxis.RenderedRect.Bottom;

            Size availableSize = new Size(width, height);
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

            float x1Value = 0, x2Value = 0, y1Value = 0, y2Value = 0;

            // Removed the screen point calculation methods and added the point to value method.
            if (!isLogarithmic)
            {
                double x1Val = x_isInversed
                                   ? x2ChartVal < xEnd ? xEnd : x2ChartVal
                                   : x1ChartVal < xStart ? xStart : x1ChartVal;
                double x2Val = x_isInversed
                                   ? x1ChartVal > xStart ? xStart : x1ChartVal
                                   : x2ChartVal > xEnd ? xEnd : x2ChartVal;

                double y1Val = y_isInversed
                                   ? y2ChartVal > yStart ? yStart : y2ChartVal < yEnd ? yEnd : y2ChartVal
                                   : y1ChartVal > yEnd ? yEnd : y1ChartVal < yStart ? yStart : y1ChartVal;
                double y2Val = y_isInversed
                                   ? y1ChartVal < yEnd ? yEnd : y1ChartVal > yStart ? yStart : y1ChartVal
                                   : y2ChartVal < yStart ? yStart : y2ChartVal > yEnd ? yEnd : y2ChartVal;
                x1Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val));
                x2Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val));
                y1Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y1Val)));
                y2Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y2Val)));
            }
            else
            {
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;

                double logx1 = xBase == 1 ? x1ChartVal : Math.Log(x1ChartVal, xBase);
                double logx2 = xBase == 1 ? x2ChartVal : Math.Log(x2ChartVal, xBase);
                double logy1 = yBase == 1 ? y1ChartVal : Math.Log(y1ChartVal, yBase);
                double logy2 = yBase == 1 ? y2ChartVal : Math.Log(y2ChartVal, yBase);

                double x1Val = x_isInversed ? logx2 < xEnd ? xEnd : logx2 : logx1 < xStart ? xStart : logx1;
                double x2Val = x_isInversed ? logx1 > xStart ? xStart : logx1 : logx2 > xEnd ? xEnd : logx2;

                double y1Val = y_isInversed
                                   ? logy2 > yStart ? yStart : logy2 < yEnd ? yEnd : logy2
                                   : logy1 > yEnd ? yEnd : logy1 < yStart ? yStart : logy1;

                double y2Val = y_isInversed
                                   ? logy1 < yEnd ? yEnd : logy1 > yStart ? yStart : logy1
                                   : logy2 < yStart ? yStart : logy2 > yEnd ? yEnd : logy2;
                x1Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val));
                x2Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val));
                y1Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y1Val)));
                y2Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y2Val)));
            }

            float x1 = 0, x2, y1, y2, diff = 0;

            width = (int)Area.SeriesClipRect.Width;
            height = (int)Area.SeriesClipRect.Height;

            x1 = x1Value;
            x2 = x2Value;
            y1 = y1ChartVal > 0 ? y1Value : y2Value;
            y2 = y1ChartVal > 0 ? y2Value : y1Value;

            var spacingSegment = this as ISegmentSpacing;
            if (spacingSegment != null)
            {
                double spacing = spacingSegment.SegmentSpacing;
                if (spacing > 0 && spacing <= 1)
                {
                    double leftpos = spacingSegment.CalculateSegmentSpacing(spacing, x1, x2);
                    double rightpos = spacingSegment.CalculateSegmentSpacing(spacing, x2, x1);
                    x2 = (float)leftpos;
                    x1 = (float)rightpos;
                }
            }

            diff = x2 - x1;

            selectedSegmentPixels.Clear();

            if (y1 < y2)
                selectedSegmentPixels = bmp.GetRectangle(
                    (int)width, 
                    (int)height, 
                    (int)(width - y2),
                    (int)(height - x1 - diff),
                    (int)(width - y1),
                    (int)(height - x1), 
                    selectedSegmentPixels);
            else
                selectedSegmentPixels = bmp.GetRectangle(
                    (int)width, 
                    (int)height,
                    (int)(width - y1), 
                    (int)(height - x1 - diff),
                    (int)(width - y2),
                    (int)(height - x1), 
                    selectedSegmentPixels);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Return double value from the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected double GetStackedYValue(int index)
        {
            return YRangeEndValues[index];
        }

        #endregion

        #region Private Static Methods

        private static void OnGroupingLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StackingSeriesBase series = (d as StackingSeriesBase);
            if (series != null && series.Area != null && series.ActualArea != null)
            {
                series.ActualArea.SBSInfoCalculated = false;
                series.Area.ScheduleUpdate();
            }
        }

        #endregion

        #region Private Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        private void CalculateStackingValues()
        {
            var isGrouped = ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed;
            Area.StackedValues = new Dictionary<object, StackingValues>();
            var stackingSeries = from series in Area.VisibleSeries
                                 where series is StackingSeriesBase && series.ActualYAxis != null && series.ActualXAxis != null
                                 select series;

            foreach (var series in stackingSeries)
            {
                // To split the series into groups according to their labels.
                var seriesGroups =
                      from seriesGroup in series.ActualYAxis.RegisteredSeries.Intersect(series.ActualXAxis.RegisteredSeries).ToList()
                      where seriesGroup is StackingSeriesBase
                      group seriesGroup by (seriesGroup as StackingSeriesBase).GroupingLabel into groups
                      select new { GroupingPath = groups.Key, Series = groups };

                foreach (var label in seriesGroups)
                {
                    int i = 0;
                    var lastValPos = new List<double>();
                    var lastXValPos = new List<double>();
                    var lastValNeg = new List<double>();
                    bool reCalculation = true;

                    foreach (ChartSeriesBase chartSeries in label.Series)
                    {
                        if (!(chartSeries is StackingSeriesBase)) continue;
                        if (!chartSeries.IsSeriesVisible) continue;
                        if (((StackingSeriesBase)chartSeries).stackValueCalculated) break;

                        var values = new StackingValues();
                        values.StartValues = new List<double>();
                        values.EndValues = new List<double>();
                        IList<double> yValues;
                        yValues = ((XyDataSeries)chartSeries).YValues;
                        List<double> xValues;
                        if (isGrouped)
                            xValues = ((XyDataSeries)chartSeries).GroupedXValuesIndexes;
                        else
                            xValues = ((XyDataSeries)chartSeries).GetXValues();

                        if (chartSeries.ActualXAxis is CategoryAxis && !(chartSeries.ActualXAxis as CategoryAxis).IsIndexed)
                        {
                            if (!(chartSeries is StackingColumn100Series) && !(chartSeries is StackingBar100Series)
                                && !(chartSeries is StackingAreaSeries) && !(chartSeries is StackingArea100Series))
                            {
                                chartSeries.GroupedActualData.Clear();
                                List<double> seriesYValues = new List<double>();
                                for (int m = 0; m < chartSeries.DistinctValuesIndexes.Count; m++)
                                {
                                    if (chartSeries.DistinctValuesIndexes[m].Count > 0)
                                    {
                                        var list = (from index in chartSeries.DistinctValuesIndexes[m]
                                                    where chartSeries.GroupedSeriesYValues[0].Count > index
                                                    select new List<double> { chartSeries.GroupedSeriesYValues[0][index], index }).
                                                    OrderByDescending(val => val[0]).ToList();
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            var yValue = list[n][0];
                                            seriesYValues.Add(yValue);
                                            chartSeries.DistinctValuesIndexes[m][n] = (int)list[n][1];
                                            chartSeries.GroupedActualData.Add(chartSeries.ActualData[(int)list[n][1]]);
                                        }
                                    }
                                }

                                yValues = seriesYValues;
                            }
                            else
                            {
                                yValues = chartSeries.GroupedSeriesYValues[0];
                                chartSeries.GroupedActualData.AddRange(chartSeries.ActualData);
                            }
                        }

                        double origin = ActualXAxis != null ? ActualXAxis.Origin : 0; // Setting origin value for stacking segment

                        int j = 0;
                        if (ActualXAxis != null && ActualXAxis.Origin == 0 && ActualYAxis is LogarithmicAxis &&
                            (ActualYAxis as LogarithmicAxis).Minimum != null)
                            origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;

                        foreach (double yValue in yValues)
                        {
                            double lastValue = 0;
                            double currentValue = yValue;

                            if (lastValPos.Count <= j)
                                lastValPos.Add(0);
                            if (lastValNeg.Count <= j)
                                lastValNeg.Add(0);
                            if (lastXValPos.Count <= j)
                                lastXValPos.Add(0);

                            if (values.StartValues.Count <= j)
                            {
                                values.StartValues.Add(0);
                                values.EndValues.Add(0);
                            }

                            bool checkXValues = false;
                            int xPlotIndex = 0;

                            for (int k = 0; k < lastXValPos.Count; k++)
                            {
                                if (xValues.Count > j)
                                {
                                    if (lastXValPos[k] == xValues[j])
                                    {
                                        xPlotIndex = k;
                                        checkXValues = true;
                                        break;
                                    }
                                }
                            }

                            if (checkXValues)
                            {
                                if (currentValue >= 0)
                                {
                                    lastValue = lastValPos[xPlotIndex];
                                    if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                        currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                    if (!double.IsNaN(lastValPos[xPlotIndex]))
                                        lastValPos[xPlotIndex] += currentValue;
                                    else
                                        lastValPos[xPlotIndex] = currentValue;
                                }
                                else
                                {
                                    lastValue = lastValNeg[xPlotIndex];
                                    if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                        currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                    if (!double.IsNaN(lastValPos[xPlotIndex]))
                                    {
                                        if (!double.IsNaN(currentValue))
                                            lastValNeg[xPlotIndex] += currentValue;
                                    }
                                    else
                                        lastValNeg[xPlotIndex] = currentValue;
                                }
                            }
                            else
                            {
                                if (currentValue >= 0)
                                {
                                    if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                        currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                    lastValPos.Add(currentValue);
                                    lastValNeg.Add(0);
                                }
                                else
                                {
                                    if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series"))
                                        currentValue = Area.GetPercentage((label.Series as IList<ISupportAxes>), currentValue, j, reCalculation);
                                    lastValPos.Add(double.IsNaN(currentValue) ? currentValue : 0);
                                    lastValNeg.Add(currentValue);
                                }

                                if (xValues.Count > j)
                                    lastXValPos.Add(xValues[j]);
                            }

                            values.StartValues[j] = (i == 0 || lastValue < origin) ? origin : lastValue;  // Setting origin value for stacking segment 
                            values.EndValues[j] = currentValue + (double.IsNaN(lastValue) ? origin : lastValue); // Included condition for Empty point support

                            if (values.EndValues[j] < lastValue)
                            {
                                values.StartValues[j] = lastValue + origin;
                            }

                            if (chartSeries.GetType().Name.Contains("Stacking") && chartSeries.GetType().Name.Contains("100Series") && values.EndValues[j] > 100)
                                values.EndValues[j] = 100;
                            j++;
                        }

                        i++;

                        // WP-824 - We add Stacked Values when both start & end values are calculated
                        if (values.StartValues.Count > 0 && values.EndValues.Count > 0)
                        {
                            Area.StackedValues.Add(chartSeries, values);
                            ((StackingSeriesBase)chartSeries).stackValueCalculated = true;
                        }

                        reCalculation = false;
                    }
                }
            }

            foreach (ChartSeriesBase chartSeries in stackingSeries)
            {
                var stackingSeriesBase = chartSeries as StackingSeriesBase;

                if (stackingSeriesBase != null)
                    stackingSeriesBase.stackValueCalculated = false;
            }
        }

        #endregion

        #endregion

        internal override List<object> GetDataPoints(double startX, double endX, double startY, double endY, int minimum, int maximum, List<double> xValues, bool validateYValues)
        {
            List<object> dataPoints = new List<object>();
            if (xValues.Count != YRangeEndValues.Count)
            {
                return null;
            }

            for (int i = minimum; i <= maximum; i++)
            {
                double xValue = xValues[i];
                if (validateYValues || (startX <= xValue && xValue <= endX))
                {
                    double topValue = YRangeEndValues[i];
                    if (startY <= topValue && topValue <= endY)
                    {
                        dataPoints.Add(ActualData[i]);
                    }
                }
            }

            return dataPoints;
        }
    }

    /// <summary>
    /// Class implementation for StackingValues
    /// </summary>
    public partial class StackingValues
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets StartValues property
        /// </summary>
        public IList<double> StartValues { get; set; }

        /// <summary>
        /// Gets or sets EndValues property
        /// </summary>
        public IList<double> EndValues { get; set; }

        #endregion

        #endregion
    }
}
