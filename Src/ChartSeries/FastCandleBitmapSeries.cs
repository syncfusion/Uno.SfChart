using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastCandleBitmapSeries is another version of CandleSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="FastHiLoBitmapSeries"/>
    public partial class FastCandleBitmapSeries : FinancialSeriesBase, ISegmentSpacing, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(FastCandleBitmapSeries),
            new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(FastCandleBitmapSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastCandleBitmapSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Fields

        #region Private Fields

        private bool isFill;

        List<int> selectedBorderPixels = new List<int>();

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Called when instance created for FastCandleBitmapSeries
        /// </summary>
        public FastCandleBitmapSeries()
        {
            DefaultStyleKey = typeof(FastCandleBitmapSeries);
        }

        #endregion

        #region Properties
        
        #region Public Properties

        /// <summary>
        /// Gets or sets the spacing between the segments across the series in cluster mode.
        /// </summary>
        /// <value>
        ///     The value ranges from 0 to 1.
        /// </value>
        public double SegmentSpacing
        {
            get { return (double)GetValue(SegmentSpacingProperty); }
            set { SetValue(SegmentSpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior (brush) for the selected segment(s).
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        /// <example>
        ///     <code>
        ///     series.SegmentSelectionBrush = new SolidColorBrush(Colors.Red);
        ///     </code>
        /// </example>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected segment.
        /// </summary>
        /// <value>
        /// <c>Int</c> value represents the index of the data point(or segment) to be selected. 
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        #endregion

        #region Internal Override Properties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Protected Internal Override Properties

        /// <summary>
        /// Gets a value indicating whether this series is placed side by side.
        /// </summary>
        /// <returns>
        /// It returns <c>true</c>, if the series is placed side by side [cluster mode].
        /// </returns>
        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// This indicates whether its a bitmap series or not.
        /// </summary>
        protected internal override bool IsBitmapSeries
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the selected segments in this series, when we enable the multiple selection.
        /// </summary>
        /// <returns>
        /// It returns <c>List<ChartSegment></c>.
        /// </returns>
        protected internal override List<ChartSegment> SelectedSegments
        {
            get
            {
                if (SelectedSegmentsIndexes.Count > 0)
                {
                    selectedSegments.Clear();
                    foreach (var index in SelectedSegmentsIndexes)
                    {
                        selectedSegments.Add(GetDataPoint(index));
                    }

                    return selectedSegments;
                }
                else
                    return null;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Interface Methods

        double ISegmentSpacing.CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            return CalculateSegmentSpacing(spacing, Right, Left);
        }
        
        #endregion

        #region Public OVerride Methods

        /// <summary>
        /// Creates the segments of FastCandleBitmapSeries
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = null;
            var isGrouped = (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed);

            if (isGrouped)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();

            if (xValues != null)
            {
                if (isGrouped)
                {
                    Segments.Clear();
                    Adornments.Clear();
                    double center = this.GetSideBySideInfo(this).Median;

                    if (AdornmentsInfo != null)
                    {
                        for (int i = 0; i < xValues.Count; i++)
                        {
                            if (i < xValues.Count && GroupedSeriesYValues[0].Count > i)
                            {
                                xValues[i] += center;
                                ChartPoint hipoint = new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]);
                                ChartPoint lopoint = new ChartPoint(xValues[i], GroupedSeriesYValues[1][i]);
                                ChartPoint oppoint = new ChartPoint(xValues[i], GroupedSeriesYValues[2][i]);
                                ChartPoint clpoint = new ChartPoint(xValues[i], GroupedSeriesYValues[3][i]);
                                AddAdornments(xValues[i], hipoint, lopoint, oppoint, oppoint, clpoint, clpoint, i, 0);
                            }
                        }
                    }

                    if (Segment == null || Segments.Count == 0)
                    {
                        Segment = new FastCandleBitmapSegment(xValues, GroupedSeriesYValues[2], GroupedSeriesYValues[3],
                            GroupedSeriesYValues[0], GroupedSeriesYValues[1], this);
                        Segment.Series = this;
                        Segment.Item = ActualData;
                        Segment.SetData(xValues, GroupedSeriesYValues[2], GroupedSeriesYValues[3],
                            GroupedSeriesYValues[0], GroupedSeriesYValues[1]);
                        this.Segments.Add(Segment);
                    }
                }
                else
                {
                    ClearUnUsedSegments(this.DataCount);

                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 4);
                        else
                            ClearUnUsedAdornments(this.DataCount * 2);

                        double center = this.GetSideBySideInfo(this).Median;

                        for (int i = 0; i < this.DataCount; i++)
                        {
                            xValues[i] += center;
                            ChartPoint hipoint = new ChartPoint(xValues[i], HighValues[i]);
                            ChartPoint lopoint = new ChartPoint(xValues[i], LowValues[i]);
                            ChartPoint oppoint = new ChartPoint(xValues[i], OpenValues[i]);
                            ChartPoint clpoint = new ChartPoint(xValues[i], CloseValues[i]);
                            AddAdornments(xValues[i], hipoint, lopoint, oppoint, oppoint, clpoint, clpoint, i, 0);
                        }
                    }

                    if (Segment == null || Segments.Count == 0)
                    {
                        Segment = new FastCandleBitmapSegment(xValues, OpenValues, CloseValues, HighValues, LowValues, this);
                        Segment.Series = this;
                        Segment.Item = ActualData;
                        Segment.SetData(xValues, OpenValues, CloseValues, HighValues, LowValues);
                        this.Segments.Add(Segment);
                    }
                    else
                    {
                        (Segment as FastCandleBitmapSegment).Item = ActualData;
                        (Segment as FastCandleBitmapSegment).SetData(xValues, OpenValues, CloseValues, HighValues, LowValues);
                    }
                }
            }
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// This method used to get the chart data index at an SfChart co-ordinates
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal override int GetDataPointIndex(Point point)
        {
            Canvas canvas = Area.GetAdorningCanvas();
            double left = Area.ActualWidth - canvas.ActualWidth;
            double top = Area.ActualHeight - canvas.ActualHeight;
            ChartDataPointInfo data = null;
            point.X = point.X - left + Area.Margin.Left;
            point.Y = point.Y - top + Area.Margin.Top;

            Point mousePos = new Point(point.X - Area.SeriesClipRect.Left, point.Y - Area.SeriesClipRect.Top);

            double currentBitmapPixel = (Area.fastRenderSurface.PixelWidth * (int)mousePos.Y + (int)mousePos.X);

            if (!Area.isBitmapPixelsConverted)
                Area.ConvertBitmapPixels();

            if (Pixels.Contains((int)currentBitmapPixel))
                data = GetDataPoint(point);

            if (data != null)
                return data.Index;
            else
                return -1;
        }

        internal override void SelectedSegmentsIndexes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int oldIndex = PreviousSelectedIndex;

                        int newIndex = (int)e.NewItems[0];

                        // For adornment selection implementation
                        if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                        {
                            dataPoint = GetDataPoint(newIndex);

                            if (dataPoint != null && SegmentSelectionBrush != null)
                            {
                                // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                                if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                                {
                                    UpdateAdornmentSelection(newIndex);
                                }

                                // Generate pixels for the particular data point
                                if (Segments.Count > 0) GeneratePixels();

                                // Set the SegmentSelectionBrush to the selected segment pixels
                                OnBitmapSelection(selectedSegmentPixels, SegmentSelectionBrush, true);
                            }

                            // trigger the SelectionChanged event
                            if (ActualArea != null && Segments.Count > 0)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = Segments[0],
                                    SelectedSegments = Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    NewPointInfo = GetDataPoint(newIndex),
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                                if (oldIndex != -1)
                                {
                                    chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                                    chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                                }

                                (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                                PreviousSelectedIndex = newIndex;
                            }
                            else if (Segments.Count == 0)
                            {
                                triggerSelectionChangedEventOnLoad = true;
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int newIndex = (int)e.OldItems[0];

                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = null,
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = null,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = PreviousSelectedIndex,
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = this,
                            IsSelected = false
                        };

                        if (PreviousSelectedIndex != -1)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(PreviousSelectedIndex);
                        }

                            (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        OnResetSegment(newIndex);
                        PreviousSelectedIndex = newIndex;
                    }

                    break;
            }
        }

        internal override void OnResetSegment(int index)
        {
            if (index >= 0)
            {
                dataPoint = GetDataPoint(index);

                if (dataPoint != null)
                {
                    // Resets the adornment selection when the mouse pointer moved away from the adornment or clicked the same adornment.
                    if (adornmentInfo != null)
                        AdornmentPresenter.ResetAdornmentSelection(index, false);

                    if (SegmentSelectionBrush != null)
                    {
                        // Generate pixels for the particular data point
                        if (Segments.Count > 0) GeneratePixels();

                        // Reset the segment pixels
                        if (this.ComparisonMode == FinancialPrice.None || isFill)
                            OnBitmapSelection(selectedSegmentPixels, null, false);
                        else
                            OnBitmapHollowSelection(selectedSegmentPixels, selectedBorderPixels);
                        selectedSegmentPixels.Clear();
                        selectedBorderPixels.Clear();
                        dataPoint = null;
                    }
                }
            }
        }

        /// <summary>
        /// This method used to get the chart data at an index.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        internal override void GeneratePixels()
        {
            WriteableBitmap bmp = Area.fastRenderSurface;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = CreateTransformer(new Size(Area.SeriesClipRect.Width,
                Area.SeriesClipRect.Height), true) as ChartTransform.ChartCartesianTransformer;

            double xStart, xEnd, yStart, yEnd, xSize, ySize, xOffset, yOffset;

            bool x_isInversed = cartesianTransformer.XAxis.IsInversed;
            bool y_isInversed = cartesianTransformer.YAxis.IsInversed;
            xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            xEnd = cartesianTransformer.XAxis.VisibleRange.End;
            yStart = cartesianTransformer.YAxis.VisibleRange.Start;
            yEnd = cartesianTransformer.YAxis.VisibleRange.End;

            if (IsActualTransposed)
            {
                ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                yOffset = cartesianTransformer.YAxis.RenderedRect.Left - Area.SeriesClipRect.Left;
                xOffset = cartesianTransformer.XAxis.RenderedRect.Top - Area.SeriesClipRect.Top;
            }
            else
            {
                ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                yOffset = cartesianTransformer.YAxis.RenderedRect.Top - Area.SeriesClipRect.Top;
                xOffset = cartesianTransformer.XAxis.RenderedRect.Left - Area.SeriesClipRect.Left;
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

            DoubleRange sbsInfo = GetSideBySideInfo(this);
            double x1Val, x2Val, openVal, closeVal, yHiVal, yLoVal, yHiVal1, yLoVal1;
            double sbsCenter = sbsInfo.Median;
            double sbsStart = sbsInfo.Start;
            double sbsEnd = sbsInfo.End;
            double tempOpenVal;

            double xChartValue = dataPoint.XData;
            double openChartValue = dataPoint.Open;
            double closeChartValue = dataPoint.Close;
            double highChartValue = dataPoint.High;
            double lowChartValue = dataPoint.Low;
            int i = dataPoint.Index;
            float x1Value, x2Value, xValue, openValue, closeValue, highValue, lowValue, highValue1, lowValue1;

            var alignedValues = Segments[0].AlignHiLoSegment(openChartValue, closeChartValue, highChartValue, lowChartValue);
            highChartValue = alignedValues[0];
            lowChartValue = alignedValues[1];
            isFill = openChartValue > closeChartValue;

            if (IsIndexed)
            {
                if (!IsActualTransposed)
                {
                    x1Val = !x_isInversed ? (sbsStart + i) : (sbsEnd + i);
                    x2Val = !x_isInversed ? (sbsEnd + i) : (sbsStart + i);
                    openVal = y_isInversed
                                 ? closeChartValue
                                 : openChartValue;
                    closeVal = y_isInversed
                                   ? openChartValue
                                   : closeChartValue;
                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                    }

                    yHiVal = highChartValue;
                    yLoVal = lowChartValue;
                    if (openChartValue > closeChartValue)
                    {
                        yHiVal1 = openChartValue;
                        yLoVal1 = closeChartValue;
                    }
                    else
                    {
                        yHiVal1 = closeChartValue;
                        yLoVal1 = openChartValue;
                    }

                    Point blpoint = cartesianTransformer.TransformToVisible(x1Val, openVal);
                    Point trpoint = cartesianTransformer.TransformToVisible(x2Val, closeVal);
                    Point point1 = cartesianTransformer.TransformToVisible(i + sbsCenter, yHiVal);
                    Point point2 = cartesianTransformer.TransformToVisible(i + sbsCenter, yLoVal);
                    Point point3 = cartesianTransformer.TransformToVisible(i + sbsCenter, yHiVal1);
                    Point point4 = cartesianTransformer.TransformToVisible(i + sbsCenter, yLoVal1);
                    xValue = (float)point1.X;
                    x1Value = (float)blpoint.X;
                    x2Value = (float)trpoint.X;
                    openValue = (float)blpoint.Y;
                    closeValue = (float)trpoint.Y;
                    highValue = (float)point1.Y;
                    lowValue = (float)point2.Y;
                    highValue1 = (float)point3.Y;
                    lowValue1 = (float)point4.Y;
                }
                else
                {
                    double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                    double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;

                    x1Val = !x_isInversed ? (sbsStart + i) : (sbsEnd + i);
                    x2Val = !x_isInversed ? (sbsEnd + i) : (sbsStart + i);
                    openVal = y_isInversed
                                 ? closeChartValue
                                 : openChartValue;
                    closeVal = y_isInversed
                                   ? openChartValue
                                   : closeChartValue;
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

                    yHiVal = highChartValue;
                    yLoVal = lowChartValue;

                    if (openChartValue > closeChartValue)
                    {
                        yHiVal1 = openChartValue;
                        yLoVal1 = closeChartValue;
                    }
                    else
                    {
                        yHiVal1 = closeChartValue;
                        yLoVal1 = openChartValue;
                    }

                    Point point1 = cartesianTransformer.TransformToVisible(i + sbsCenter, yHiVal);
                    Point point2 = cartesianTransformer.TransformToVisible(i + sbsCenter, yLoVal);
                    Point point3 = cartesianTransformer.TransformToVisible(i + sbsCenter, yHiVal1);
                    Point point4 = cartesianTransformer.TransformToVisible(i + sbsCenter, yLoVal1);
                    xValue = (float)point1.Y;
                    x1Value = (float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val));
                    x2Value = (float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val));
                    openValue = (float)(yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(openVal)));
                    closeValue = (float)(yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(closeVal)));
                    highValue = (float)point1.X;
                    lowValue = (float)point2.X;
                    highValue1 = (float)point3.X;
                    lowValue1 = (float)point4.X;
                }
            }
            else
            {
                if (!IsActualTransposed)
                {
                    x1Val = x_isInversed ? (xChartValue + sbsEnd) : (xChartValue + sbsStart);
                    x2Val = x_isInversed ? (xChartValue + sbsStart) : (xChartValue + sbsEnd);
                    openVal = y_isInversed
                                  ? closeChartValue
                                  : openChartValue;
                    closeVal = y_isInversed
                                   ? openChartValue
                                   : closeChartValue;
                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                    }

                    yHiVal = highChartValue;
                    yLoVal = lowChartValue;

                    if (openChartValue > closeChartValue)
                    {
                        yHiVal1 = openChartValue;
                        yLoVal1 = closeChartValue;
                    }
                    else
                    {
                        yHiVal1 = closeChartValue;
                        yLoVal1 = openChartValue;
                    }

                    Point blpoint = cartesianTransformer.TransformToVisible(x1Val, openVal);
                    Point trpoint = cartesianTransformer.TransformToVisible(x2Val, closeVal);
                    Point point1 = cartesianTransformer.TransformToVisible(xChartValue + sbsCenter, yHiVal);
                    Point point2 = cartesianTransformer.TransformToVisible(xChartValue + sbsCenter, yLoVal);
                    Point point3 = cartesianTransformer.TransformToVisible(i + sbsCenter, yHiVal1);
                    Point point4 = cartesianTransformer.TransformToVisible(i + sbsCenter, yLoVal1);
                    xValue = (float)point1.X;
                    x1Value = (float)blpoint.X;
                    x2Value = (float)trpoint.X;
                    openValue = (float)blpoint.Y;
                    closeValue = (float)trpoint.Y;
                    highValue = (float)point1.Y;
                    lowValue = (float)point2.Y;
                    highValue1 = (float)point3.Y;
                    lowValue1 = (float)point4.Y;
                }
                else
                {
                    double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                    double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;

                    x1Val = x_isInversed ? (xChartValue + sbsEnd) : (xChartValue + sbsStart);
                    x2Val = x_isInversed ? (xChartValue + sbsStart) : (xChartValue + sbsEnd);
                    openVal = y_isInversed
                                   ? closeChartValue
                                   : openChartValue;
                    closeVal = y_isInversed
                                   ? openChartValue
                                   : closeChartValue;
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

                    yHiVal = highChartValue;
                    yLoVal = lowChartValue;

                    if (openChartValue > closeChartValue)
                    {
                        yHiVal1 = openChartValue;
                        yLoVal1 = closeChartValue;
                    }
                    else
                    {
                        yHiVal1 = closeChartValue;
                        yLoVal1 = openChartValue;
                    }

                    Point point1 = cartesianTransformer.TransformToVisible(xChartValue + sbsCenter, yHiVal);
                    Point point2 = cartesianTransformer.TransformToVisible(xChartValue + sbsCenter, yLoVal);
                    Point point3 = cartesianTransformer.TransformToVisible(i + sbsCenter, yHiVal1);
                    Point point4 = cartesianTransformer.TransformToVisible(i + sbsCenter, yLoVal1);
                    xValue = (float)point1.Y;
                    x1Value = (float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val));
                    x2Value = (float)(xOffset + (xSize) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val));
                    openValue = (float)(yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(openVal)));
                    closeValue = (float)(yOffset + (ySize) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(closeVal)));
                    highValue = (float)point1.X;
                    lowValue = (float)point2.X;
                    highValue1 = (float)point3.X;
                    lowValue1 = (float)point4.X;
                }
            }

            double spacing = (this as ISegmentSpacing).SegmentSpacing;
            int width = (int)Area.SeriesClipRect.Width;
            int height = (int)Area.SeriesClipRect.Height;

            int leftThickness = (int)StrokeThickness / 2;
            int rightThickness = (int)(StrokeThickness % 2 == 0.0
                ? (StrokeThickness / 2) : (StrokeThickness / 2) + 1);

            float x, x1, x2, open, close, high, low, high1, low1;

            x = xValue;
            x1 = x1Value;
            x2 = x2Value;
            open = openValue;
            close = closeValue;
            high = y_isInversed ? lowValue : highValue;
            low = y_isInversed ? highValue : lowValue;
            high1 = y_isInversed ? lowValue1 : highValue1;
            low1 = y_isInversed ? highValue1 : lowValue1;
            var leftOffset = x - leftThickness;
            var rightOffset = x + rightThickness;

            if (spacing > 0 && spacing <= 1)
            {
                double leftpos = (this as ISegmentSpacing).CalculateSegmentSpacing(spacing, x2, x1);
                double rightpos = (this as ISegmentSpacing).CalculateSegmentSpacing(spacing, x1, x2);
                x1 = (float)(leftpos);
                x2 = (float)(rightpos);
            }

            selectedSegmentPixels.Clear();
            selectedBorderPixels.Clear();
            close = open == close ? close + 1 : close;

            if (!IsActualTransposed)
            {
                if (this.ComparisonMode != FinancialPrice.None)
                {
                    selectedBorderPixels = bmp.GetDrawRectangle(width, height, (int)(x1), (int)open, (int)x2, (int)close, selectedBorderPixels);
                    selectedBorderPixels = bmp.GetRectangle(width, height, (int)leftOffset, (int)high, (int)rightOffset, (int)high1, selectedBorderPixels);
                    selectedBorderPixels = bmp.GetRectangle(width, height, (int)leftOffset, (int)low1, (int)rightOffset, (int)low, selectedBorderPixels);
                }

                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)(x1), (int)open, (int)x2, (int)close, selectedSegmentPixels);
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)leftOffset, (int)high, (int)rightOffset, (int)high1, selectedSegmentPixels);
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)leftOffset, (int)low1, (int)rightOffset, (int)low, selectedSegmentPixels);
            }
            else
            {
                if (this.ComparisonMode != FinancialPrice.None)
                {
                    selectedBorderPixels = bmp.GetDrawRectangle((int)width, (int)height, (int)(width - close), (int)(height - x2),
                     (int)(width - open), (int)(height - x1), selectedBorderPixels);
                    selectedBorderPixels = bmp.GetRectangle(width, height, (int)high, (int)(leftOffset), (int)high1, (int)(rightOffset), selectedBorderPixels);
                    selectedBorderPixels = bmp.GetRectangle(width, height, (int)low1, (int)(leftOffset), (int)low, (int)(rightOffset), selectedBorderPixels);
                }

                selectedSegmentPixels = bmp.GetRectangle((int)width, (int)height, (int)(width - close), (int)(height - x2),
                        (int)(width - open), (int)(height - x1), selectedSegmentPixels);
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)high, (int)(leftOffset), (int)high1, (int)(rightOffset), selectedSegmentPixels);
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)low1, (int)(leftOffset), (int)low, (int)(rightOffset), selectedSegmentPixels);
            }
        }

        #endregion
        
        #region Protected Internal Override Methods

        /// <summary>
        /// Method used to set SegmentSelectionBrush to SelectedIndex segment
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="oldIndex"></param>
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            if (ActualArea != null && ActualArea.SelectionBehaviour != null && !ActualArea.GetEnableSeriesSelection())
            {
                // Reset the old segment
                if (ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                {
                    if (SelectedSegmentsIndexes.Contains(oldIndex))
                        SelectedSegmentsIndexes.Remove(oldIndex);

                    OnResetSegment(oldIndex);
                }

                if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                {
                    if (!SelectedSegmentsIndexes.Contains(newIndex))
                        SelectedSegmentsIndexes.Add(newIndex);

                    dataPoint = GetDataPoint(newIndex);

                    if (dataPoint != null && SegmentSelectionBrush != null)
                    {
                        // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                        if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                        {
                            UpdateAdornmentSelection(newIndex);
                        }

                        // Generate pixels for the particular data point
                        if (Segments.Count > 0) GeneratePixels();

                        // Set the SegmentSelectionBrush to the selected segment pixels
                        OnBitmapSelection(selectedSegmentPixels, SegmentSelectionBrush, true);
                    }

                    // trigger the SelectionChanged event
                    if (ActualArea != null && Segments.Count > 0)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = Segments[0],
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            NewPointInfo = GetDataPoint(newIndex),
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                        if (oldIndex != -1)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                        }

                        (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        PreviousSelectedIndex = newIndex;
                    }
                    else if (Segments.Count == 0)
                    {
                        triggerSelectionChangedEventOnLoad = true;
                    }
                }
                else if (newIndex == -1 && ActualArea != null)
                {
                    chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                    {
                        SelectedSegment = null,
                        SelectedSegments = Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        PreviousSelectedSegment = null,
                        PreviousSelectedSeries = this,
                        IsSelected = false
                    };

                    if (oldIndex != -1)
                    {
                        chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                        chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                    }

                    (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                    PreviousSelectedIndex = newIndex;
                }
            }
            else if (newIndex >= 0 && Segments.Count == 0)
            {
                triggerSelectionChangedEventOnLoad = true;
            }
        }

        #endregion

        #region Protected Override Methods

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (ShowTooltip)
            {
                Canvas canvas = ActualArea.GetAdorningCanvas();
                mousePos = e.GetCurrentPoint(canvas).Position;
                ChartDataPointInfo dataPoint = new ChartDataPointInfo();
                int index = ChartExtensionUtils.GetAdornmentIndex(e.OriginalSource);
                if (index > -1)
                {
                    if (ActualXAxis is CategoryAxis && (!(ActualXAxis as CategoryAxis).IsIndexed))
                    {
                        if (GroupedSeriesYValues[0].Count > index)
                            dataPoint.High = GroupedSeriesYValues[0][index];

                        if (GroupedSeriesYValues[1].Count > index)
                            dataPoint.Low = GroupedSeriesYValues[1][index];

                        if (GroupedSeriesYValues[2].Count > index)
                            dataPoint.Open = GroupedSeriesYValues[2][index];

                        if (GroupedSeriesYValues[3].Count > index)
                            dataPoint.Close = GroupedSeriesYValues[3][index];
                    }
                    else
                    {
                        if (ActualSeriesYValues[0].Count > index)
                            dataPoint.High = ActualSeriesYValues[0][index];

                        if (ActualSeriesYValues[1].Count > index)
                            dataPoint.Low = ActualSeriesYValues[1][index];

                        if (ActualSeriesYValues[2].Count > index)
                            dataPoint.Open = ActualSeriesYValues[2][index];

                        if (ActualSeriesYValues[3].Count > index)
                            dataPoint.Close = ActualSeriesYValues[3][index];
                    }

                    dataPoint.Index = index;
                    dataPoint.Series = this;

                    if (ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                    UpdateSeriesTooltip(dataPoint);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastCandleBitmapSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                SegmentSpacing = this.SegmentSpacing,
            });
        }

        #endregion

        #region Protected Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected double CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            double diff = Right - Left;
            double totalspacing = diff * spacing / 2;
            Left = Left + totalspacing;
            Right = Right - totalspacing;
            return Left;
        }

        #endregion

        #region Private Static Methods

        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as FastCandleBitmapSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }
        
        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastCandleBitmapSeries).UpdateArea();
        }
        
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.ActualArea == null || series.ActualArea.SelectionBehaviour == null) return;
            if (series.ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                series.SelectedIndexChanged((int)e.NewValue, (int)e.OldValue);
            else if ((int)e.NewValue != -1)
                series.SelectedSegmentsIndexes.Add((int)e.NewValue);
        }

        #endregion

        #region Private Methods
                
        private void OnBitmapHollowSelection(List<int> SegmentPixels, List<int> BorderPixels)
        {
            if (SegmentPixels != null && SegmentPixels.Count > 0)
            {
                int seriesIndex = Area.Series.IndexOf(this);
                if (!Area.isBitmapPixelsConverted)
                    Area.ConvertBitmapPixels();

                // Gets the upper series from the selected series
                var upperSeriesCollection = (from series in Area.Series
                                             where Area.Series.IndexOf(series) > seriesIndex
                                             select series).ToList();

                // Gets the upper series pixels in to single collection
                foreach (var series in upperSeriesCollection)
                {
                    upperSeriesPixels.UnionWith(series.Pixels);
                }

                {
                    byte[] buffer = Area.GetFastBuffer();
                    int j = 0;
                    Color SegmentColor = Colors.Transparent;
                    Color BorderColor = ((Segments[0] as FastCandleBitmapSegment).GetSegmentBrush(dataPoint.Index));

                    foreach (var pixel in SegmentPixels)
                    {
                        if (Pixels.Contains(pixel) && !upperSeriesPixels.Contains(pixel))
                        {
                            buffer[pixel] = SegmentColor.A;
                        }
                    }

                    foreach (var pixel1 in BorderPixels)
                    {
                        if (Pixels.Contains(pixel1) && !upperSeriesPixels.Contains(pixel1))
                        {
                            if (j == 0)
                            {
                                buffer[pixel1] = BorderColor.B;
                                j = j + 1;
                            }
                            else if (j == 1)
                            {
                                buffer[pixel1] = BorderColor.G;
                                j = j + 1;
                            }
                            else if (j == 2)
                            {
                                buffer[pixel1] = BorderColor.R;
                                j = j + 1;
                            }
                            else if (j == 3)
                            {
                                buffer[pixel1] = BorderColor.A;
                                j = 0;
                            }
                        }
                    }

                    Area.RenderToBuffer();
                }

                upperSeriesPixels.Clear();
            }
        }

        #endregion

        #endregion
    }
}
