using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastHiLoSeries is another version of HiLoSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="FastHiLoOpenCloseBitmapSeries"/>   
    public partial class FastHiLoBitmapSeries : RangeSeriesBase, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(FastHiLoBitmapSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastHiLoBitmapSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Properties

        #region Public Properties

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

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of FastHiLoSeries.
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
                        FastHiLoSegment segment = new FastHiLoSegment(xValues as IList<double>, GroupedSeriesYValues[0], GroupedSeriesYValues[1], this);
                        segment.Series = this;
                        segment.Item = ActualData;
                        segment.SetData(xValues as IList<double>, GroupedSeriesYValues[0], GroupedSeriesYValues[1]);
                        Segment = segment;
                        this.Segments.Add(segment);
                    }

                    double center = this.GetSideBySideInfo(this).Median;
                    for (int i = 0; i < xValues.Count; i++)
                    {
                        if (i < xValues.Count)
                        {
                            xValues[i] += center;
                            if (AdornmentsInfo != null && GroupedSeriesYValues[0].Count > i)
                                AddAdornments(xValues[i], 0, GroupedSeriesYValues[0][i], GroupedSeriesYValues[1][i], i);
                        }
                    }
                }
                else
                {
                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 2);
                        else
                            ClearUnUsedAdornments(this.DataCount);
                    }

                    if (Segment == null || Segments.Count == 0)
                    {
                        FastHiLoSegment segment = new FastHiLoSegment(xValues as IList<double>, HighValues, LowValues, this);
                        segment.Series = this;
                        segment.Item = ActualData;
                        segment.SetData(xValues as IList<double>, HighValues, LowValues);
                        Segment = segment;
                        this.Segments.Add(segment);
                    }
                    else if (xValues != null)
                    {
                        (Segment as FastHiLoSegment).Item = this.ActualData;
                        (Segment as FastHiLoSegment).SetData(xValues as IList<double>, HighValues, LowValues);
                    }

                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 2);
                        else
                            ClearUnUsedAdornments(this.DataCount);
                    }

                    double center = this.GetSideBySideInfo(this).Median;

                    for (int i = 0; i < this.DataCount; i++)
                    {
                        xValues[i] += center;
                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], 0, HighValues[i], LowValues[i], i);
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
                        OnBitmapSelection(selectedSegmentPixels, null, false);

                        selectedSegmentPixels.Clear();
                        dataPoint = null;
                    }
                }
            }
        }

        /// <summary>
        /// This method used to gets the segment pixel positions at data point.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override void GeneratePixels()
        {
            WriteableBitmap bmp = Area.fastRenderSurface;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = CreateTransformer(new Size(Area.SeriesClipRect.Width,
                Area.SeriesClipRect.Height),
                true) as ChartTransform.ChartCartesianTransformer;

            bool y_isInversed = cartesianTransformer.YAxis.IsInversed;

            double xChartValue = dataPoint.XData;
            double yHiChartValue = dataPoint.High;
            double yLoChartValue = dataPoint.Low;
            int i = dataPoint.Index;
            float xValue, yHiValue, yLoValue;

            if (IsIndexed)
            {
                if (!IsActualTransposed)
                {
                    Point hipoint = cartesianTransformer.TransformToVisible(i, yHiChartValue);
                    Point lopoint = cartesianTransformer.TransformToVisible(i, yLoChartValue);
                    xValue = (float)hipoint.X;
                    yHiValue = (float)hipoint.Y;
                    yLoValue = (float)lopoint.Y;
                }
                else
                {
                    Point hipoint = cartesianTransformer.TransformToVisible(i, yHiChartValue);
                    Point lopoint = cartesianTransformer.TransformToVisible(i, yLoChartValue);
                    xValue = (float)hipoint.Y;
                    yHiValue = (float)hipoint.X;
                    yLoValue = (float)lopoint.X;
                }
            }
            else
            {
                if (!IsActualTransposed)
                {
                    Point hipoint = cartesianTransformer.TransformToVisible(xChartValue, yHiChartValue);
                    Point lopoint = cartesianTransformer.TransformToVisible(xChartValue, yLoChartValue);
                    xValue = (float)hipoint.X;
                    yHiValue = (float)hipoint.Y;
                    yLoValue = (float)lopoint.Y;
                }
                else
                {
                    Point hipoint = cartesianTransformer.TransformToVisible(xChartValue, yHiChartValue);
                    Point lopoint = cartesianTransformer.TransformToVisible(xChartValue, yLoChartValue);
                    xValue = (float)hipoint.Y;
                    yHiValue = (float)hipoint.X;
                    yLoValue = (float)lopoint.X;
                }
            }

            int width = (int)Area.SeriesClipRect.Width;
            int height = (int)Area.SeriesClipRect.Height;
            int leftThickness = (int)StrokeThickness / 2;
            int rightThickness = (int)(StrokeThickness % 2 == 0
                ? (StrokeThickness / 2) : StrokeThickness / 2 + 1);

            float xStart = xValue;
            float yStart = y_isInversed ? yLoValue : yHiValue;
            float xEnd = xValue;
            float yEnd = y_isInversed ? yHiValue : yLoValue;
            var leftOffset = xStart - leftThickness;

            selectedSegmentPixels.Clear();

            if (!IsActualTransposed)
            {
                var rightOffset = xStart + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, selectedSegmentPixels);
            }
            else
            {
                var rightOffset = (int)xEnd + rightThickness;
                selectedSegmentPixels = bmp.GetRectangle(width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, selectedSegmentPixels);
            }
        }

        /// <summary>
        /// This method used to get the chart data at a mouse position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            double xVal = 0;
            double yVal = 0;
            double stackedYValue = double.NaN;
            dataPoint = null;
            dataPoint = new ChartDataPointInfo();

            if (this.Area.SeriesClipRect.Contains(mousePos))
            {
                var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left, mousePos.Y - this.Area.SeriesClipRect.Top);

                this.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);

                dataPoint.XData = xVal;
                int index = this.GetXValues().IndexOf(xVal);
                if (index > -1)
                {
                    dataPoint.High = ActualSeriesYValues[0][index];
                    dataPoint.Low = ActualSeriesYValues[1][index];
                    dataPoint.Index = index;
                    dataPoint.Series = this;
                    if (index > -1 && ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                }
            }

            return dataPoint;
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

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastHiLoBitmapSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
            });
        }

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
                    }
                    else
                    {
                        if (ActualSeriesYValues[0].Count > index)
                            dataPoint.High = ActualSeriesYValues[0][index];
                        if (ActualSeriesYValues[1].Count > index)
                            dataPoint.Low = ActualSeriesYValues[1][index];
                    }

                    dataPoint.Index = index;
                    dataPoint.Series = this;
                    if (ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                    UpdateSeriesTooltip(dataPoint);
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastHiLoBitmapSeries).UpdateArea();
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
