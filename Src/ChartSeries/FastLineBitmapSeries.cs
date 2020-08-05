using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastLineBitmapSeries is another version of LineSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineBitmapSegment"/>
    /// <seealso cref="FastLineSeries"/>
    /// <seealso cref="LineSeries"/>    
    public partial class FastLineBitmapSeries : XyDataSeries, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableAntiAliasing"/> property. 
        /// </summary>
        public static readonly DependencyProperty EnableAntiAliasingProperty =
            DependencyProperty.Register("EnableAntiAliasing", typeof(bool), typeof(FastLineBitmapSeries),
            new PropertyMetadata(false, OnSeriesPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashArray"/> property. 
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection),
            typeof(FastLineBitmapSeries), new PropertyMetadata(null, OnSeriesPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(FastLineBitmapSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastLineBitmapSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Fields

        #region Private Fields

        private IList<double> xValues;

        private Point hitPoint = new Point();

        bool isAdornmentPending;

        Polygon polygon = new Polygon();

        PointCollection polygonPoints = new PointCollection();

        #endregion

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
        /// Gets or sets a value that indicates whether to enable anti-aliasing for <see cref="FastLineBitmapSeries"/>, to draw smooth edges.
        /// </summary>
        public bool EnableAntiAliasing
        {
            get { return (bool)GetValue(EnableAntiAliasingProperty); }
            set { SetValue(EnableAntiAliasingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash array for the line stroke.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Shapes.StrokeDashArray"/>.
        /// </value>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
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

        #region Protected Internal Override Properties

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
        /// The property confirms the linearity of this series.
        /// </summary>
        /// <remarks>
        ///  Returns <c>true</c> if its linear, otherwise it returns <c>false</c>.
        /// </remarks>
        protected internal override bool IsLinear
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

        private FastLineBitmapSegment Segment { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of FastLineBitmapSeries.
        /// </summary>
        public override void CreateSegments()
        {
            var isGrouped = ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed;

            if (isGrouped)
                xValues = GroupedXValuesIndexes;
            else
                xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            if (isGrouped)
            {
                Segments.Clear();
                Adornments.Clear();

                if (Segment == null || Segments.Count == 0)
                {
                    FastLineBitmapSegment segment = new FastLineBitmapSegment(xValues, GroupedSeriesYValues[0], this);
                    Segment = segment;
                    Segments.Add(segment);
                }
            }
            else
            {
                ClearUnUsedAdornments(this.DataCount);
                if (Segment == null || Segments.Count == 0)
                {
                    FastLineBitmapSegment segment = new FastLineBitmapSegment(xValues, YValues, this);
                    Segment = segment;
                    Segments.Add(segment);
                }
                else if (ActualXValues != null)
                {
                    Segment.SetData(xValues, YValues);
                    (Segment as FastLineBitmapSegment).SetRange();
                    Segment.Item = ActualData;
                }
            }

            isAdornmentPending = true;
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// Method used to return the hittest series while mouse action.
        /// </summary>
        /// <returns></returns>
        internal override bool IsHitTestSeries()
        {
            var point = Area.adorningCanvasPoint;

            int low = 0;
            int high = DataCount - 1;
            var xValues = GetXValues();
            var yValues = YValues;
            double xValue = ActualXAxis.PointToValue(point);

            // Binary search algorithm
            while (low <= high)
            {
                int mid = (low + high) / 2;
                if (xValue < xValues[mid])
                    high = mid - 1;
                else if (xValue > xValues[mid])
                    low = mid + 1;
                else if (xValue == xValues[mid])
                {
                    return false;
                }
            }

            if (high > -1 && low > -1 && high < xValues.Count && low < xValues.Count)
            {
                double x1 = ActualXAxis.ValueToPoint(xValues[high]);
                double y1 = ActualYAxis.ValueToPoint(yValues[high]);

                double x2 = ActualXAxis.ValueToPoint(xValues[low]);
                double y2 = ActualYAxis.ValueToPoint(yValues[low]);

                if (Math.Abs(x1 - x2) > 2)
                {
                    double thickness = StrokeThickness / 2;
                    polygon.Points = GetPolygonPoints(x1, y1, x2, y2, thickness, thickness);

                    if (PointInsidePolygon(polygon, point.X, point.Y))
                        return true;
                }
                else if (Math.Round(y1) == Math.Round(point.Y) || Math.Round(y2) == Math.Round(point.Y))
                    return true;
            }

            return false;
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
                            // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                            if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                            {
                                UpdateAdornmentSelection(newIndex);
                            }

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
            if (index >= 0 && adornmentInfo != null)
            {
                AdornmentPresenter.ResetAdornmentSelection(index, false);
            }
        }

        /// <summary>
        /// This method used to gets the chart data point at a position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            Rect rect;
            int startIndex, endIndex;
            List<int> hitIndexes = new List<int>();
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            CalculateHittestRect(mousePos, out startIndex, out endIndex, out rect);

            for (int i = startIndex; i <= endIndex; i++)
            {
                hitPoint.X = IsIndexed ? i : xValues[i];
                hitPoint.Y = YValues[i];

                if (rect.Contains(hitPoint))
                    hitIndexes.Add(i);
            }

            if (hitIndexes.Count > 0)
            {
                int i = hitIndexes[hitIndexes.Count / 2];
                hitIndexes = null;

                dataPoint = new ChartDataPointInfo();
                dataPoint.Index = i;
                dataPoint.XData = xValues[i];
                dataPoint.YData = YValues[i];
                dataPoint.Series = this;

                if (i > -1 && ActualData.Count > i)
                    dataPoint.Item = ActualData[i];

                return dataPoint;
            }
            else
                return dataPoint;
        }

        internal override void UpdateTooltip(object originalSource)
        {
            if (ShowTooltip)
            {
                ChartDataPointInfo dataPoint = new ChartDataPointInfo();
                int index = ChartExtensionUtils.GetAdornmentIndex(originalSource);
                if (index > -1)
                {
                    dataPoint.Index = index;
                    if (xValues.Count > index)
                        dataPoint.XData = xValues[index];
                    if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed
                            && GroupedSeriesYValues[0].Count > index)
                        dataPoint.YData = GroupedSeriesYValues[0][index];
                    else if (YValues.Count > index)
                        dataPoint.YData = YValues[index];
                    dataPoint.Series = this;
                    if (ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                    UpdateSeriesTooltip(dataPoint);
                }
            }
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method used to trigger SelectionChanged event to SelectedIndex segment
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="oldIndex"></param>
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            if (ActualArea != null && ActualArea.SelectionBehaviour != null)
            {
                // Resets the adornment selection when the mouse pointer moved away from the adornment or clicked the same adornment.
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

                    // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                    if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                    {
                        UpdateAdornmentSelection(newIndex);
                    }

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

        /// <summary>
        /// Called when VisibleRange property changed
        /// </summary>
        protected override void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
            if (AdornmentsInfo != null && isAdornmentPending)
            {
                List<double> xValues = new List<double>();
                if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                    xValues = GroupedXValuesIndexes;
                else
                    xValues = GetXValues();

                if (xValues != null && ActualXAxis != null
                    && !ActualXAxis.VisibleRange.IsEmpty)
                {
                    double xBase = ActualXAxis.IsLogarithmic ? (ActualXAxis as LogarithmicAxis).LogarithmicBase : 1;
                    bool xIsLogarithmic = ActualXAxis.IsLogarithmic;
                    double start = ActualXAxis.VisibleRange.Start;
                    double end = ActualXAxis.VisibleRange.End;

                    for (int i = 0; i < DataCount; i++)
                    {
                        double x, y;
                        if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed &&
                            (this.ActualXAxis as CategoryAxis).AggregateFunctions != AggregateFunctions.None)
                        {
                            if (i < xValues.Count && GroupedSeriesYValues[0].Count > i)
                            {
                                y = GroupedSeriesYValues[0][i];
                                x = xValues[i];
                            }
                            else
                                return;
                        }
                        else
                        {
                            x = xValues[i];
                            y = YValues[i];
                        }

                        double edgeValue = xIsLogarithmic ? Math.Log(x, xBase) : x;

                        if (edgeValue >= start && edgeValue <= end)
                        {
                            if (i < Adornments.Count)
                            {
                                Adornments[i].SetData(x, y, x, y);
                            }
                            else
                            {
                                Adornments.Add(this.CreateAdornment(this, x, y, x, y));
                            }

                            Adornments[i].Item = ActualData[i];
                        }
                    }
                }

                isAdornmentPending = false;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastLineBitmapSeries()
            {
                EnableAntiAliasing = this.EnableAntiAliasing,
                StrokeDashArray = this.StrokeDashArray,
                SelectedIndex = this.SelectedIndex,
                SegmentSelectionBrush = this.SegmentSelectionBrush
            });
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

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            Canvas canvas = ActualArea.GetAdorningCanvas();
            mousePos = e.GetCurrentPoint(canvas).Position;
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                UpdateTooltip(e.OriginalSource);
        }

#if NETFX_CORE
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                UpdateTooltip(e.OriginalSource);
            base.OnTapped(e);
        }
#endif
        #endregion
        
        #region Private Static Methods

        /// <summary>
        /// Method used to check the point within the polygon or not.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="pointx"></param>
        /// <param name="pointy"></param>
        /// <returns></returns>
        private static bool PointInsidePolygon(Polygon polygon, double pointX, double pointY)
        {
            int i, j;
            var inside = false;
            var points = polygon.Points;

            for (i = 0, j = points.Count - 1; i < points.Count; j = i++)
            {
                if (((points[i].Y > pointY) != (points[j].Y > pointY)) &&
                    (pointX < (points[j].X - points[i].X) * (pointY - points[i].Y) /
                    (points[j].Y - points[i].Y) + points[i].X)) inside = !inside;
            }

            return inside;
        }

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastLineBitmapSeries).UpdateArea();

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

        private static void OnSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastLineBitmapSeries).UpdateArea();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method used to get polygon points.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="leftThickness"></param>
        /// <param name="rightThickness"></param>
        /// <returns></returns>
        private PointCollection GetPolygonPoints(double x1, double y1, double x2, double y2,
                                   double leftThickness, double rightThickness)
        {
            polygonPoints.Clear();
            var dx = x2 - x1;
            var dy = y2 - y1;
            var radian = Math.Atan2(dy, dx);
            var cos = Math.Cos(-radian);
            var sin = Math.Sin(-radian);
            var x11 = (x1 * cos) - (y1 * sin);
            var y11 = (x1 * sin) + (y1 * cos);
            var x12 = (x2 * cos) - (y2 * sin);
            var y12 = (x2 * sin) + (y2 * cos);
            cos = Math.Cos(radian);
            sin = Math.Sin(radian);

            var leftTopX = (x11 * cos) - ((y11 + leftThickness) * sin);
            var leftTopY = (x11 * sin) + ((y11 + leftThickness) * cos);
            var rightTopX = (x12 * cos) - ((y12 + leftThickness) * sin);
            var rightTopY = (x12 * sin) + ((y12 + leftThickness) * cos);
            var leftBottomX = (x11 * cos) - ((y11 - rightThickness) * sin);
            var leftBottomY = (x11 * sin) + ((y11 - rightThickness) * cos);
            var rightBottomX = (x12 * cos) - ((y12 - rightThickness) * sin);
            var rightBottomY = (x12 * sin) + ((y12 - rightThickness) * cos);

            polygonPoints.Add(new Point(leftTopX, leftTopY));
            polygonPoints.Add(new Point(rightTopX, rightTopY));
            polygonPoints.Add(new Point(rightBottomX, rightBottomY));
            polygonPoints.Add(new Point(leftBottomX, leftBottomY));
            polygonPoints.Add(new Point(leftTopX, leftTopY));

            return polygonPoints;
        }

        #endregion

        #endregion
    }
}
