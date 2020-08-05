using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastColumnBitmapSeries is another version of ColumnSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="FastHiLoBitmapSeries"/>
    public partial class FastColumnBitmapSeries : XyDataSeries, ISegmentSpacing, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(FastColumnBitmapSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastColumnBitmapSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(FastColumnBitmapSeries),
            new PropertyMetadata(0.0, OnSegmentSpacingChanged));
        
        #endregion
        
        #region Fields

        #region Private Fields

        private List<double> xValues = null;

        #endregion

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

        #region Protected Internal Properties

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

        #region Private Properties

        private ChartSegment Segment { get; set; }

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
        /// Creates the segments of FastColumnBitmapSeries
        /// </summary>
        public override void CreateSegments()
        {
            var isGrouped = (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed);
            if (isGrouped)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            IList<double> x1Values, x2Values, y1Values, y2Values;
            x1Values = new List<double>();
            x2Values = new List<double>();
            y1Values = new List<double>();
            y2Values = new List<double>();
            x1Values.Clear();
            x2Values.Clear();
            y1Values.Clear();
            y2Values.Clear();

            if (xValues != null)
            {
                ClearUnUsedAdornments(this.DataCount);
                DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                double origin = ActualXAxis != null ? ActualXAxis.Origin : 0;
                if (ActualXAxis != null && ActualXAxis.Origin == 0 && ActualYAxis is LogarithmicAxis &&
                    (ActualYAxis as LogarithmicAxis).Minimum != null)
                    origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;

                if (isGrouped)
                {
                    Segments.Clear();
                    Adornments.Clear();
                    GroupedActualData.Clear();

                    for (int i = 0; i < DistinctValuesIndexes.Count; i++)
                    {
                        var list = (from index in DistinctValuesIndexes[i]
                                    where GroupedSeriesYValues[0].Count > index
                                    select new List<double> { GroupedSeriesYValues[0][index], index }).
                              OrderByDescending(val => val[0]).ToList();

                        for (int j = 0; j < list.Count; j++)
                        {
                            var yValue = list[j][0];
                            GroupedActualData.Add(ActualData[(int)list[j][1]]);
                            if (i < xValues.Count)
                            {
                                x1Values.Add(i + sbsInfo.Start);
                                x2Values.Add(i + sbsInfo.End);
                                y1Values.Add(yValue);
                                y2Values.Add(origin);  // setting origin value for fastcolumn segment
                            }
                        }
                    }

                    if (Segment != null && (IsActualTransposed && Segment is FastColumnBitmapSegment)
                             || (!IsActualTransposed && Segment is FastBarBitmapSegment))
                        Segments.Clear();

                    if (Segment == null || Segments.Count == 0)
                    {
                        if (IsActualTransposed)
                            Segment = new FastBarBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                        else
                            Segment = new FastColumnBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                        Segment.SetData(x1Values, y1Values, x2Values, y2Values);
                        this.Segments.Add(Segment);
                    }

                    if (AdornmentsInfo != null)
                    {
                        int count = 0;
                        for (int i = 0; i < DistinctValuesIndexes.Count; i++)
                        {
                            var list = (from index in DistinctValuesIndexes[i]
                                        select new List<double> { GroupedSeriesYValues[0][index], index }).
                               OrderByDescending(val => val[0]).ToList();
                            for (int j = 0; j < DistinctValuesIndexes[i].Count; j++)
                            {
                                var yValue = list[j][0];
                                if (i < xValues.Count)
                                {
                                    if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                        AddColumnAdornments(i, yValue, x1Values[count], y1Values[count], count, sbsInfo.Delta / 2);
                                    else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                        AddColumnAdornments(i, yValue, x1Values[count], y2Values[count], count, sbsInfo.Delta / 2);
                                    else
                                        AddColumnAdornments(i, yValue, x1Values[count], y1Values[count] + (y2Values[count] - y1Values[count]) / 2, count, sbsInfo.Delta / 2);
                                }

                                count++;
                            }
                        }
                    }
                }
                else
                {
                    if (!this.IsIndexed)
                    {
                        ClearUnUsedAdornments(this.DataCount);
                        for (int i = 0; i < this.DataCount; i++)
                        {
                            x1Values.Add(xValues[i] + sbsInfo.Start);
                            x2Values.Add(xValues[i] + sbsInfo.End);
                            y1Values.Add(YValues[i]);
                            y2Values.Add(origin);  // setting origin value for fastcolumn segment
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.DataCount; i++)
                        {
                            x1Values.Add(i + sbsInfo.Start);
                            x2Values.Add(i + sbsInfo.End);
                            y1Values.Add(YValues[i]);
                            y2Values.Add(origin);  // Setting origin value for fastcolumn segment
                        }
                    }

                    if (Segment != null && (IsActualTransposed && Segment is FastColumnBitmapSegment)
                              || (!IsActualTransposed && Segment is FastBarBitmapSegment))
                        Segments.Clear();

                    if (Segment == null || Segments.Count == 0)
                    {
                        if (IsActualTransposed)
                            Segment = new FastBarBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                        else
                            Segment = new FastColumnBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                        Segment.SetData(x1Values, y1Values, x2Values, y2Values);
                        this.Segments.Add(Segment);
                    }
                    else
                    {
                        if (Segment is FastBarBitmapSegment)
                        {
                            (Segment as FastBarBitmapSegment).Item = ActualData;
                            (Segment as FastBarBitmapSegment).SetData(x1Values, y1Values, x2Values, y2Values);
                        }
                        else
                        {
                            (Segment as FastColumnBitmapSegment).Item = ActualData;
                            (Segment as FastColumnBitmapSegment).SetData(x1Values, y1Values, x2Values, y2Values);
                        }
                    }

                    if (AdornmentsInfo != null)
                    {
                        for (int i = 0; i < this.DataCount; i++)
                        {
                            if (i < this.DataCount)
                            {
                                if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                    AddColumnAdornments(xValues[i], YValues[i], x1Values[i], y1Values[i], i, sbsInfo.Delta / 2);
                                else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                    AddColumnAdornments(xValues[i], YValues[i], x1Values[i], y2Values[i], i, sbsInfo.Delta / 2);
                                else
                                    AddColumnAdornments(xValues[i], YValues[i], x1Values[i], y1Values[i] + (y2Values[i] - y1Values[i]) / 2, i, sbsInfo.Delta / 2);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// Method used to return the hittest series while mouse action.
        /// </summary>
        /// <returns></returns>
        internal override bool IsHitTestSeries()
        {
            var point = new Point(Area.adorningCanvasPoint.X - this.ActualArea.SeriesClipRect.Left,
                Area.adorningCanvasPoint.Y - this.ActualArea.SeriesClipRect.Top);

            foreach (var rect in bitmapRects)
            {
                if (rect.Contains(point))
                    return true;
            }

            return false;
        }

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
                        OnBitmapSelection(selectedSegmentPixels, null, false);

                        selectedSegmentPixels.Clear();
                        dataPoint = null;
                    }
                }
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
                else if (newIndex == -1)
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
                    if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                    {
                        if (xValues.Count > index)
                            dataPoint.XData = GroupedXValuesIndexes[index];
                        if (GroupedSeriesYValues[0].Count > index)
                            dataPoint.YData = GroupedSeriesYValues[0][index];

                        if (GroupedActualData.Count > index)
                            dataPoint.Item = GroupedActualData[index];
                    }
                    else
                    {
                        if (xValues.Count > index)
                            dataPoint.XData = xValues[index];
                        if (YValues.Count > index)
                            dataPoint.YData = YValues[index];

                        if (ActualData.Count > index)
                            dataPoint.Item = ActualData[index];
                    }

                    dataPoint.Index = index;
                    dataPoint.Series = this;
                    UpdateSeriesTooltip(dataPoint);
                }
            }
        }

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            Segment = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastColumnBitmapSeries()
            {
                SelectedIndex = this.SelectedIndex,
                SegmentSpacing = this.SegmentSpacing,
                SegmentSelectionBrush = this.SegmentSelectionBrush
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

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastColumnBitmapSeries).UpdateArea();
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

        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as FastColumnBitmapSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }

        #endregion
        
        #endregion
    }
}
