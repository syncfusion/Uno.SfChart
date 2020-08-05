using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the fast scatter elements that use a WriteableBitmap to define their appearance. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.XyDataSeries" />
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ISegmentSelectable" />
    public partial class FastScatterBitmapSeries : XyDataSeries, ISegmentSelectable
    {
        #region Dependency Property Regisrtation
        
        /// <summary>
        /// The DependencyProperty for <see cref="ScatterWidth"/> property. 
        /// </summary>
        public static readonly DependencyProperty ScatterWidthProperty =
            DependencyProperty.Register("ScatterWidth", typeof(double), typeof(FastScatterBitmapSeries),
            new PropertyMetadata(3d, OnScatterWidthChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ScatterHeight"/> property. 
        /// </summary>
        public static readonly DependencyProperty ScatterHeightProperty =
            DependencyProperty.Register("ScatterHeight", typeof(double), typeof(FastScatterBitmapSeries),
            new PropertyMetadata(3d, OnScatterHeightChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(FastScatterBitmapSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastScatterBitmapSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Symbol"/> property.
        /// </summary>
        public static readonly DependencyProperty ShapeTypeProperty =
            DependencyProperty.Register(
                "ShapeType",
                typeof(ChartSymbol),
                typeof(FastScatterBitmapSeries),
                new PropertyMetadata(ChartSymbol.Ellipse, OnShapeTypePropertyChanged));

        #endregion

        #region Fields

        #region Private Fields

        private IList<double> xValues;

        private Point hitPoint = new Point();

        private Point startPoint = new Point();

        private Point endPoint = new Point();

        bool isAdornmentsBending;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that specifies the width of the FastScatterBitmap segment. This is a bindable property.
        /// </summary>
        /// <remarks>
        /// The default value is 3.
        /// </remarks>
        public double ScatterWidth
        {
            get { return (double)GetValue(ScatterWidthProperty); }
            set { SetValue(ScatterWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the height of the FastScatterBitmap segment. This is a bindable property.
        /// </summary>
        /// <remarks>
        /// The default value is 3.
        /// </remarks>
        public double ScatterHeight
        {
            get { return (double)GetValue(ScatterHeightProperty); }
            set { SetValue(ScatterHeightProperty, value); }
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

        /// <summary>
        /// Gets or sets different types of shapes in a fast scatter bitmap series.
        /// </summary>
        /// <value>This property takes fast scatter shape value, and its default shape type is ellipse.
        /// </value>
        /// <remarks>
        /// Fast scatter bitmap series does not support Custom, HorizontalLine and VerticalLine shapes.
        /// By using the above shapes for fast scatter bitmap series, you can render only the default type, which is ellipse. 
        /// </remarks>
        public ChartSymbol ShapeType
        {
            get { return (ChartSymbol)GetValue(ShapeTypeProperty); }
            set { SetValue(ShapeTypeProperty, value); }
        }

        #endregion

        #region Protected Internal Properties

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

        private FastScatterBitmapSegment Segment { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of FastScatterBitmapSeries
        /// </summary>
        public override void CreateSegments()
        {
            bool isGrouping = this.ActualXAxis is CategoryAxis && !(this.ActualXAxis as CategoryAxis).IsIndexed;
            if (isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            if (isGrouping)
            {
                Segments.Clear();
                Adornments.Clear();

                if (Segments == null || Segments.Count == 0)
                {
                    Segment = new FastScatterBitmapSegment(xValues, GroupedSeriesYValues[0], this);
                    Segments.Add(Segment);
                }
            }
            else
            {
                ClearUnUsedAdornments(this.DataCount);
                if (Segments == null || Segments.Count == 0)
                {
                    Segment = new FastScatterBitmapSegment(xValues, YValues, this);
                    Segments.Add(Segment);
                }
                else if (ActualXValues != null)
                {
                    Segment.SetData(xValues, YValues);
                    (Segment as FastScatterBitmapSegment).SetRange();
                    Segment.Item = ActualData;
                }
            }

            isAdornmentsBending = true;
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// Method used to return the hittest series while mouse action.
        /// </summary>
        /// <returns></returns>
        internal override bool IsHitTestSeries()
        {
            // Gets the current mouse position chart data point
            ChartDataPointInfo datapoint = GetDataPoint(Area.adorningCanvasPoint);

            if (datapoint != null)
                return true;

            return false;
        }

        internal override void SelectedSegmentsIndexes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && ActualArea.SelectionBehaviour.SelectionStyle != SelectionStyle.Single)
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
                    if (e.OldItems != null && ActualArea.SelectionBehaviour.SelectionStyle != SelectionStyle.Single)
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
        /// <returns></returns>
        internal override void GeneratePixels()
        {
            WriteableBitmap bmp = Area.fastRenderSurface;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = CreateTransformer(new Size(Area.SeriesClipRect.Width,
                Area.SeriesClipRect.Height),
                true) as ChartTransform.ChartCartesianTransformer;

            double xChartValue = dataPoint.XData;
            double yChartValue = dataPoint.YData;
            int i = dataPoint.Index;
            double xValue, yValue;

            if (IsIndexed)
            {
                bool isGrouped = ActualXAxis is CategoryAxis && !((ActualXAxis as CategoryAxis).IsIndexed);
                if (!IsActualTransposed)
                {
                    double xVal = (isGrouped) ? xChartValue : i;
                    double yVal = yChartValue;
                    Point point = cartesianTransformer.TransformToVisible(xVal, yVal);
                    xValue = point.X;
                    yValue = point.Y;
                }
                else
                {
                    double xVal = (isGrouped) ? xChartValue : i;
                    double yVal = yChartValue;
                    Point point = cartesianTransformer.TransformToVisible(xVal, yVal);
                    xValue = point.Y;
                    yValue = point.X;
                }
            }
            else
            {
                if (!IsActualTransposed)
                {
                    double xVal = xChartValue;
                    double yVal = yChartValue;
                    Point point = cartesianTransformer.TransformToVisible(xVal, yVal);
                    xValue = point.X;
                    yValue = point.Y;
                }
                else
                {
                    double xVal = xChartValue;
                    double yVal = yChartValue;
                    Point point = cartesianTransformer.TransformToVisible(xVal, yVal);
                    xValue = point.Y;
                    yValue = point.X;
                }
            }

            double xr = ScatterHeight, yr = ScatterWidth;
            int width = (int)Area.SeriesClipRect.Width;
            int height = (int)Area.SeriesClipRect.Height;
            selectedSegmentPixels.Clear();

            if (IsActualTransposed)
            {
                if (yValue > -1)
                {
                    selectedSegmentPixels = bmp.GetEllipseCentered(height, width, (int)yValue, (int)xValue, (int)xr, (int)yr, selectedSegmentPixels);
                }
            }
            else
            {
                if (yValue > -1)
                {
                    selectedSegmentPixels = bmp.GetEllipseCentered(height, width, (int)xValue, (int)yValue, (int)xr, (int)yr, selectedSegmentPixels);
                }
            }
        }

        /// <summary>
        /// This method used to gets the chart data point at a position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            bool isGrouped = ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed;
            if (isGrouped)
                xValues = GroupedXValuesIndexes;
            else
                xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            hitPoint.X = mousePos.X - this.Area.SeriesClipRect.Left;
            hitPoint.Y = mousePos.Y - this.Area.SeriesClipRect.Top;

            hitPoint.X = hitPoint.X - ScatterWidth;
            hitPoint.Y = hitPoint.Y - ScatterHeight;

            startPoint.X = ActualArea.PointToValue(ActualXAxis, hitPoint);
            startPoint.Y = ActualArea.PointToValue(ActualYAxis, hitPoint);

            hitPoint.X = hitPoint.X + (2 * ScatterWidth);
            hitPoint.Y = hitPoint.Y + (2 * ScatterHeight);

            endPoint.X = ActualArea.PointToValue(ActualXAxis, hitPoint);
            endPoint.Y = ActualArea.PointToValue(ActualYAxis, hitPoint);

            Rect rect = new Rect(startPoint, endPoint);

            dataPoint = null;

            for (int i = 0; i < YValues.Count; i++)
            {
                if (isGrouped)
                {
                    if (i < xValues.Count)
                    {
                        hitPoint.X = xValues[i];
                        hitPoint.Y = GroupedSeriesYValues[0][i];
                    }
                    else
                        return dataPoint;
                }
                else
                {
                    hitPoint.X = IsIndexed ? i : xValues[i];
                    hitPoint.Y = YValues[i];
                }

                if (rect.Contains(hitPoint))
                {
                    dataPoint = new ChartDataPointInfo();
                    dataPoint.Index = i;
                    dataPoint.XData = xValues[i];
                    dataPoint.YData = (isGrouped) ?
                                      GroupedSeriesYValues[0][i] : YValues[i];
                    dataPoint.Series = this;
                    if (i > -1 && ActualData.Count > i)
                        dataPoint.Item = ActualData[i];
                    break;
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

        protected override void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
            bool isGrouped = (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed);
            if (AdornmentsInfo != null && isAdornmentsBending)
            {
                List<double> xValues = null;
                if (isGrouped)
                    xValues = GroupedXValuesIndexes;
                else
                    xValues = GetXValues();
                if (xValues != null && ActualXAxis != null
                    && !ActualXAxis.VisibleRange.IsEmpty)
                {
                    for (int i = 0; i < DataCount; i++)
                    {
                        if (isGrouped)
                        {
                            if (i < xValues.Count)
                            {
                                AddAdornments(xValues[i], GroupedSeriesYValues[0][i], i);
                            }
                            else
                                return;
                        }
                        else
                            AddAdornments(xValues[i], YValues[i], i);
                    }
                }

                isAdornmentsBending = false;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return
                base.CloneSeries(new FastScatterBitmapSeries()
                {
                    ScatterHeight = this.ScatterHeight,
                    ScatterWidth = this.ScatterWidth,
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
                    dataPoint.Index = index;

                    if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed
                       && GroupedXValuesIndexes.Count > index)
                    {
                        dataPoint.XData = GroupedXValuesIndexes[index];
                        dataPoint.YData = GroupedSeriesYValues[0][index];
                    }
                    else
                    {
                        if (xValues.Count > index)
                            dataPoint.XData = xValues[index];
                        if (YValues.Count > index)
                            dataPoint.YData = YValues[index];
                    }

                    dataPoint.Series = this;
                    if (ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                    UpdateSeriesTooltip(dataPoint);
                }
            }
        }

        #endregion

        #region Private Static Method

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastScatterBitmapSeries).UpdateArea();
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

        private static void OnScatterWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FastScatterBitmapSeries series = d as FastScatterBitmapSeries;
            if (series != null)
                series.UpdateArea();
        }

        private static void OnScatterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FastScatterBitmapSeries series = d as FastScatterBitmapSeries;
            if (series != null)
                series.UpdateArea();
        }

        private static void OnShapeTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FastScatterBitmapSeries series = d as FastScatterBitmapSeries;

            if (series == null)
            {
                return;
            }

            if (series.ShapeType == ChartSymbol.Custom || series.ShapeType == ChartSymbol.HorizontalLine || series.ShapeType == ChartSymbol.VerticalLine)
            {
                series.ShapeType = ChartSymbol.Ellipse;
            }

            if (series.LegendIcon == ChartLegendIcon.SeriesType)
            {
                series.UpdateLegendIconTemplate(true);
            }

            series.UpdateArea();

        }

        #endregion

        #region Private Methods

        private void AddAdornments(double x, double yValue, int i)
        {
            double adornX = 0d, adornY = 0d;
            adornX = x;
            adornY = yValue;
            if (i < Adornments.Count)
            {
                Adornments[i].SetData(adornX, adornY, adornX, adornY);
            }
            else
            {
                Adornments.Add(this.CreateAdornment(this, adornX, adornY, adornX, adornY));
            }

            Adornments[i].Item = ActualData[i];
        }

        #endregion

        #endregion
    }
}
