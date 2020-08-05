using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastHiLoOpenCloseBitmapSeries is another version of HiLoOpenCloseSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="FastHiLoBitmapSeries"/>   
    public partial class FastHiLoOpenCloseBitmapSeries : FinancialSeriesBase, ISegmentSpacing, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSpacing"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(FastHiLoOpenCloseBitmapSeries),
            new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush),
            typeof(FastHiLoOpenCloseBitmapSeries), new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastHiLoOpenCloseBitmapSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Properties

        #region Public Properties

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

        #endregion

        #region Internal Properties

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

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of FastHiLoOpenCloseBitmapSeries
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = null;
            var isGrouped = ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed;

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

                    if (Segment == null || Segments.Count == 0)
                    {
                        FastHiLoOpenCloseSegment segment = new FastHiLoOpenCloseSegment(xValues, GroupedSeriesYValues[0], GroupedSeriesYValues[1], GroupedSeriesYValues[2], GroupedSeriesYValues[3], this);
                        segment.Series = this;
                        segment.Item = ActualData;
                        segment.SetData(xValues, GroupedSeriesYValues[0], GroupedSeriesYValues[1], GroupedSeriesYValues[2], GroupedSeriesYValues[3]);
                        Segment = segment;
                        this.Segments.Add(Segment);
                    }

                    DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                    double median = sbsInfo.Delta / 2;
                    double center = sbsInfo.Median;
                    double Left = sbsInfo.Start;
                    double Right = sbsInfo.End;
                    double leftpos = (this as ISegmentSpacing).CalculateSegmentSpacing(SegmentSpacing, Right, Left);
                    double rightpos = (this as ISegmentSpacing).CalculateSegmentSpacing(SegmentSpacing, Left, Right);

                    if (SegmentSpacing > 0 && SegmentSpacing <= 1)
                    {
                        Left = leftpos;
                        Right = rightpos;
                    }

                    for (int i = 0; i < xValues.Count; i++)
                    {
                        if (i < xValues.Count && GroupedSeriesYValues[0].Count > i)
                        {
                            ChartPoint highPt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[0][i]);
                            ChartPoint lowPt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[1][i]);
                            ChartPoint startOpenPt = new ChartPoint(xValues[i] + Left, GroupedSeriesYValues[2][i]);
                            ChartPoint endOpenPt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[2][i]);
                            ChartPoint startClosePt = new ChartPoint(xValues[i] + Right, GroupedSeriesYValues[3][i]);
                            ChartPoint endClosePt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[3][i]);

                            if (AdornmentsInfo != null)
                                AddAdornments(xValues[i], highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, i, median);
                        }
                    }
                }
                else
                {
                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 4);
                        else
                            ClearUnUsedAdornments(this.DataCount * 2);
                    }

                    if (Segment == null || Segments.Count == 0)
                    {
                        FastHiLoOpenCloseSegment segment = new FastHiLoOpenCloseSegment(xValues, HighValues, LowValues, OpenValues, CloseValues, this);
                        segment.Series = this;
                        segment.Item = ActualData;
                        segment.SetData(xValues, HighValues, LowValues, OpenValues, CloseValues);

                        Segment = segment;
                        this.Segments.Add(Segment);
                    }
                    else if (xValues != null)
                    {
                        (Segment as FastHiLoOpenCloseSegment).Item = ActualData;
                        (Segment as FastHiLoOpenCloseSegment).SetData(xValues, HighValues, LowValues, OpenValues, CloseValues);
                    }

                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 4);
                        else
                            ClearUnUsedAdornments(this.DataCount * 2);
                    }

                    DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                    double median = sbsInfo.Delta / 2;
                    double center = sbsInfo.Median;
                    double Left = sbsInfo.Start;
                    double Right = sbsInfo.End;
                    double leftpos = (this as ISegmentSpacing).CalculateSegmentSpacing(SegmentSpacing, Right, Left);
                    double rightpos = (this as ISegmentSpacing).CalculateSegmentSpacing(SegmentSpacing, Left, Right);

                    if (SegmentSpacing > 0 && SegmentSpacing <= 1)
                    {
                        Left = leftpos;
                        Right = rightpos;
                    }

                    for (int i = 0; i < this.DataCount; i++)
                    {
                        ChartPoint highPt = new ChartPoint(xValues[i] + center, HighValues[i]);
                        ChartPoint lowPt = new ChartPoint(xValues[i] + center, LowValues[i]);
                        ChartPoint startOpenPt = new ChartPoint(xValues[i] + Left, OpenValues[i]);
                        ChartPoint endOpenPt = new ChartPoint(xValues[i] + center, OpenValues[i]);
                        ChartPoint startClosePt = new ChartPoint(xValues[i] + Right, CloseValues[i]);
                        ChartPoint endClosePt = new ChartPoint(xValues[i] + center, CloseValues[i]);

                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, i, median);
                    }
                }
            }
        }

        #endregion

        #region Internal Override Methods

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
                                    PreviousSelectedSeries = null,
                                    PreviousSelectedSegment = null,
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
                        OnBitmapSelection(selectedSegmentPixels, null, false);

                        selectedSegmentPixels.Clear();
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
        internal override void GeneratePixels()
        {
            WriteableBitmap bmp = Area.fastRenderSurface;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = CreateTransformer(new Size(Area.SeriesClipRect.Width,
                Area.SeriesClipRect.Height),
                true) as ChartTransform.ChartCartesianTransformer;

            bool x_isInversed = cartesianTransformer.XAxis.IsInversed;
            bool y_isInversed = cartesianTransformer.YAxis.IsInversed;

            DoubleRange sbsInfo = GetSideBySideInfo(this);
            double center = sbsInfo.Median;
            double Left = sbsInfo.Start;
            double Right = sbsInfo.End;

            double xChartValue = dataPoint.XData;
            double yOpenChartValue = dataPoint.Open;
            double yCloseChartValue = dataPoint.Close;
            double yHighChartValue = dataPoint.High;
            double yLowChartValue = dataPoint.Low;
            int i = dataPoint.Index;
            float xValue, yHiValue, yLoValue, yOpenStartValue, yOpenEndValue, yCloseValue, yCloseEndValue;

            double spacing = (this as ISegmentSpacing).SegmentSpacing;
            double leftpos = (this as ISegmentSpacing).CalculateSegmentSpacing(spacing, Right, Left);
            double rightpos = (this as ISegmentSpacing).CalculateSegmentSpacing(spacing, Left, Right);

            if (spacing > 0 && spacing <= 1)
            {
                Left = leftpos;
                Right = rightpos;
            }

            double highValue = yHighChartValue;
            double lowValue = yLowChartValue;
            var alignedValues = Segments[0].AlignHiLoSegment(yOpenChartValue, yCloseChartValue, highValue, lowValue);
            highValue = alignedValues[0];
            lowValue = alignedValues[1];

            if (IsIndexed)
            {
                Point hiPoint = cartesianTransformer.TransformToVisible(i + center, highValue);
                Point loPoint = cartesianTransformer.TransformToVisible(xChartValue + center, lowValue);
                Point startopenpoint = cartesianTransformer.TransformToVisible(xChartValue + center, yOpenChartValue);
                Point endopenpoint = cartesianTransformer.TransformToVisible(xChartValue + Left, yOpenChartValue);
                Point endclosepoint = cartesianTransformer.TransformToVisible(i + Right, yCloseChartValue);

                if (!IsActualTransposed)
                {
                    xValue = (float)hiPoint.X;
                    yHiValue = (float)hiPoint.Y;
                    yLoValue = (float)loPoint.Y;
                    yOpenStartValue = (float)startopenpoint.Y;
                    yOpenEndValue = (float)endopenpoint.X;
                    yCloseValue = (float)endclosepoint.Y;
                    yCloseEndValue = (float)endclosepoint.X;
                }
                else
                {
                    xValue = (float)hiPoint.Y;
                    yHiValue = (float)hiPoint.X;
                    yLoValue = (float)loPoint.X;
                    yOpenStartValue = (float)startopenpoint.X;
                    yOpenEndValue = (float)endopenpoint.Y;
                    yCloseValue = (float)endclosepoint.X;
                    yCloseEndValue = (float)endclosepoint.Y;
                }
            }
            else
            {
                Point hiPoint = cartesianTransformer.TransformToVisible(xChartValue + center, highValue);
                Point loPoint = cartesianTransformer.TransformToVisible(xChartValue + center, lowValue);
                Point startopenpoint = cartesianTransformer.TransformToVisible(xChartValue + center, yOpenChartValue);
                Point endopenpoint = cartesianTransformer.TransformToVisible(xChartValue + Left, yOpenChartValue);
                Point endclosepoint = cartesianTransformer.TransformToVisible(xChartValue + Right, yCloseChartValue);

                if (!IsActualTransposed)
                {
                    xValue = (float)hiPoint.X;
                    yHiValue = (float)hiPoint.Y;
                    yLoValue = (float)loPoint.Y;
                    yOpenStartValue = (float)startopenpoint.Y;
                    yOpenEndValue = (float)endopenpoint.X;
                    yCloseValue = (float)endclosepoint.Y;
                    yCloseEndValue = (float)endclosepoint.X;
                }
                else
                {
                    xValue = (float)hiPoint.Y;
                    yHiValue = (float)hiPoint.X;
                    yLoValue = (float)loPoint.X;
                    yOpenStartValue = (float)startopenpoint.X;
                    yOpenEndValue = (float)endopenpoint.Y;
                    yCloseValue = (float)endclosepoint.X;
                    yCloseEndValue = (float)endclosepoint.Y;
                }
            }

            int width = (int)Area.SeriesClipRect.Width;
            int height = (int)Area.SeriesClipRect.Height;
            int leftThickness = (int)StrokeThickness / 2;
            int rightThickness = (int)(StrokeThickness % 2 == 0
                ? (StrokeThickness / 2) : StrokeThickness / 2 + 1);

            selectedSegmentPixels.Clear();

            float xStart = 0;
            float yStart = 0;
            float yEnd = 0;
            float yOpen = 0;
            float yOpenEnd = 0;
            float yClose = 0;
            float yCloseEnd = 0;
            int leftOffset, rightOffset;

            xStart = xValue;
            yStart = y_isInversed ? yLoValue : yHiValue;
            yEnd = y_isInversed ? yHiValue : yLoValue;
            yOpen = x_isInversed ? yCloseValue : yOpenStartValue;
            yOpenEnd = x_isInversed ? yCloseEndValue : yOpenEndValue;
            yClose = x_isInversed ? yOpenStartValue : yCloseValue;
            yCloseEnd = x_isInversed ? yOpenEndValue : yCloseEndValue;

            if (!IsActualTransposed)
            {
                leftOffset = (int)xStart - leftThickness;
                rightOffset = (int)xStart + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, leftOffset, (int)yStart, rightOffset, (int)yEnd, selectedSegmentPixels);
                leftOffset = (int)yOpen - leftThickness;
                rightOffset = (int)yOpen + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)yOpenEnd, leftOffset, (int)xStart - leftThickness, (int)rightOffset, selectedSegmentPixels);
                leftOffset = (int)yClose - leftThickness;
                rightOffset = (int)yClose + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)xStart + leftThickness, leftOffset, (int)yCloseEnd, (int)rightOffset, selectedSegmentPixels);
            }
            else
            {
                leftOffset = (int)xStart - leftThickness;
                rightOffset = (int)xStart + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, selectedSegmentPixels);
                leftOffset = (int)yOpen - leftThickness;
                rightOffset = (int)yOpen + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, leftOffset, (int)xStart + leftThickness, (int)rightOffset, (int)yOpenEnd, selectedSegmentPixels);
                leftOffset = (int)yClose - leftThickness;
                rightOffset = (int)yClose + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, leftOffset, (int)yCloseEnd, (int)rightOffset, (int)xStart - leftThickness, selectedSegmentPixels);
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
                            PreviousSelectedSeries = null,
                            PreviousSelectedSegment = null,
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
                    if ((ActualXAxis is CategoryAxis) && (!(ActualXAxis as CategoryAxis).IsIndexed))
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
            return base.CloneSeries(new FastHiLoOpenCloseBitmapSeries()
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
            var series = d as FastHiLoOpenCloseBitmapSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }
        
        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastHiLoOpenCloseBitmapSeries).UpdateArea();
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
        
        #endregion
    }
}
